using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public IEnumerable<string> SelectedPolicySets { get; set; }
        [Display(Name = "Policy Set")]
        public IEnumerable<SelectListItem> PolicySets { get; set; }
    }
}