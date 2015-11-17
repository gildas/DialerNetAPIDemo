using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace iSelectManager.Models
{
    public class Campaign
    {
        public string id { get; set; }
        [Display(Name="Campaign")]
        public string DisplayName { get; set; }
        [Display(Name = "Workgroup")]
        public string AcdWorkgroup { get; set; }
        [Display(Name="Contact List")]
        public ContactList ContactList { get; set; }
        [Display(Name="Policy sets")]
        public ICollection<PolicySet> PolicySets { get; set; }

        private ININ.IceLib.Configuration.Dialer.CampaignConfiguration configuration { get; set; }

        public static ICollection<Campaign> find_all()
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();
            var campaigns = new List<Campaign>();

            query_settings.SetPropertiesToRetrieveToAll();
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            foreach (var configuration in configurations)
            {
                campaigns.Add(new Campaign(configuration));
            }
            return campaigns;
        }

        public static Campaign find(string id)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(CampaignConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieveToAll();
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(id);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(id);
            return new Campaign(configurations.First());
        }

        public static Campaign find_by_name(string name)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(CampaignConfiguration.Property.DisplayName, name, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieveToAll();
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(name);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(name);
            return new Campaign(configurations.First());
        }

        public Campaign()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            AcdWorkgroup = string.Empty;
            ContactList = null;
            PolicySets = new List<PolicySet>();
            configuration = null;
        }

        public Campaign(ININ.IceLib.Configuration.Dialer.CampaignConfiguration ic_campaign)
        {
            id = ic_campaign.ConfigurationId.Id;
            DisplayName = ic_campaign.ConfigurationId.DisplayName;
            AcdWorkgroup = ic_campaign.AcdWorkgroup.Value.DisplayName;
            try
            {
                ContactList = ContactList.find(ic_campaign.ContactList.Value.Id);
            }
            catch(KeyNotFoundException)
            {
                //TODO: Trace/Warn?
            }

            PolicySets = new List<PolicySet>();
            foreach (var ic_policyset in ic_campaign.PolicySets.Value)
            {
                try
                {
                    PolicySets.Add(PolicySet.find(ic_policyset.Id));
                }
                catch (KeyNotFoundException)
                {
                    //TODO: Trace/Warn?
                }
            }
            configuration = ic_campaign;
        }

        public void apply_policies(IEnumerable<string> policy_ids)
        {
            if (policy_ids.Count() == 0)
            {
                return;
            }
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new PolicySetConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query.StartCaching(query_settings);
            var policyset_configurations = query.GetConfigurationList();
            query.StopCaching();

            configuration.PrepareForEdit();
            configuration.PolicySets.Value.Clear();
            foreach (var ic_policyset in policyset_configurations)
            {
                if (policy_ids.Contains(ic_policyset.ConfigurationId.Id))
                {
                    configuration.PolicySets.Value.Add(ic_policyset.ConfigurationId);
                }
            }
            configuration.Commit();
        }
    }
}