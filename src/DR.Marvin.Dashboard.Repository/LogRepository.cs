using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DR.Marvin.Dashboard.Model;
using AutoMapper;

namespace DR.Marvin.Dashboard.Repository
{
    public class LogRepository : ILogRepository
    {

        private static Entities GetDb(Credentials credentials)
        {
            return new Entities($"metadata=res://*/MarvinEntities.csdl|res://*/MarvinEntities.ssdl|res://*/MarvinEntities.msl;provider=System.Data.SqlClient;provider connection string=\"data source={credentials.Hostname};initial catalog={credentials.Catalog};persist security info=True;user id={credentials.User};password={credentials.Password};MultipleActiveResultSets=True;App=EntityFramework\"");
        }
        public IList<LogLevel> GetActiveLevels(Credentials credentials, int days = 7)
        {
            return new List<LogLevel>
            {
                LogLevel.DEBUG, LogLevel.INFO, LogLevel.WARN, LogLevel.ERROR, LogLevel.FATAL
            };
            /* READ ACTUAL VALUES FROM DB , SLOWER . LEAVE HARDCODED FOR NOW.
            var fromDate = DateTime.UtcNow.AddDays(-days);
            using (var db= GetDb(credentials))
            {
                var levels=  db.log.Where(a => a.Date > fromDate)
                    .Select(a => a.Level)
                    .Distinct().ToList();
                return levels.Select(l => (LogLevel) Enum.Parse(typeof(LogLevel), l)).OrderByDescending(l=>(int)l).ToList();
            }*/
        }

        public IDictionary<Tuple<LogLevel, DateTime>, int> GetCounts(Credentials credentials, int hours = 24)
        {
            var limit = DateTime.UtcNow.AddHours(-hours);
            using (var db = GetDb(credentials))
            {
                var raw =  (from l in db.log
                    where l.Date > limit
                    group l by  new { l.Level, l.Date.Year, l.Date.Month, l.Date.Day, l.Date.Hour }
                    into g
                    select new { 
                     g.Key,
                     Count = g.Count()}).ToList();

                return raw.ToDictionary(x =>
                    new Tuple<LogLevel, DateTime>((LogLevel) Enum.Parse(typeof(LogLevel), x.Key.Level),
                        new DateTime(x.Key.Year, x.Key.Month, x.Key.Day, x.Key.Hour, 0, 0)), x => x.Count);
                
            }
        }

        public IEnumerable<LogEntry> GetLatest(
            Credentials credentials, 
            LogLevel? logLevel, 
            int skip = 0, 
            int count = 100, 
            string query = null, 
            string application = null,
            int days = 7)
        {
            using (var db = GetDb(credentials))
            {
                var fromDate = DateTime.UtcNow.AddDays(-days);

                var includeLevels =
                    logLevel.HasValue ?
                    ((LogLevel[]) Enum.GetValues(typeof(LogLevel)))
                    .Where(LevelFilter(logLevel.Value))
                    .Select(l => l.ToString()) : 
                    new string[] {};

                var logitems =
                    db.log.Where(a => a.Date > fromDate
                    && (!logLevel.HasValue                || includeLevels.Contains(a.Level))
                    && (string.IsNullOrEmpty(query)       || a.Message.Contains(query))
                    && (string.IsNullOrEmpty(application) || a.Logger == application))
                    .OrderByDescending(a => a.Date)
                    .Skip(skip)
                    .Take(count)
                    .ToList();
                return logitems.Select(Mapper.Map<LogEntry>);
            }
        }

        // prevent implicit closure capture
        private static Func<LogLevel, bool> LevelFilter(LogLevel maxLevel)
        {
            return level => level <= maxLevel;
        }
    }
}
