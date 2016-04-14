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
using System.Net.Mail;
using PagedList;

namespace ToolLibrary.Controllers
{
    public class ToolsController : Controller
    {
        private ToolDbContext db = new ToolDbContext();

        public ToolsController()
        {
           
        }

        // GET: Tools
        public ActionResult Index(int? categoryId, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "description";
            ViewBag.CategoryID = categoryId;
            ViewBag.selectedCategoryId = categoryId;
            if (searchString != null) {
                page = 1;
            }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;

            if (categoryId == null)
            {
                ViewBag.SelectedCategoryId = categoryId;
                var tools = db.Tools.Include(t => t.Category);
                if (!String.IsNullOrEmpty(searchString))
                    tools = Search(searchString, tools);
                tools = Sort(sortOrder, tools);
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                return View(tools.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var tools = db.Tools.Where(t => t.CategoryId == categoryId).Include(t => t.Category);
                if (!String.IsNullOrEmpty(searchString)) 
                    tools = Search(searchString, tools); 
                tools = Sort(sortOrder, tools);             
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                return View(tools.ToPagedList(pageNumber, pageSize));
            }   
        }

        // GET: Tools/Details/5
        public ActionResult Details(int? id, int? categoryId)
        {
            ViewBag.selectedCategoryId = categoryId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }

            var checkedOutTool = (from t in db.Tools
                                  join r in db.Rentals on t.Id equals r.Tool.Id
                                  where t.Id == id
                                  orderby r.CheckedOut
                                  select t);

            var rentals = (from r in db.Rentals
                           where r.Tool.Id == id
                           select r).ToList();

            var detailViewModel = new DetailViewModel();
            detailViewModel.tool = tool;
            detailViewModel.CategoryId = Convert.ToInt16(categoryId);
            ViewBag.CategoryId = tool.CategoryId;
            if (rentals.Count > 0)
            {
                detailViewModel.Rentals = rentals;
                List<DateTime> reservedDates = GetCheckedOutDates(tool.Id);
                detailViewModel.ReservedDates = reservedDates;
                detailViewModel.TriggerOnLoad = false;
                detailViewModel.TriggerOnLoadMessage = "";
                return View(detailViewModel);
            }
            else
            {
                detailViewModel.Rentals = rentals;
                detailViewModel.tool = tool;
                detailViewModel.TriggerOnLoad = false;
                detailViewModel.TriggerOnLoadMessage = "";
                List <DateTime> ReservedDates = GetCheckedOutDates(tool.Id);
                detailViewModel.ReservedDates = ReservedDates;              
                return View(detailViewModel);
            }
                 
            //DetailViewModel viewModel = new DetailViewModel();
            //viewModel.tool = tool;
            //detailViewModel.CategoryId = Convert.ToInt16(categoryId);
                         
            //List<DateTime> ReservedDates = GetCheckedOutDates(tool.Id);
            //viewModel.ReservedDates = ReservedDates;
            //ViewBag.CategoryId = tool.CategoryId;
               
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(FormCollection form)
        {
            var userId = User.Identity.GetUserId();
            DateTime checkedOut = DateTime.Parse(form["txtOut"]);
            DateTime dueDate = DateTime.Parse(form["txtReturn"]);
            ViewBag.CategoryId = form["categoryId"];
            string categoryId = form["categoryId"];
            
            if (checkedOut == default(DateTime))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (dueDate == default(DateTime))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int id = Convert.ToInt16(form["id"]);

            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }

            // Check whether checked out more than seven days.
            TimeSpan difference = dueDate - checkedOut;
            if (difference.TotalDays > 7)
            {
                TempData["isError"] = true;
                TempData["Message"] = "Date range greater than 7 days.";
                return RedirectToAction("Details", "Tools", new { id = id });
            }

            // Check to see if conflicts with another rental
            List<Rental> rentalList = (from r in db.Rentals
                                       where r.Tool.Id == id
                                       select r).ToList();
            if (rentalList.Count > 0)
            {
                // determine if already rented by user
                foreach (Rental r in rentalList)
                {
                    if (r.UserID == userId)
                    {
                        TempData["isError"] = true;
                        TempData["Message"] = "Item already checked out by user.";
                        return RedirectToAction("Details", "Tools", new { id = id, categoryId = categoryId });
                    }
                }

                Rental rental = new Rental();
                rental.CheckedOut = checkedOut;
                rental.DueDate = dueDate;
                rental.Tool = tool;
                rental.UserID = userId;
                if (AllowedToAdd(rentalList, rental))
                {
                    tool.IsCheckedOut = true;
                    db.Rentals.Add(rental);
                    db.SaveChanges();
                    db.Entry(tool).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("Checkout", rental);
                }
                else
                {
                    TempData["isError"] = true;
                    TempData["Message"] = "Problem with checkout dates. Please verify no conflicts.";
                    return RedirectToAction("Details", "Tools", new { id = id, categoryId = categoryId });
                }               
            }
            else
            {
                List<DateTime> ReservedDates = new List<DateTime>();
                ReservedDates = GetCheckedOutDates(tool.Id);

                foreach (DateTime d in ReservedDates)
                {
                    if (dueDate >= d && checkedOut < d) //overlapping dates
                    {
                        TempData["isError"] = true;
                        TempData["Message"] = "Problem with checkout dates. Please verify no conflicts.";
                        return RedirectToAction("Details", "Tools", new { id = id, categoryId = categoryId });
                    }
                }
                Rental rental = new Rental();
                rental.CheckedOut = checkedOut;
                rental.DueDate = dueDate;
                rental.Tool = tool;
                rental.UserID = userId;
                tool.IsCheckedOut = true;
                db.Rentals.Add(rental);
                db.SaveChanges();
                db.Entry(tool).State = EntityState.Modified;
                db.SaveChanges();
                return View("Checkout", rental);
            }

            /*List<DateTime> ReservedDates = new List<DateTime>();
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



            try
            { // no overlap
                rental.CheckedOut = checkedOut;
                rental.DueDate = dueDate;
                rental.UserID = userId;
                rental.Tool = tool;
                tool.IsCheckedOut = true;
                db.Rentals.Add(rental);
                db.SaveChanges();
                db.Entry(tool).State = EntityState.Modified;
                db.SaveChanges();

                //send email
            }
            catch
            {
                DateTime due = ReservedDates.FirstOrDefault();
                DetailViewModel dd = CreateDetailviewModel(tool, due);
                dd.ReservedDates = ReservedDates;
                dd.TriggerOnLoad = true;
                dd.TriggerOnLoadMessage = "There was a problem creating this request. Please contact teh administrator.";
                dd.RedirectUrl = "Details";
                return View(dd);
            };

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

            return View(dt);*/
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
                return RedirectToAction("Index", "Tools", new { categoryId = tool.CategoryId });
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", tool.CategoryId);
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
                for (DateTime dt = r.CheckedOut; dt <= r.DueDate; dt = dt.AddDays(1))
                {
                    dates.Add(dt);
                }
            };

