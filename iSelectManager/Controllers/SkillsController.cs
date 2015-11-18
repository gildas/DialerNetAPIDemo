using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSelectManager.Models;
using System.Web.Configuration;

namespace iSelectManager.Controllers
{
    public class SkillsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: Skills
        public ActionResult Index()
        {
            return View(Skill.find_all());
        }

        // GET: Skills/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Skill skill = Skill.find(id);
            if (skill == null) return HttpNotFound();
            return View(skill);
        }

        // GET: Skills/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Skills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DisplayName")] Skill skill, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    Skill.upload(Uploader.Process(upload));
                }
                else
                {
                    //db.Skills.Add(skill);
                    //db.SaveChanges();
                    Skill.create(skill.DisplayName);
                }
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        // GET: Skills/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Skill skill = Skill.find(id);
            if (skill == null) return HttpNotFound();
            return View(skill);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,DisplayName")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skill);
        }

        // GET: Skills/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Skill skill = Skill.find(id);
            if (skill == null) return HttpNotFound();
            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Skill skill = db.Skills.Find(id);
            db.Skills.Remove(skill);
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
