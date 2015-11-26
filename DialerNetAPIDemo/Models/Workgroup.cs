using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.Dialer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class Workgroup
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        [Display(Name="Agents")]
        public ICollection<Agent> Agents { get; set; }

        private WorkgroupConfiguration configuration { get; set; }

        public static ICollection<Workgroup> find_all(bool resolve_members = true)
        {
            return Application.WorkgroupConfigurations.Select(item => new Workgroup(item, resolve_members)).ToList();
        }

        public static Workgroup find(string id, bool resolve_members = true)
        {
            try
            {
                return new Workgroup(Application.WorkgroupConfigurations.First(item => item.ConfigurationId.Id == id), resolve_members);
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static Workgroup find(ConfigurationId id, bool resolve_members = true)
        {
            return find(id.Id, resolve_members);
        }

        public static Workgroup find_by_name(string name, bool resolve_members = true)
        {
            try
            {
                return new Workgroup(Application.WorkgroupConfigurations.First(item => item.ConfigurationId.DisplayName == name), resolve_members);
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, name));
            }
        }

        public Workgroup()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            Agents = new List<Agent>();
            configuration = null;
        }

        public Workgroup(WorkgroupConfiguration ic_configuration, bool resolve_members = true)
        {
            id = ic_configuration.ConfigurationId.Id;
            DisplayName = ic_configuration.ConfigurationId.DisplayName;
            Agents = new List<Agent>();
            if (resolve_members)
            {
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
        }

        private static ReadOnlyCollection<WorkgroupConfiguration> WorkgroupConfigurations
        {
            get
            {
                if (_WorkgroupConfigurations == null)
                {
                    try
                    {
                        var configurations = new WorkgroupConfigurationList(new DialerConfigurationManager(Application.ICSession).ConfigurationManager);
                        var query_settings = configurations.CreateQuerySettings();

                        query_settings.SetPropertiesToRetrieve(new[] { 
                            WorkgroupConfiguration.Property.Id,
                            WorkgroupConfiguration.Property.DisplayName,
                            WorkgroupConfiguration.Property.Members
                        });
                        configurations.StartCaching(query_settings);
                        _WorkgroupConfigurations = configurations.GetConfigurationList();
                    }
                    catch(Exception e)
                    {
                        HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve workgroups", e);
                        _WorkgroupConfigurations = new List<WorkgroupConfiguration>().AsReadOnly();
                    }
                }
                return _WorkgroupConfigurations;
            }
        }
        private static ReadOnlyCollection<WorkgroupConfiguration> _WorkgroupConfigurations = null;
    }
}
