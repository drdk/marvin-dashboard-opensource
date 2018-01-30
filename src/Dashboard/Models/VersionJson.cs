using System;

namespace DR.Marvin.Dashboard.Models
{
    public class VersionJson
    {
        public DateTime BuildTime { get; set; }
        public string GitSha { get; set; }
    }
}