﻿using ININ.IceLib.Configuration;
using ININ.IceLib.Dialer.Supervisor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class Agent
    {
        public string id { get; set; }
        [Display(Name = "Agent")]
        public string DisplayName { get; set; }
        public IEnumerable<Campaign> LoggedInCampaigns { get; set; }

        private UserConfiguration configuration { get; set; }

        public static Agent find(string id, List<UserConfiguration.Property> properties = null)
        {
            var query = new UserConfigurationList(ConfigurationManager.GetInstance(Application.ICSession));
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(UserConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetRightsFilterToView();
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var results = query.GetConfigurationList();
            query.StopCaching();

            if (results.Count() == 0) throw new KeyNotFoundException(id);
            if (results.Count()  > 1) throw new IndexOutOfRangeException(id);

            return new Agent(results.First());
        }

        public static Agent find(ConfigurationId id, List<UserConfiguration.Property> properties = null)
        {
            return find(id.Id, properties);
        }

        public Agent()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            configuration = null;
        }

        protected Agent(UserConfiguration ic_configuration)
        {
            id = ic_configuration.ConfigurationId.Id;
            DisplayName = ic_configuration.ConfigurationId.DisplayName;
            configuration = ic_configuration;
        }

        private static List<UserConfiguration.Property> DefaultProperties   = new List<UserConfiguration.Property> { UserConfiguration.Property.StatusText };
        private static List<UserConfiguration.Property> MandatoryProperties = new List<UserConfiguration.Property> { UserConfiguration.Property.Id, UserConfiguration.Property.DisplayName };
    }
}
