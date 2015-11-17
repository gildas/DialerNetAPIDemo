using ININ.IceLib.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSelectManager.Models
{
    public class Workgroup
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        [Display(Name="Agents")]
        public ICollection<Agent> Agents { get; set; }

        private WorkgroupConfiguration configuration { get; set; }

        internal static Workgroup find_by_name(string name)
        {
            var manager = ConfigurationManager.GetInstance(Application.ICSession);
            var configurations = new WorkgroupConfigurationList(manager);
            var querySettings = configurations.CreateQuerySettings();

            querySettings.SetFilterDefinition(WorkgroupConfiguration.Property.DisplayName, name, FilterMatchType.Exact);
            querySettings.SetRightsFilterToView();
            querySettings.SetPropertiesToRetrieve(new[] { WorkgroupConfiguration.Property.Id, WorkgroupConfiguration.Property.DisplayName, WorkgroupConfiguration.Property.Members });
            configurations.StartCaching(querySettings);

            var results = configurations.GetConfigurationList();

            configurations.StopCaching();

            if (results.Count() == 0) throw new KeyNotFoundException(name);
            if (results.Count()  > 1) throw new IndexOutOfRangeException(name);
            return new Workgroup(results.First());
        }

        public Workgroup()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            Agents = new List<Agent>();
            configuration = null;
        }

        public Workgroup(WorkgroupConfiguration ic_configuration)
        {
            id = ic_configuration.ConfigurationId.Id;
            DisplayName = ic_configuration.ConfigurationId.DisplayName;

            Agents = new List<Agent>();
            foreach(var ic_member in ic_configuration.Members.Value)
            {
                Agents.Add(new Agent { id = ic_member.Id, DisplayName = ic_member.DisplayName });
            }

            configuration = ic_configuration;
        }
    }
}