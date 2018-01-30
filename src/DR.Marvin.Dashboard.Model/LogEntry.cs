using System;

namespace DR.Marvin.Dashboard.Model
{
    public class LogEntry
    {
        public DateTime TimeStamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string MachineName { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string Appilcation { get; set; }
        public string Urn { get; set; }
    }
}