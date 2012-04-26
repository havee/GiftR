using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GiftR.MVCWeb.Filters;

using GiftR.Services;
using GiftR.MVCWeb.Factories;
using GiftR.MVCWeb.Controllers;
using Microsoft.Practices.Unity;

namespace GiftR.MVCWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            ActionLogFilterProvider provider = new ActionLogFilterProvider();
            provider.Add("*", "Index");
            FilterProviders.Providers.Add(provider);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{icode}", // URL with parameters
                new { controller = "Home", action = "Index", icode = "" } // Parameter defaults  
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var container = new UnityContainer();
            container.RegisterType<ISitesService, SitesService>();            
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<ISiteTypesService, SiteTypesService>();
            container.RegisterType<IController, HomeController>("Home");
            container.RegisterType<IController, AccountController>("Account");
            container.RegisterType<IController, SitesController>("Sites");
            
            var factory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(factory);

            log4net.Config.XmlConfigurator.Configure();
        }
    }
}