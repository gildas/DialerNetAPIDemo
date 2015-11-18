using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace iSelectManager.Models
{
    public class Skill
    {
        public string id { get; set; }
        [Display(Name = "Skill")]
        public string DisplayName { get; set; }

        private SkillConfiguration configuration { get; set; }

        public static ICollection<Skill> find_all(string id)
        {
            return SkillConfigurations.Select(item => new Skill(item)).ToList();
        }

        public static Skill find(string id)
        {
            try
            {
                return new Skill(SkillConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static Skill find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public static Skill find_by_name(string name)
        {
            try
            {
                return new Skill(SkillConfigurations.First(item => item.ConfigurationId.DisplayName == name));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, name));
            }
        }

        public static Skill find_or_create(string name)
        {
            if (! SkillConfigurations.Any(item => item.ConfigurationId.DisplayName == name))
            {
                try
                {
                    var configurations = new SkillConfigurationList(ConfigurationManager.GetInstance(Application.ICSession));
                    var skill          = configurations.CreateObject();

                    skill.SetConfigurationId(name);
                    skill.SetDisplayName(name);
                    skill.Commit();
                    _SkillConfigurations = null; // So the list is fetched again
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            return new Skill(SkillConfigurations.First(item => item.ConfigurationId.DisplayName == name));
        }

        public Skill()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            configuration = null;
        }

        public Skill(SkillConfiguration ic_configuration)
        {
            id = ic_configuration.ConfigurationId.Id;
            DisplayName = ic_configuration.ConfigurationId.DisplayName;
            configuration = ic_configuration;
        }

        private static ReadOnlyCollection<SkillConfiguration> SkillConfigurations
        {
            get
            {
                if (_SkillConfigurations == null)
                {
                    try
                    {
                        var configurations = new SkillConfigurationList(ConfigurationManager.GetInstance(Application.ICSession));
                        var query_settings = configurations.CreateQuerySettings();

                        query_settings.SetPropertiesToRetrieve(new[] { 
                            SkillConfiguration.Property.Id,
                            SkillConfiguration.Property.DisplayName,
                            SkillConfiguration.Property.UserAssignments,
                            SkillConfiguration.Property.WorkgroupAssignments,
                        });
                        configurations.StartCaching(query_settings);
                        _SkillConfigurations = configurations.GetConfigurationList();
                    }
                    catch(Exception e)
                    {
                        HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
                        _SkillConfigurations = new List<SkillConfiguration>().AsReadOnly();
                    }
                }
                return _SkillConfigurations;
            }
        }
        private static ReadOnlyCollection<SkillConfiguration> _SkillConfigurations = null;
    }
}