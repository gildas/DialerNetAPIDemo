using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSelectManager.Models
{
    public class CampaignPolicySetsViewModel
    {
        public string id { get { return Campaign.id; } }
        public string DisplayName { get { return Campaign.DisplayName; } }
        public Campaign Campaign { get; set; }
        public string PolicySetLabel { get; set; }
        public IEnumerable<string> SelectedPolicySets { get; set; }
        public IEnumerable<SelectListItem> PolicySets { get; set; }
    }
}