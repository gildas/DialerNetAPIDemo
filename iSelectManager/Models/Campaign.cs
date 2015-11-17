using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.Dialer.Supervisor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Workgroup AcdWorkgroup { get; set; }
        [Display(Name="Contact List")]
        public ContactList ContactList { get; set; }
        [Display(Name="Policy sets")]
        public ICollection<PolicySet> PolicySets { get; set; }
        public ICollection<Agent> ActiveAgents
        {
            get
            {
                if (_active_agents == null)
                {
                    var agent_manager = new AgentManager(Application.ICSession);

                    _active_agents = new List<Agent>();
                    foreach (var agent_id in agent_manager.GetActiveAgentsForCampaign(configuration.ConfigurationId))
                    {
                        try
                        {
                            _active_agents.Add(Agent.find(agent_id));
                        }
                        catch(KeyNotFoundException)
                        {
                            //TODO: Trace/Warn?
                        }
                    }

                }
                return _active_agents;
            }
        }

        private ININ.IceLib.Configuration.Dialer.CampaignConfiguration configuration { get; set; }

        public static ICollection<Campaign> find_all(IEnumerable<CampaignConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();
            var campaigns = new List<Campaign>();

            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            foreach (var configuration in configurations)
            {
                campaigns.Add(new Campaign(configuration));
            }
            return campaigns;
        }

        public static Campaign find(string id, IEnumerable<CampaignConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(CampaignConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(id);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(id);
            return new Campaign(configurations.First());
        }

        public static Campaign find(ConfigurationId id, IEnumerable<CampaignConfiguration.Property> properties = null)
        {
            return find(id.Id, properties);
        }

        public static Campaign find_by_name(string name, IEnumerable<CampaignConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new CampaignConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(CampaignConfiguration.Property.DisplayName, name, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
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
            AcdWorkgroup = null;
            ContactList = null;
            PolicySets = new List<PolicySet>();
            configuration = null;
        }

        public Campaign(ININ.IceLib.Configuration.Dialer.CampaignConfiguration ic_campaign)
        {
            id = ic_campaign.ConfigurationId.Id;
            DisplayName = ic_campaign.ConfigurationId.DisplayName;
            if (! string.IsNullOrEmpty(ic_campaign.AcdWorkgroup.Value.Id))
            {
                try
                {
                    AcdWorkgroup = Workgroup.find(ic_campaign.AcdWorkgroup.Value.Id);
                }
                catch(KeyNotFoundException)
                {
                    //TODO: Trace/Warn?
                }
            }
            if (! string.IsNullOrEmpty(ic_campaign.ContactList.Value.Id))
            {
                try
                {
                    ContactList = ContactList.find(ic_campaign.ContactList.Value.Id);
                }
                catch(KeyNotFoundException)
                {
                    //TODO: Trace/Warn?
                }
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
            if (policy_ids.Count() == 0) return;
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

        public void activate_agents(IEnumerable<string> agent_ids)
        {
            if (agent_ids.Count() == 0) return;

            var agent_manager = new AgentManager(Application.ICSession);
            var active_agents = ActiveAgents;
            var campaign_ids  = new Collection<ConfigurationId> { configuration.ConfigurationId };
            var empty_ids     = new Collection<ConfigurationId>();

            // First deactivate agents (all agents that where active and are not in the new list
            var logoff = new Collection<string>(ActiveAgents.Where(x => !agent_ids.Any(y => y == x.id)).Select(agent => agent.id).ToList());

            agent_manager.AllocateAgents(logoff, campaign_ids, empty_ids);

            // Then activate agents
            var logon = new Collection<string>(agent_ids.ToList());

            agent_manager.AllocateAgents(logon, empty_ids, campaign_ids);
        }

        private static List<CampaignConfiguration.Property> DefaultProperties   = new List<CampaignConfiguration.Property> { CampaignConfiguration.Property.AcdWorkgroup, CampaignConfiguration.Property.ContactList, CampaignConfiguration.Property.PolicySets };
        private static List<CampaignConfiguration.Property> MandatoryProperties = new List<CampaignConfiguration.Property> { CampaignConfiguration.Property.Id, CampaignConfiguration.Property.DisplayName };

        private List<Agent> _active_agents = null;
    }
}