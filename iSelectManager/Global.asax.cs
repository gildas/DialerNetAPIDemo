using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ININ.IceLib.Connection;
using System.Web.Configuration;

namespace iSelectManager
{
    public class Application : System.Web.HttpApplication
    {
        public Session Session { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                SessionSettings session_settings = new SessionSettings();
                HostSettings host_settings = new HostSettings(new HostEndpoint(WebConfigurationManager.AppSettings["ICServer"]));
                ICAuthSettings auth_settings = new ICAuthSettings(WebConfigurationManager.AppSettings["ICUser"], WebConfigurationManager.AppSettings["ICPassword"]);

                Session = new Session();
                session_settings.ApplicationName = "iSelectManager";
                Session.Connect(session_settings, host_settings, auth_settings, new StationlessSettings());
            }
            catch (Exception e)
            {
                HttpContext.Current.Trace.Warn("CIC", "Unable to connect", e);
            }
        }

        protected void Application_End()
        {
            if (Session != null && Session.ConnectionState == ConnectionState.Up)
            {
                Session.Disconnect();
            }
        }
    }
}
