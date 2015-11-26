using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DialerNetAPIDemo.Models;

namespace DialerNetAPIDemo.Controllers
{
    public class SkillSetsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: SkillSets
        public ActionResult Index()
        {
            return View(SkillSet.find_all());
        }

        // GET: SkillSets/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SkillSet skillSet = SkillSet.find(id);
            if (skillSet == null) return HttpNotFound();
            return View(skillSet);
        }

        // GET: SkillSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SkillSets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,DisplayName,MinimumProficiency")] SkillSet skillSet)
        {
            if (ModelState.IsValid)
            {
                db.SkillSets.Add(skillSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skillSet);
        }

        // GET: SkillSets/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SkillSet skillSet = SkillSet.find(id);
            if (skillSet == null) return HttpNotFound();
            return View(skillSet);
        }

        // POST: SkillSets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,DisplayName,MinimumProficiency")] SkillSet skillSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skillSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skillSet);
        }

        // GET: SkillSets/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkillSet skillSet = db.SkillSets.Find(id);
            if (skillSet == null)
            {
                return HttpNotFound();
            }
            return View(skillSet);
        }

        // POST: SkillSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SkillSet skillSet = db.SkillSets.Find(id);
            db.SkillSets.Remove(skillSet);
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
