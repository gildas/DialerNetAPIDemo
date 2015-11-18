using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.Dialer.Supervisor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

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

        [Display(Name="Skill sets")]
        public ICollection<SkillSet> SkillSets { get; set; }

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

        private CampaignConfiguration configuration { get; set; }

        public static ICollection<Campaign> find_all(IEnumerable<CampaignConfiguration.Property> properties = null)
        {
            return Application.CampaignConfigurations.Select(item => new Campaign(item)).ToList();
        }

        public static Campaign find(string id)
        {
            try
            {
                return new Campaign(Application.CampaignConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static Campaign find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public static Campaign find_by_name(string name)
        {
            try
            {
                return new Campaign(Application.CampaignConfigurations.First(item => item.ConfigurationId.DisplayName == name));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, name));
            }
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

        public Campaign(CampaignConfiguration ic_campaign)
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

            SkillSets = new List<SkillSet>();
            foreach (var ic_skillset in ic_campaign.SkillSets.Value)
            {
                try
                {
                    SkillSets.Add(SkillSet.find(ic_skillset.Id));
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
            if (policy_ids == null || policy_ids.Count() == 0) return;
            configuration.PrepareForEdit();
            configuration.PolicySets.Value.Clear();
            Application.PolicySetConfigurations.Where(item => policy_ids.Contains(item.ConfigurationId.Id)).ForEach(item => configuration.PolicySets.Value.Add(item.ConfigurationId));
            configuration.Commit();
        }

        public void activate_agents(IEnumerable<string> agent_ids)
        {
            if (agent_ids == null || agent_ids.Count() == 0) return;

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

        public void add_skillset(string skillset_id)
        {
            add_skillset(SkillSet.find(id).ConfigurationId);
        }

        public void add_skillset(SkillSet skillset)
        {
            add_skillset(skillset.ConfigurationId);
        }

        public void add_skillset(ConfigurationId skillset_id)
        {
            if (configuration.SkillSets.Value.Any(item => item.Id == skillset_id.Id)) return;
            configuration.PrepareForEdit();
            configuration.SkillSets.Value.Add(skillset_id);
            configuration.Commit();
        }

        private List<Agent> _active_agents = null;
    }
}