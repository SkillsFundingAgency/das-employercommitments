﻿using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.EmployerCommitments.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "commitments/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CatchAll",
                url: "{*path}",
                defaults: new { controller = "Home", action = "CatchAll", path = UrlParameter.Optional }
            );
        }
    }
}
