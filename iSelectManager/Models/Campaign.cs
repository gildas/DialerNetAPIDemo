using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSelectManager.Models
{
    public class Campaign
    {
        public string id { get; set; }
        public string DisplayName { get; set; }
        public ContactList ContactList { get; set; }
        public ICollection<PolicySet> PolicySets { get; set; }

        private ININ.IceLib.Configuration.Dialer.CampaignConfiguration CampaignConfiguration { get; set; }

        public static ICollection<Campaign> find_all()
        {
            List<Campaign> campaigns = new List<Campaign>();

            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                var campaign = new Campaign(ic_campaign);
                campaigns.Add(campaign);
            }
            return campaigns;
        }

        public static Campaign find(string p_id)
        {
            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                if (p_id == ic_campaign.ConfigurationId.Id)
                {
                    return new Campaign(ic_campaign);
                }
            }
            return null;
        }

        public static Campaign find_by_name(string p_name)
        {
            foreach (var ic_campaign in Application.CampaignConfigurations)
            {
                if (p_name == ic_campaign.ConfigurationId.DisplayName)
                {
                    return new Campaign(ic_campaign);
                }
            }
            return null;
        }

        public Campaign()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            ContactList = null;
            PolicySets = new List<PolicySet>();
            CampaignConfiguration = null;
        }

        public Campaign(ININ.IceLib.Configuration.Dialer.CampaignConfiguration ic_campaign)
        {
            id = ic_campaign.ConfigurationId.Id;
            DisplayName = ic_campaign.ConfigurationId.DisplayName;
            ContactList = ContactList.find(ic_campaign.ContactList.Value.Id);

            PolicySets = new List<PolicySet>();
            foreach (var ic_policyset in ic_campaign.PolicySets.Value)
            {
                PolicySets.Add(PolicySet.find(ic_policyset.Id));
            }
            CampaignConfiguration = ic_campaign;
        }

        public IEnumerable<Agent> Agents()
        {
            List<Agent> agents = new List<Agent>();

            var acd_workgroup = CampaignConfiguration.AcdWorkgroup;

            var campaign_group = CampaignConfiguration.CampaignGroup;

            return agents;
        }
    }
}