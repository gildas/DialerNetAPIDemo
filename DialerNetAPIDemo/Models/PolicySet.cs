using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class PolicySet
    {
        public string id { get; set; }
        [Display(Name="Policy Set")]
        public string DisplayName { get; set; }

        private ININ.IceLib.Configuration.Dialer.PolicySetConfiguration configuration { get; set; }

        public static ICollection<PolicySet> find_all()
        {
            return Application.PolicySetConfigurations.Select(item => new PolicySet(item)).ToList();
        }

        public static ICollection<PolicySet> find_all_by_id(IEnumerable<string> ids, IEnumerable<PolicySetConfiguration.Property> properties = null)
        {
            return Application.PolicySetConfigurations.Where(item => ids.Contains(item.ConfigurationId.Id)).Select(item => new PolicySet(item)).ToList();
        }

        public static PolicySet find(string id)
        {
            try
            {
                return new PolicySet(Application.PolicySetConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static PolicySet find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public static PolicySet find_by_name(string name)
        {
            try
            {
                return new PolicySet(Application.PolicySetConfigurations.First(item => item.ConfigurationId.DisplayName == name));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, name));
            }
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
    }
}
