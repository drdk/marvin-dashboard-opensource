using System;
using System.Collections.Generic;
using DR.Marvin.Dashboard.Model;

namespace DR.Marvin.Dashboard.Models
{
    public class LogCountModel
    {
        public IList<GraphInfo> Charts;
        public string Environment;
        public IList<string> Environments;

        public class GraphInfo
        {
            public string Id;
            public string Title;
            public string Json;
        }
    }
    
}