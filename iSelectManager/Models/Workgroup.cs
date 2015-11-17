using ININ.IceLib.Configuration;
using ININ.IceLib.Dialer;
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

        public static Workgroup find(string id, IEnumerable<WorkgroupConfiguration.Property> properties = null)
        {
            var query = new WorkgroupConfigurationList(ConfigurationManager.GetInstance(Application.ICSession));
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(WorkgroupConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetRightsFilterToView();
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var results = query.GetConfigurationList();
            query.StopCaching();

            if (results.Count() == 0) throw new KeyNotFoundException(id);
            if (results.Count()  > 1) throw new IndexOutOfRangeException(id);
            return new Workgroup(results.First());
        }

        public static Workgroup find(ConfigurationId id, IEnumerable<WorkgroupConfiguration.Property> properties = null)
        {
            return find(id.Id, properties);
        }

        public static Workgroup find_by_name(string name, IEnumerable<WorkgroupConfiguration.Property> properties = null)
        {
            var query = new WorkgroupConfigurationList(ConfigurationManager.GetInstance(Application.ICSession));
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(WorkgroupConfiguration.Property.DisplayName, name, FilterMatchType.Exact);
            query_settings.SetRightsFilterToView();
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var results = query.GetConfigurationList();
            query.StopCaching();

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
                try
                {
                    Agents.Add(Agent.find(ic_member));
                }
                catch(KeyNotFoundException)
                {
                    //TODO: Trace/Warn?
                }
            }
        }

        private static List<WorkgroupConfiguration.Property> DefaultProperties   = new List<WorkgroupConfiguration.Property> { WorkgroupConfiguration.Property.Members };
        private static List<WorkgroupConfiguration.Property> MandatoryProperties = new List<WorkgroupConfiguration.Property> { WorkgroupConfiguration.Property.Id, WorkgroupConfiguration.Property.DisplayName };
    }
}