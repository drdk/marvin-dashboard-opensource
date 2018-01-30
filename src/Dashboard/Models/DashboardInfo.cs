using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DR.Marvin.Dashboard.Models
{
    public class Dashboard
    {
        public DashboardInfo Info { get; set; }
        public string Environment { get; set; }
        public IList<string> Environments { get; set; }
        public DateTime TimeStamp => DateTime.UtcNow;
    }
    public class DashboardInfo
    {
        public IList<DashboardJob> WaitingJobs { get; set; }
        public IList<DashboardJob> ActiveJobs { get; set; }
        public IList<DashboardJob> RecentlyDoneJobs { get; set; }
        public IList<DashboardJob> RecentlyFailedJobs { get; set; }
        private IList<DashboardJob> _recentlyCanceledJobs; // TODO : temp workaround, remove after dashboardinfo has been updated in production
        [Newtonsoft.Json.JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public IList<DashboardJob> RecentlyCanceledJobs
        {
            get
            {
                return _recentlyCanceledJobs ;
            }
            set
            {
                _recentlyCanceledJobs = value ?? new List<DashboardJob>();
            }
        }

        public IList<DashbaordPlugin> Plugins { get; set; }
    }


    public class DashboardJob 
    {
        public string Urn { get; set; }
        public string SourceUrn { get; set; }
        public string Name { get; set; }
        public string CurrentPluginUrn { get; set; }
        public DateTime? EstimatedDone { get; set; }
        public double PercentDone { get; set; }
        public List<TaskProgress> TaskPercentDone { get; set; }
        public string State { get; set; }
        public DateTime? Started { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? EndTime { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int Duration { get; set; }
        public string SourceFormat { get; set; }
        public string DestionationFormat { get; set; }
    }

    public enum Priority
    {
        low = 0,
        medium = 5,
        high = 10
    }

    public class DashbaordPlugin
    {
        public string Urn { get; set; }
        public string PluginType { get; set; }
    }

    public class TaskProgress
    {
        public int PercentDone { get; set; }
        public int PercentOfTotal { get; set; }
        public string Name { get; set; }
    }
}