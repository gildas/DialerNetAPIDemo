using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSelectManager.Models;

namespace iSelectManager.Controllers
{
    public class WorkgroupsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: Workgroups
        public ActionResult Index()
        {
            return View(Workgroup.find_all(false));
        }

        // GET: Workgroups/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workgroup workgroup = db.Workgroups.Find(id);
            if (workgroup == null)
            {
                return HttpNotFound();
            }
            return View(workgroup);
        }

        // GET: Workgroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Workgroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,DisplayName")] Workgroup workgroup)
        {
            if (ModelState.IsValid)
            {
                db.Workgroups.Add(workgroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(workgroup);
        }

        // GET: Workgroups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workgroup workgroup = db.Workgroups.Find(id);
            if (workgroup == null)
            {
                return HttpNotFound();
            }
            return View(workgroup);
        }

        // POST: Workgroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,DisplayName")] Workgroup workgroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workgroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workgroup);
        }

        // GET: Workgroups/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workgroup workgroup = db.Workgroups.Find(id);
            if (workgroup == null)
            {
                return HttpNotFound();
            }
            return View(workgroup);
        }

        // POST: Workgroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Workgroup workgroup = db.Workgroups.Find(id);
            db.Workgroups.Remove(workgroup);
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