            return dates;
        }

        private DetailViewModel CreateDetailviewModel(Tool tool, DateTime dueDate)
        {
            DetailViewModel dt = new DetailViewModel();
            dt.tool = tool;
            return dt;
        }

        [HttpPost]
        public ActionResult Search(string searchString)
        {
            var movies = from m in db.Tools
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Name.Contains(searchString));
            }

            return View(movies);
        }

        private IQueryable<Tool> Search (string searchString, IQueryable<Tool> tools)
        {
            var toolSearchResult = tools.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) || s.Description.ToUpper().Contains(searchString.ToUpper()));
            return toolSearchResult;
        }

        private IQueryable<Tool> Sort (string sortOrder, IQueryable<Tool> tools)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    tools = tools.OrderByDescending(s => s.Name);
                    return tools;
                case "description":
                    tools = tools.OrderBy(s => s.Description);
                    return tools;
                case "description_desc":
                    tools = tools.OrderByDescending(s => s.Description);
                    return tools;
                default:
                    tools = tools.OrderBy(s => s.Name);
                    return tools;
            };
        }

        public bool AllowedToAdd(List<Rental> rentalList, Rental newRental)
        {
            bool OKToAdd = true;
            DateTime Test_period_start = newRental.CheckedOut;
            DateTime Test_period_end = newRental.DueDate;
            foreach (Rental r in rentalList)
            {
                DateTime Base_period_start = r.CheckedOut;
                DateTime Base_period_end = r.DueDate;
                if (TimePeriodOverlap(Base_period_start, Base_period_end, Test_period_start, Test_period_end))
                {
                    OKToAdd = false;
                }          
            }
            return OKToAdd;            
        }

        public bool TimePeriodOverlap(DateTime BS, DateTime BE, DateTime TS, DateTime TE)
        {
            return (
                (TS >= BS && TS < BE) || (TE <= BE && TE > BS) || (TS <= BS && TE >= BE)
            );
        }
    }
}
