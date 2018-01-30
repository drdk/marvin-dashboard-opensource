using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DR.Common.RESTClient;
using DR.Marvin.Dashboard.Models;

namespace DR.Marvin.Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(string environment = null)
        {
            var node = MvcApplication.Configuration.GetNodeForEnviroment(environment);

            var restClient = new JsonClient(useISODates: true);
            var dashboard = new Models.Dashboard
            {
                Info = restClient.Get<DashboardInfo>($"http://{node.ClusterHost}/api/Status/GetDashboardInfo", useDefaultCredentials: true),
                Environment = node.Name,
                Environments = MvcApplication.Configuration.Nodes.Select(c=>c.Name).ToList()
            };
            ViewBag.EnvironmentApiRoot = new Uri($"http://{node.ClusterHost}");
            ViewBag.OndemandRoot =
                string.IsNullOrEmpty(node.OndemandHost) ? null : new Uri($"http://{node.OndemandHost}");
            return View(dashboard);
        }

        public ActionResult Cancel(string environment, string jobUrn)
        {
            var node = MvcApplication.Configuration.GetNodeForEnviroment(environment);
            var restClient = new JsonClient(useISODates: true);
            var command = new Command()
            {
                Urn = jobUrn,
                Type = CommandType.Cancel,
                Username = User.Identity.Name
            };
            
            var res = restClient.Post($"http://{node.ClusterHost}/api/Job/Command", command);
                
            return RedirectToAction(nameof(Index), new { environment });
        }
    }
}