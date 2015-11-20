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
    public class ContactsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: Contacts
        public ActionResult Index(string contactlist_id)
        {
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            return View(Contact.find_all(contactList));
        }

        // GET: Contacts/Details/5
        public ActionResult Details(string id, string contactlist_id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            var contact = Contact.find(contactList, id);
            if (contact == null) return HttpNotFound();
            return View(contact);
        }

        // GET: Contacts/Create
        public ActionResult Create(string contactlist_id)
        {
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            var columns = new Dictionary<string, object>();

            contactList.columns.ForEach(column => columns.Add(column, null));
            return View(new Contact { ContactList = contactList, Columns = columns });
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string contactlist_id, FormCollection formdata)
        {
            if (ModelState.IsValid)
            {
                //db.Contacts.Add(contact);
                //db.SaveChanges();
                if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var contactList = ContactList.find(contactlist_id);
                if (contactList == null) return HttpNotFound();

                var columns = new Dictionary<string, object>();

                foreach(var key in formdata.AllKeys)
                {
                    if (key.Equals("__RequestVerificationToken", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (formdata[key] == null || string.IsNullOrWhiteSpace(formdata[key]))                     continue;
                    columns.Add(key, formdata[key]);
                }
                contactList.insert_contact(columns, "J");
                return RedirectToAction("Index", new { contactlist_id = contactList.id });
            }
            return View(formdata);
        }

        // GET: Contacts/Edit/5
        public ActionResult Edit(string id, string contactlist_id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            var contact = Contact.find(contactList, id);
            if (contact == null) return HttpNotFound();
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, string contactlist_id, FormCollection formdata)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(contact).State = EntityState.Modified;
                //db.SaveChanges();
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var contactList = ContactList.find(contactlist_id);
                if (contactList == null) return HttpNotFound();
                var contact = Contact.find(contactList, id);
                if (contact == null) return HttpNotFound();

                var columns = new Dictionary<string, object>();

                foreach(var key in formdata.AllKeys)
                {
                    if (key.Equals("__RequestVerificationToken", StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (key.Equals("id",                         StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (formdata[key] == null || string.IsNullOrWhiteSpace(formdata[key]))                     continue;
                    columns.Add(key, formdata[key]);
                }
                contactList.update_contact(contact, columns);
                return RedirectToAction("Index", new { contactlist_id = contactList.id });
            }
            return View(formdata);
        }

        // GET: Contacts/Delete/5
        public ActionResult Delete(string id, string contactlist_id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            var contact = Contact.find(contactList, id);
            if (contact == null) return HttpNotFound();
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string contactlist_id)
        {
            //Contact contact = db.Contacts.Find(id);
            //db.Contacts.Remove(contact);
            //db.SaveChanges();
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (contactlist_id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contactList = ContactList.find(contactlist_id);
            if (contactList == null) return HttpNotFound();
            var contact = Contact.find(contactList, id);
            if (contact == null) return HttpNotFound();
            int deleted = contactList.delete_contact(contact);
            return RedirectToAction("Index", new { contactlist_id = contactList.id, record_count = deleted });
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
