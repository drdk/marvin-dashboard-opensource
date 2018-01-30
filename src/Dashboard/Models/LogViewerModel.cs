using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DR.Marvin.Dashboard.Model;

namespace DR.Marvin.Dashboard.Models
{
    public class LogViewerModel
    {
        public int Skip;
        public int Count;
        public LogLevel? LogLevel;
        public IList<LogLevel> ActiveLevels;
        public string Query;
        public IList<LogEntry> Entries;
        public string ApplicationQuery;
        public IList<string> ApplicationNames;
        public string Environment;
        public IList<string> Environments;
        public Uri EnvironmentApiRoot;
    }
}