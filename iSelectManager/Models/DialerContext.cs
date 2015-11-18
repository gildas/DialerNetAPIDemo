using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace iSelectManager.Models
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

        public System.Data.Entity.DbSet<iSelectManager.Models.ContactList> ContactLists { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.Campaign> Campaigns { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.Agent> Agents { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.PolicySet> PolicySets { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.Workgroup> Workgroups { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.CampaignViewModel> CampaignViewModels { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.Skill> Skills { get; set; }

        public System.Data.Entity.DbSet<iSelectManager.Models.SkillSet> SkillSets { get; set; }
    
    }
}
