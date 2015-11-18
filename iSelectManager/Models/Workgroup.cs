using ININ.IceLib.Configuration;
using ININ.IceLib.Dialer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

        public static ICollection<Workgroup> find_all(string id)
        {
            return Application.WorkgroupConfigurations.Select(item => new Workgroup(item)).ToList();
        }

        public static Workgroup find(string id)
        {
            try
            {
                return new Workgroup(Application.WorkgroupConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static Workgroup find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public static Workgroup find_by_name(string name)
        {
            try
            {
                return new Workgroup(Application.WorkgroupConfigurations.First(item => item.ConfigurationId.DisplayName == name));
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
    }
}