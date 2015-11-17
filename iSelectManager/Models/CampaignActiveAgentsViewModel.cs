using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSelectManager.Models
{
    public class CampaignActiveAgentsViewModel
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        public Campaign Campaign { get; set; }
        [Display(Name = "Active Agents")]
        public IEnumerable<string> ActiveAgents { get; set; }
        public IEnumerable<SelectListItem> Agents { get; set; }
    }
}