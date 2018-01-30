using System;
using System.Collections.Generic;

namespace DR.Marvin.Dashboard.Model
{
    public interface ILogRepository
    {
        IEnumerable<LogEntry> GetLatest(Credentials credentials, LogLevel? logLevel, int skip = 0, int count = 100, string query = null,
            string application = null, int days = 7);

        IList<LogLevel> GetActiveLevels(Credentials credentials, int days = 7);
        IDictionary<Tuple<LogLevel,DateTime>, int> GetCounts(Credentials credentials, int hours = 24);

    }
}
