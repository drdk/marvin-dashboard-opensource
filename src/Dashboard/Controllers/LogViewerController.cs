using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DR.Marvin.Dashboard.Model;
using DR.Marvin.Dashboard.Models;
using Google.DataTable.Net.Wrapper;

namespace DR.Marvin.Dashboard.Controllers
{
    public class LogViewerController : Controller
    {
        private readonly ILogRepository _logRepository;

        public LogViewerController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        private readonly IList<string> _appNames = new List<string> { null, "Marvin" };
        // GET: LogViewer
        public ActionResult Index(int skip = 0, int count = 100, LogLevel? logLevel = null, string query = null, string applicationQuery = null, string environment = null)
        {
            var node = MvcApplication.Configuration.GetNodeForEnviroment(environment);
            var cred = node.DbCredentials;
            if (string.IsNullOrEmpty(applicationQuery))
                applicationQuery = null;
            if (string.IsNullOrEmpty(query))
                query = null;
            var res = _logRepository.GetLatest(cred, logLevel, skip, count, query, applicationQuery).ToList();
            while (skip >= count && res.Count == 0)
            {
                skip -= count;
                res = _logRepository.GetLatest(cred, logLevel, skip, count, query, applicationQuery).ToList();
            }
            var model = new LogViewerModel
            {
                ApplicationNames = _appNames,
                Count = count,
                Skip = skip,
                Entries = res,
                LogLevel = logLevel,
                ActiveLevels = _logRepository.GetActiveLevels(cred),
                ApplicationQuery = applicationQuery,
                Query = query,
                Environment = environment,
                Environments = MvcApplication.Configuration.Nodes.Select(c => c.Name).ToList(),
                EnvironmentApiRoot = new Uri($"http://{node.ClusterHost}")
            };
            return View(model);
        }

        public ActionResult Count(string environment = null)
        {
            var node = MvcApplication.Configuration.GetNodeForEnviroment(environment);
            var cred = node.DbCredentials;
            var counts = _logRepository.GetCounts(cred);
            var levels = _logRepository.GetActiveLevels(cred);
           
            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            DataTable dt = new DataTable();

            dt.AddColumn(new Column(ColumnType.Datetime, "datetime", "Klokkeslet"));
            foreach (var logLevel in levels)
            {
                dt.AddColumn(new Column(ColumnType.Number, logLevel.ToString()));
            }
            for (var x = 0; x <= 24; x++) //zero fill
            {
                var row = dt.NewRow();
                var h = now.AddHours(-x);
                var time = new Cell(h, $"{h:g}");
                row.AddCell(time);
                foreach (var logLevel in levels)
                {
                    var key = new Tuple<LogLevel, DateTime>(logLevel, h);
                    int count;
                    if (!counts.TryGetValue(key, out count))
                        count = 0;
                    row.AddCell(new Cell(count));
                }
                dt.AddRow(row);
            }
           


            var model = new LogCountModel
            {
                Charts = new List<LogCountModel.GraphInfo>(new []{ new LogCountModel.GraphInfo
                {
                    Id = "logCount",
                    Json = dt.GetJson(),
                    Title = "Antallet log beskeder de sidste 24 timer"
                }}),
                Environment = environment,
                Environments = MvcApplication.Configuration.Nodes.Select(c => c.Name).ToList()
            };
            return View(model);
        }

    }
}