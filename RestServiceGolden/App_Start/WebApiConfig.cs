using RestServiceGolden.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RestServiceGolden
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            //config.EnableCors();
            // Rutas de API web
            config.EnableCors(new EnableCorsAttribute("http://localhost:4200", "*", "*"));

            // Add handler to deal with preflight requests, this is the important part
            config.MessageHandlers.Add(new PreflightRequestsHandler()); // Defined above

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
