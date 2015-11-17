using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSelectManager.Models
{
    public class CampaignPolicySetsViewModel
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        public Campaign Campaign { get; set; }
        public string PolicySetLabel { get; set; }
        public IEnumerable<string> SelectedPolicySets { get; set; }
        public IEnumerable<SelectListItem> PolicySets { get; set; }
    }
}