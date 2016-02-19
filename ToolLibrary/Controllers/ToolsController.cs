using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToolLibrary.DAL;
using ToolLibrary.Models;
using Microsoft.AspNet.Identity;

namespace ToolLibrary.Controllers
{
    public class ToolsController : Controller
    {
        private ToolDbContext db = new ToolDbContext();

        public ToolsController()
        {
            //AutoMapper.Mapper.CreateMap<Tool, ToolViewModel>();
        }

        // GET: Tools
        public ActionResult Index(int? categoryId)
        {
            if (categoryId == null)
            {
                //var tools = db.Tools;
                ViewBag.CategoryID = categoryId;
                var tools = db.Tools.Include(t => t.Category);
                return View(tools.ToList());
            }
            else
            {
                //var tools = db.Tools;
                //var tools = db.Tools.Include(t => t.Category);
                var tools = db.Tools.Where(t => t.CategoryId == categoryId).Include(t => t.Category);
                //var tools = db.Tools.
                //Include("Category").
                //Where(b => b.CategoryId == categoryId).
                //OrderByDescending(b => b.Name).
                //ToList();
                //ViewBag.SelectedCategoryId = categoryId;
                return View(tools.ToList());
            }   
           
            //return View(AutoMapper.Mapper.Map<List<Tool>, List<ToolViewModel>>(tools));
            //return View(db.Tools.Include("Category").ToList());
        }

        // GET: Tools/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            if (tool.IsCheckedOut) // is in rentals table
            {
                var checkedOutTool = (from t in db.Tools
                                      join r in db.Rentals on t.Id equals r.Tool.Id
                                      where t.Id == id
                                      orderby r.CheckedOut
                                      select new DetailViewModel {
                                            Id = t.Id,
                                            Name = t.Name,
                                            Description = t.Description,
                                            AdditionalDescription = t.AdditionalDescription,
                                            CategoryId = t.CategoryId,
                                            Manufacturer = t.Manufacturer,
                                            ImageUrl = t.ImageUrl,
                                            Due = r.DueDate,
                                            TriggerOnLoad = false,
                                            TriggerOnLoadMessage = "",}).FirstOrDefault();
                checkedOutTool.ReservedDates = GetCheckedOutDates(checkedOutTool.Id);
                return View(checkedOutTool);
            }
            
            DetailViewModel viewModel = new DetailViewModel();
            viewModel.Id = tool.Id;
            viewModel.Name = tool.Name;
            viewModel.Description = tool.Description;
            viewModel.AdditionalDescription = tool.AdditionalDescription;
            viewModel.Manufacturer = tool.Manufacturer;
            viewModel.ImageUrl = tool.ImageUrl;
            viewModel.CategoryId = tool.CategoryId;
            viewModel.Due = DateTime.MinValue;
            viewModel.AdditionalDescription = tool.Description;
            viewModel.TriggerOnLoad = false;
            viewModel.TriggerOnLoadMessage = "";
                 
            List<DateTime> ReservedDates = GetCheckedOutDates(tool.Id);
            viewModel.ReservedDates = ReservedDates;
            ViewBag.CategoryId = tool.CategoryId;
            return View(viewModel);   
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(FormCollection form)
        {
            var userId = User.Identity.GetUserId();
            DateTime checkedOut = new DateTime();
            DateTime dueDate = new DateTime();

            if (form["txtOut"].ToString() != "")
            {
                checkedOut = DateTime.Parse(form["txtOut"].ToString());
            }

            if (form["txtReturn"].ToString() != "")
            {
                dueDate = DateTime.Parse(form["txtReturn"].ToString()); 
            }
            
            if (checkedOut == default(DateTime))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (dueDate == default(DateTime))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? id = Convert.ToInt16(form["id"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }

            Rental rental = new Rental();
            
            List<DateTime> ReservedDates = new List<DateTime>();
            ReservedDates = GetCheckedOutDates(tool.Id);
            foreach (DateTime d in ReservedDates)
            {
                if (dueDate >= d && checkedOut < d) //overlapping dates
                {
                    DetailViewModel dd = CreateDetailviewModel(tool, d);
                    dd.ReservedDates = ReservedDates;
                    dd.TriggerOnLoad = true;
                    dd.TriggerOnLoadMessage = "Conflict with another reservation. Please choose new dates.";
                    dd.RedirectUrl = "Details";
                    return View(dd);
                } 
            }

            // no overlap
            rental.CheckedOut = checkedOut;
            rental.DueDate = dueDate;
            rental.UserID = userId;
            rental.Tool = tool;
            tool.IsCheckedOut = true;
            db.Rentals.Add(rental);
            db.SaveChanges();
            db.Entry(tool).State = EntityState.Modified;
            db.SaveChanges();

            //return view model
            DetailViewModel dt = CreateDetailviewModel(tool, dueDate);           
            for (var newCheckedOutDates = rental.CheckedOut; newCheckedOutDates <= rental.DueDate; newCheckedOutDates = newCheckedOutDates.AddDays(1))
            {
                ReservedDates.Add(newCheckedOutDates);
            }
            ViewBag.CategoryId = ViewBag.CategoryId;
            dt.ReservedDates = ReservedDates;
            dt.TriggerOnLoad = true;
            dt.TriggerOnLoadMessage = "Item checked out successfully!";
            dt.RedirectUrl = "Index";

            return View(dt);
        }

        // GET: Tools/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Tools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Manufacturer,AdditionalDescription,ImageUrl,Type,CategoryId,Barcode,Name,Description,IsCheckedOut")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                db.Tools.Add(tool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tool);
        }

        // GET: Tools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", tool.CategoryId);
            //PopulateCategoriesDropDownList(tool.Category);
            return View(tool);
        }

        // POST: Tools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Manufacturer,AdditionalDescription,ImageUrl,Type,CategoryId,Barcode,Name,Description,IsCheckedOut")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tool).State = EntityState.Modified;
                db.SaveChanges();
                string redirectUrl = "Index?categoryId=?" + tool.CategoryId;
                //return RedirectToAction(redirectUrl);
                return RedirectToAction("Index", "Tools", new { categoryId = tool.CategoryId });
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", tool.CategoryId);
            //PopulateCategoriesDropDownList(tool.CategoryId);
            return View(tool);
        }

        // GET: Tools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tool tool = db.Tools.Find(id);
            db.Tools.Remove(tool);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<DateTime> GetCheckedOutDates(int id)
        {
            var rentals = from r in db.Rentals
                          where r.Tool.Id == id
                          select new { r.CheckedOut, r.DueDate };

            var dates = new List<DateTime>();

            foreach (var r in rentals)
            {
                for (var dt = r.CheckedOut; dt <= r.DueDate; dt = dt.AddDays(1))
                {
                    dates.Add(dt);
                }
            };

            return dates;
        }

        private DetailViewModel CreateDetailviewModel(Tool tool, DateTime dueDate)
        {
            DetailViewModel dt = new DetailViewModel();
            dt.Id = tool.Id;
            dt.Name = tool.Name;
            dt.Description = tool.Description;
            dt.AdditionalDescription = tool.AdditionalDescription;
            dt.CategoryId = tool.CategoryId;
            dt.Due = dueDate;
            dt.ImageUrl = tool.ImageUrl;
            dt.Manufacturer = tool.Manufacturer;
            return dt;
        }
        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoryQuery = from d in db.Categories
                                   orderby d.Name
                                   select d;
            ViewBag.CategoryID = new SelectList(categoryQuery, "CategoryID", "Name", selectedCategory);
        }
    }
}
