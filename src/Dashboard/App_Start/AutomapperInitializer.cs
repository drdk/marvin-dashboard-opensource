using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;

namespace DR.Marvin.Dashboard.App_Start
{
    public class AutomapperInitializer
    {
        public static void Initialize()
        {
            //Get all Profiles
            var profiles = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            from type in assembly.GetTypes()
                            where !Regex.IsMatch(assembly.FullName, @"^(?:System\.|Microsoft\.|AutoMapper,)")
                                && typeof(Profile).IsAssignableFrom(type)
                            select type).ToList();
            Mapper.Initialize(cfg =>
            {
                cfg.AllowNullCollections = true;
                foreach (var profile in profiles)
                {
                    cfg.AddProfile((Profile)Activator.CreateInstance(profile));
                }
            });
            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}
