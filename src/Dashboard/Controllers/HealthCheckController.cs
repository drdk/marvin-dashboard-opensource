using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DR.Marvin.Dashboard.Models;
using DR.Common.RESTClient;
using Version= DR.Marvin.Dashboard.Models.Version;

namespace DR.Marvin.Dashboard.Controllers
{
    public class HealthCheckController : Controller
    { 
        public ActionResult Index()
        {
            return View();
        }

        private static VersionJson GetVersion(IJsonClient restClient, Uri appRoot)
        {
            return restClient.Get<VersionJson>(
                $"{appRoot}__version.js",
                useDefaultCredentials: true);
        }

        private static IEnumerable<HealthCheck> GetChecks(IJsonClient restClient, Uri check)
        {
            return restClient.Get<IEnumerable<HealthCheck>>(check.ToString(),
                useDefaultCredentials: true);
        }

        public ActionResult Ping()
        {
            return new HttpStatusCodeResult(200);
        }

        public ActionResult Check()
        {
            var timer = new Stopwatch();
            timer.Start();

            var check = new Check
            {
                Groups = MvcApplication.Configuration.Groups.Select(cfg => new Group {Name = cfg.Name}).ToList()
            };

            Parallel.ForEach(MvcApplication.Configuration.Groups, cfg =>
            {
                var group = check.Groups.Single(g => g.Name == cfg.Name);
                var nodes = MvcApplication.Configuration.Nodes.First(nc => nc.Name == cfg.NodeConfiguration).Hosts;
                group.Nodes = nodes.Select(host => new Node { Name = host }).ToList();
                
                Parallel.ForEach(nodes, node =>
                {
                    var viewNode = group.Nodes.Single(vn => vn.Name == node);
                    var nodeTime = new Stopwatch();
                    nodeTime.Start();
                    try
                    {
                        var restClient = new JsonClient(useISODates: true);
                        var appUri = new Uri($"http://{node}{cfg.Url}");
                        var jsonVersion = GetVersion(restClient, appUri);
                        viewNode.Version = new Version {BuildTime = jsonVersion.BuildTime, GitHash = jsonVersion.GitSha};
                        if (!string.IsNullOrEmpty(cfg.HealthCheck))
                        {
                            viewNode.CheckUri = new Uri($"{appUri}{cfg.HealthCheck}");
                            var checks = GetChecks(restClient, viewNode.CheckUri);
                            viewNode.CheckPassed = checks.All(c => c.Passed.GetValueOrDefault(true));
                        }
                    }
                    catch (Exception e)
                    {
                        viewNode.CheckPassed = false;
                        viewNode.Exception = e;
                    }
                    nodeTime.Stop();
                    viewNode.Time = nodeTime.Elapsed;
                });

            });
            timer.Stop();
            check.Duration = timer.Elapsed;
            return PartialView(check);
        }
    }
}