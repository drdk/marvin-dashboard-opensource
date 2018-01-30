//#define uselocal
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DR.Marvin.Dashboard.App_Start;
using DR.Marvin.Dashboard.Models;
using Newtonsoft.Json;
using StructureMap;

namespace DR.Marvin.Dashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static Configuration Configuration { get; private set; }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutomapperInitializer.Initialize();

#if uselocal
            var configurationPath = Server.MapPath("~/configuration.local.json");
#else
            var configurationPath = Server.MapPath("~/configuration.json");
#endif
            using (var reader = new StreamReader(configurationPath))
                Configuration = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());
        }
    }
}
