﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iSelectManager.Models
{
    public class Agent
    {
        public string id { get; set; }
        [Display(Name = "Agent")]
        public string DisplayName { get; set; }

        public static ICollection<Agent> find_all()
        {
            List<Agent> agents = new List<Agent>();

            return agents;
        }
    }
}