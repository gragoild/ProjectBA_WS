﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace JSONapi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/FestivalData",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}