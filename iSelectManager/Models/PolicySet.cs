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

        public static ICollection<PolicySet> find_all()
        {
            List<PolicySet> policysets = new List<PolicySet>();

            foreach (var ic_policyset in Application.PolicySetConfigurations)
            {
                policysets.Add(new PolicySet(ic_policyset));
            }
            return policysets;
        }

        public static PolicySet find(string p_id)
        {
            foreach (var ic_policyset in Application.PolicySetConfigurations)
            {
                if (p_id == ic_policyset.ConfigurationId.Id)
                {
                    return new PolicySet(ic_policyset);
                }
            }
            return null;
        }

        public static PolicySet find_by_name(string p_name)
        {
            foreach (var ic_policyset in Application.PolicySetConfigurations)
            {
                if (p_name == ic_policyset.ConfigurationId.DisplayName)
                {
                    return new PolicySet(ic_policyset);
                }
            }
            return null;
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