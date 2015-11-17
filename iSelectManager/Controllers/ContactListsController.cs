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
    public class ContactListsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: ContactLists
        public ActionResult Index()
        {
            //return View(db.ContactLists.ToList());
            return View(ContactList.find_all());
        }

        // GET: ContactLists/Details/5
        public ActionResult Details(string id, int affected_records = -1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = ContactList.find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            contactList.AffectedRecords = affected_records;
            return View(contactList);
        }

        // GET: ContactLists/UpdateSkill
        public ActionResult UpdateSkill(string id, string name, int skill_index, string skill)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = ContactList.find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            int affected_records = contactList.UpdateContacts("LASTNAME", name, string.Format("SKILL{0}", skill_index), skill);

            return RedirectToAction("Details", new { id = id, affected_records = affected_records });
        }

        // GET: ContactLists/UpdateCampaign
        public ActionResult UpdateCampaign(string id, string name, string campaign)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = ContactList.find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            int affected_records = contactList.UpdateContacts("LASTNAME", name, "CAMPAIGNNAMEFILTER", campaign);

            return RedirectToAction("Details", new { id = id, affected_records = affected_records });
        }

        // GET: ContactLists/UpdateStatus
        public ActionResult UpdateStatus(string id, string name, string status)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = ContactList.find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            int affected_records = contactList.UpdateContactStatuses("LASTNAME", name, status);

            return RedirectToAction("Details", new { id = id, affected_records = affected_records });
        }

        // GET: ContactLists/UpdateStatus
        public ActionResult ScheduleCall(string id, string name, string campaign, DateTime when)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = ContactList.find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            var agent_id = "";
            var site_id = "";
            
            int affected_records = contactList.ScheduleCall("LASTNAME", name, Campaign.find_by_name(campaign), agent_id, site_id, when);

            return RedirectToAction("Details", new { id = id, affected_records = affected_records });
        }

        // GET: ContactLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,display_name")] ContactList contactList)
        {
            if (ModelState.IsValid)
            {
                db.ContactLists.Add(contactList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactList);
        }

        // GET: ContactLists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = db.ContactLists.Find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            return View(contactList);
        }

        // POST: ContactLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,display_name")] ContactList contactList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactList);
        }

        // GET: ContactLists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactList contactList = db.ContactLists.Find(id);
            if (contactList == null)
            {
                return HttpNotFound();
            }
            return View(contactList);
        }

        // POST: ContactLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ContactList contactList = db.ContactLists.Find(id);
            db.ContactLists.Remove(contactList);
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
