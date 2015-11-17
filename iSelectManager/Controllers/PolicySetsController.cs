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
    public class PolicySetsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: PolicySets
        public ActionResult Index()
        {
            return View(PolicySet.find_all());
        }

        // GET: PolicySets/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PolicySet policySet = PolicySet.find(id);
            if (policySet == null)
            {
                return HttpNotFound();
            }
            return View(policySet);
        }

        // GET: PolicySets/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PolicySet policySet = PolicySet.find(id);
            if (policySet == null)
            {
                return HttpNotFound();
            }
            ViewBag.Campaigns = Campaign.find_all();
            return View(policySet);
        }

        // POST: PolicySets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,DisplayName")] PolicySet policySet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(policySet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(policySet);
        }

        // GET: PolicySets/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PolicySet policySet = PolicySet.find(id);
            if (policySet == null)
            {
                return HttpNotFound();
            }
            return View(policySet);
        }

        // POST: PolicySets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PolicySet policySet = db.PolicySets.Find(id);
            db.PolicySets.Remove(policySet);
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
