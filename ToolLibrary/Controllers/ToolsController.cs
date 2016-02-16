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
                return View(db.Tools.ToList());
            }

            var tools = db.Tools.
                Include("Category").
                Where(b => b.Category.Id == categoryId).
                OrderByDescending(b => b.Name).
                ToList();

            ViewBag.SelectedCategoryId = categoryId;
            
            return View(tools);
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
            return View(tool);
        }

        [HttpPost]
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
            rental.CheckedOut = checkedOut;
            rental.DueDate = dueDate;
            rental.UserID = userId;
            rental.Tool = tool;
            tool.IsCheckedOut = true;
            db.Rentals.Add(rental);
            db.SaveChanges();
            db.Entry(tool).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.CheckedOut = true;
            ViewBag.AlertMessage = "You have checked out this item.";
            return View(tool);
        }

        // GET: Tools/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Manufacturer,AdditionalDescription,ImageUrl,Type,Barcode,Name,Description,IsCheckedOut")] Tool tool)
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
            return View(tool);
        }

        // POST: Tools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Manufacturer,AdditionalDescription,ImageUrl,Type,Barcode,Name,Description,IsCheckedOut")] Tool tool)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tool).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
    }
}
