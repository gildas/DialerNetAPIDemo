using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSelectManager.Models
{
    public class PolicySet
    {
        public string id { get; set; }
        [Display(Name="Policy Set")]
        public string DisplayName { get; set; }

        private ININ.IceLib.Configuration.Dialer.PolicySetConfiguration configuration { get; set; }

        public static ICollection<PolicySet> find_all(IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new PolicySetConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();
            var policysets = new List<PolicySet>();

            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            foreach (var configuration in configurations)
            {
                policysets.Add(new PolicySet(configuration));
            }
            return policysets;
        }

        public static ICollection<PolicySet> find_all_by_id(IEnumerable<string> ids, IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new PolicySetConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();
            var policysets = new List<PolicySet>();

            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            foreach (var configuration in configurations)
            {
                if (ids.Contains(configuration.ConfigurationId.Id))
                {
                    policysets.Add(new PolicySet(configuration));
                }
            }
            return policysets;
        }

        public static PolicySet find(string id, IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new PolicySetConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(PolicySetConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(id);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(id);
            return new PolicySet(configurations.First());
        }

        public static PolicySet find(ConfigurationId id, IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            return find(id.Id, properties);
        }

        public static PolicySet find_by_name(string name, IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new PolicySetConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(PolicySetConfiguration.Property.DisplayName, name, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(name);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(name);
            return new PolicySet(configurations.First());
        }

        public PolicySet()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            configuration = null;
        }

        public PolicySet(ININ.IceLib.Configuration.Dialer.PolicySetConfiguration ic_policyset)
        {
            id = ic_policyset.ConfigurationId.Id;
            DisplayName = ic_policyset.ConfigurationId.DisplayName;
            configuration = ic_policyset;
        }

        private static List<PolicySetConfiguration.Property> DefaultProperties   = new List<PolicySetConfiguration.Property>();
        private static List<PolicySetConfiguration.Property> MandatoryProperties = new List<PolicySetConfiguration.Property> { PolicySetConfiguration.Property.Id, PolicySetConfiguration.Property.DisplayName };
    }
}