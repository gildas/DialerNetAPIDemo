using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class SkillSet
    {
        public string id { get; set; }
        [Display(Name="Skill Set")]
        public string DisplayName { get; set; }
        [Display(Name="Minimum Proficiency")]
        public int MinimumProficiency
        {
            get
            {
                return (configuration == null) ? 1 : configuration.MinimumProficiency.Value;
            }
            set
            {
                if (configuration == null) throw new ININ.IceLib.Configuration.ConfigurationReferenceException();
                configuration.PrepareForEdit();
                configuration.MinimumProficiency.Value = value;
                configuration.Commit();
            }
        }

        public IEnumerable<DialerSkill> Skills { get { return configuration.Skills.Value;  } }

        public ConfigurationId ConfigurationId
        {
            get
            {
                if (configuration == null) throw new ININ.IceLib.Configuration.ConfigurationReferenceException();
                return configuration.ConfigurationId;
            }
        }

        private SkillSetConfiguration configuration { get; set; }

        public static ICollection<SkillSet> find_all()
        {
            return SkillSetConfigurations.Select(item => new SkillSet(item)).ToList();
        }

        public static SkillSet find(string id)
        {
            try
            {
                return new SkillSet(SkillSetConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static SkillSet find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public static SkillSet find_by_name(string name)
        {
            try
            {
                return new SkillSet(SkillSetConfigurations.First(item => item.ConfigurationId.DisplayName == name));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, name));
            }
        }

        public static SkillSet find_or_create(string name, string column_name, int minimum_proficiency = 1)
        {
            if (! SkillSetConfigurations.Any(item => item.ConfigurationId.DisplayName == name))
            {
                try
                {
                    var configurations = new SkillSetConfigurationList(new DialerConfigurationManager(Application.ICSession).ConfigurationManager);
                    var skillset       = configurations.CreateObject();

                    skillset.SetDisplayName(name);
                    skillset.ColumnName.Value         = column_name;
                    skillset.MinimumProficiency.Value = minimum_proficiency;
                    skillset.Commit();
                    _SkillSetConfigurations = null; // So the list is fetched again
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            return new SkillSet(SkillSetConfigurations.First(item => item.ConfigurationId.DisplayName == name));
        }

        public SkillSet()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            configuration = null;
        }

        public SkillSet(SkillSetConfiguration ic_configuration)
        {
            id = ic_configuration.ConfigurationId.Id;
            DisplayName = ic_configuration.ConfigurationId.DisplayName;
            configuration = ic_configuration;
        }

        public void add_skill(string skill_id, int ratio = 1)
        {
            add_skill(Skill.find(skill_id));
        }

        public void add_skill(Skill skill, string database_value = null, int ratio = 1)
        {
            if (configuration.Skills.Value.Any(item => item.Id.Id == skill.id)) return;
            configuration.PrepareForEdit();
            configuration.Skills.Value.Add(new DialerSkill(skill.id) { Value = database_value ?? skill.DisplayName, Ratio = ratio });
            configuration.Commit();
        }

        private static ReadOnlyCollection<SkillSetConfiguration> SkillSetConfigurations
        {
            get
            {
                if (_SkillSetConfigurations == null)
                {
                    try
                    {
                        var configurations = new SkillSetConfigurationList(new DialerConfigurationManager(Application.ICSession).ConfigurationManager);
                        var query_settings = configurations.CreateQuerySettings();

                        query_settings.SetPropertiesToRetrieve(new[] { 
                            SkillSetConfiguration.Property.Id,
                            SkillSetConfiguration.Property.DisplayName,
                            SkillSetConfiguration.Property.RevisionLevel,
                            SkillSetConfiguration.Property.ColumnName,
                            SkillSetConfiguration.Property.MinimumProficiency,
                            SkillSetConfiguration.Property.Skills
                        });
                        configurations.StartCaching(query_settings);
                        _SkillSetConfigurations = configurations.GetConfigurationList();
                    }
                    catch(Exception e)
                    {
                        HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
                        _SkillSetConfigurations = new List<SkillSetConfiguration>().AsReadOnly();
                    }
                }
                return _SkillSetConfigurations;
            }
        }
        private static ReadOnlyCollection<SkillSetConfiguration> _SkillSetConfigurations = null;
    }
}
