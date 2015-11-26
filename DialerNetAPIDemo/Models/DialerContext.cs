using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class DialerContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public DialerContext() : base("name=DialerContext")
        {
        }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.ContactList> ContactLists { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.Campaign> Campaigns { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.Agent> Agents { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.PolicySet> PolicySets { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.Workgroup> Workgroups { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.CampaignViewModel> CampaignViewModels { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.Skill> Skills { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.SkillSet> SkillSets { get; set; }

        public System.Data.Entity.DbSet<DialerNetAPIDemo.Models.Contact> Contacts { get; set; }
    
    }
}
