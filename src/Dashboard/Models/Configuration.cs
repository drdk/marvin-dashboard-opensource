using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DR.Marvin.Dashboard.Model;

namespace DR.Marvin.Dashboard.Models
{
    public class Configuration
    {
        public NodesConfiguration[] Nodes { get; set; }
        public GroupConfiguration[] Groups { get; set; }

        public NodesConfiguration GetNodeForEnviroment(string environment)
        {
            return string.IsNullOrEmpty(environment)
                ? Nodes.First()
                : Nodes.Single(c => c.Name == environment);
        }
    }

    public class NodesConfiguration
    {
        public string Name { get; set; }
        public string[] Hosts { get; set; }
        public string ClusterHost { get; set; }
        public string TeamCityStatusIcon { get; set; }
        public string TeamCityUri { get; set; }
        public string OndemandHost { get; set; }
        public Credentials DbCredentials { get; set; }
    }
 
    public class GroupConfiguration
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string HealthCheck { get; set; }
        public string NodeConfiguration { get; set; }
        
    }
}