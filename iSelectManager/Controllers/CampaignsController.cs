using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iSelectManager.Models;
using ININ.IceLib.Dialer.Supervisor;

namespace iSelectManager.Controllers
{
    public class CampaignsController : Controller
    {
        private DialerContext db = new DialerContext();

        // GET: Campaigns
        public ActionResult Index()
        {
            return View(Campaign.find_all());
        }

        // GET: Campaigns/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = Campaign.find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = Campaign.find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            var policysets = PolicySet.find_all();

            CampaignPolicySetsViewModel model = new CampaignPolicySetsViewModel
            {
                id = campaign.id,
                DisplayName = campaign.DisplayName,
                Campaign = campaign,
                PolicySets = policysets.Select(x => new SelectListItem { Text = x.DisplayName, Value = x.id }),
                SelectedPolicySets = campaign.PolicySets.Select(x => x.id)
            };
            return View(model);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,SelectedPolicySets")] CampaignPolicySetsViewModel model)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(campaign).State = EntityState.Modified;
                //db.SaveChanges();
                Campaign campaign = Campaign.find(model.id);

                campaign.apply_policies(model.SelectedPolicySets);
                return RedirectToAction("Index");
            }
            return View(model.Campaign);
        }

        // GET: Campaigns/Activation/5
        public ActionResult Activation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = Campaign.find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }

            CampaignActiveAgentsViewModel model = new CampaignActiveAgentsViewModel
            {
                id = campaign.id,
                DisplayName = campaign.DisplayName,
                Campaign = campaign,
                Agents = campaign.AcdWorkgroup.Agents.Select(x => new SelectListItem { Text = x.DisplayName, Value = x.id }),
                ActiveAgents =  campaign.ActiveAgents.Select(x => x.id)
            };

            return View(model);
        }

        // POST: Campaigns/Activation/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activation([Bind(Include = "id,ActiveAgents")] CampaignActiveAgentsViewModel model)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(campaign).State = EntityState.Modified;
                //db.SaveChanges();
                Campaign campaign = Campaign.find(model.id);

                campaign.activate_agents(model.ActiveAgents);
                return RedirectToAction("Index");
            }
            return View(model.Campaign);
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
