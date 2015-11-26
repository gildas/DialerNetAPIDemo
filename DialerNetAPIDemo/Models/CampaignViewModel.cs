using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DialerNetAPIDemo.Models
{
    public class CampaignViewModel
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        public Campaign Campaign { get; set; }
        [Display(Name = "Policy Set")]
        public IEnumerable<string> SelectedPolicySets { get; set; }
        public IEnumerable<SelectListItem> PolicySets { get; set; }
        [Display(Name = "Active Agents")]
        public IEnumerable<string> ActiveAgents { get; set; }
        public IEnumerable<SelectListItem> Agents { get; set; }
    }
}
