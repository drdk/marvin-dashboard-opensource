using System;

namespace DR.Marvin.Dashboard.Models
{
    public class HealthCheck
    {
        public string Name { get; set; }
        public bool? Passed { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

}