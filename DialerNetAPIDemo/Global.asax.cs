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
using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.People;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DialerNetAPIDemo
{
    public class Application : System.Web.HttpApplication
    {
        public static Session ICSession { get; private set; }
        public static DialerConfigurationManager DialerConfiguration { get; private set; }
        public static ReadOnlyCollection<CampaignConfiguration>  CampaignConfigurations { get; private set; }
        public static ReadOnlyCollection<WorkgroupConfiguration> WorkgroupConfigurations { get; private set; }
        public static ReadOnlyCollection<ContactListConfiguration> ContactListConfigurations { get; private set; }
        public static ReadOnlyCollection<PolicySetConfiguration> PolicySetConfigurations { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            if (ICSession != null && ICSession.ConnectionState == ConnectionState.Up)
            {
                ICSession.Disconnect();
            }
        }

        public bool LoggedIn { get; private set; }

        public string Username
        {
            get
            {
                return LoggedIn ? ICSession.DisplayName : string.Empty;
            }
        }

        public void Login()
        {
            if (LoggedIn) return;
            try
            {
                var session_settings = new SessionSettings();
                var host_settings = new HostSettings(new HostEndpoint(WebConfigurationManager.AppSettings["ICServer"]));
                var auth_settings = new ICAuthSettings(WebConfigurationManager.AppSettings["ICUser"], WebConfigurationManager.AppSettings["ICPassword"]);

                ICSession = new Session();
                session_settings.ApplicationName = "DialerNetAPIDemo";

                ICSession.ConnectionStateChanged += ICSession_ConnectionStateChanged;
                ICSession.Connect(session_settings, host_settings, auth_settings, new StationlessSettings());

                DialerConfiguration = new DialerConfigurationManager(ICSession);

                InitializeCampaigns(ICSession);
                InitializeWorkgroups(ICSession);
                InitializeContactLists(ICSession);
                InitializePolicySets(ICSession);
            }
            catch (Exception e)
            {
                HttpContext.Current.Trace.Warn("CIC", "Unable to connect", e);
            }
        }

        void ICSession_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            LoggedIn = args.State == ConnectionState.Up;
        }

        private void InitializeCampaigns(ININ.IceLib.Connection.Session session)
        {
            try
            {
                var configurations = new CampaignConfigurationList(new DialerConfigurationManager(session).ConfigurationManager);
                var query_settings = configurations.CreateQuerySettings();

                query_settings.SetPropertiesToRetrieve(new[] { 
                    CampaignConfiguration.Property.Id,
                    CampaignConfiguration.Property.DisplayName,
                    CampaignConfiguration.Property.RevisionLevel,
                    CampaignConfiguration.Property.AcdWorkgroup,
                    CampaignConfiguration.Property.ContactList,
                    CampaignConfiguration.Property.PolicySets,
                    CampaignConfiguration.Property.SkillSets
                });
                configurations.ConfigurationObjectsAdded   += configurations_ConfigurationObjectsAdded;
                configurations.ConfigurationObjectsRemoved += configurations_ConfigurationObjectsRemoved;
                configurations.StartCaching(query_settings);
                lock (updating)
                {
                    CampaignConfigurations = configurations.GetConfigurationList();
                }
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        void configurations_ConfigurationObjectsAdded(object sender, ConfigurationWatchEventArgs<CampaignConfiguration> args)
        {
            try
            {
                lock (updating)
                {
                    CampaignConfigurations = (new ReadOnlyCollectionBuilder<CampaignConfiguration>(CampaignConfigurations.ToList().Concat(args.ObjectsAffected.ToList()))).ToReadOnlyCollection();
                }
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        void configurations_ConfigurationObjectsRemoved(object sender, ConfigurationWatchEventArgs<CampaignConfiguration> args)
        {
            try
            {
                lock (updating)
                {
                    CampaignConfigurations = (new ReadOnlyCollectionBuilder<CampaignConfiguration>(CampaignConfigurations.ToList().RemoveAll(item => args.ObjectsAffected.Contains(item)))).ToReadOnlyCollection();
                }
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        private void InitializeWorkgroups(ININ.IceLib.Connection.Session session)
        {
            try
            {
                var configurations = new WorkgroupConfigurationList(new DialerConfigurationManager(session).ConfigurationManager);
                var query_settings = configurations.CreateQuerySettings();

                query_settings.SetPropertiesToRetrieve(new[] {
                    WorkgroupConfiguration.Property.Id,
                    WorkgroupConfiguration.Property.DisplayName,
                    WorkgroupConfiguration.Property.Members
                });
                configurations.StartCaching(query_settings);
                WorkgroupConfigurations = configurations.GetConfigurationList();
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        private void InitializeContactLists(ININ.IceLib.Connection.Session session)
        {
            try
            {
                var configurations = new ContactListConfigurationList(new DialerConfigurationManager(session).ConfigurationManager);
                var query_settings = configurations.CreateQuerySettings();

                query_settings.SetPropertiesToRetrieve(new[] { 
                    ContactListConfiguration.Property.Id,
                    ContactListConfiguration.Property.DisplayName,
                    ContactListConfiguration.Property.RevisionLevel,
                    ContactListConfiguration.Property.Connection,
                    ContactListConfiguration.Property.TableName,
                    ContactListConfiguration.Property.ContactColumns
                });
                configurations.StartCaching(query_settings);
                //configurations.StartCachingAsync(query_settings, OnCampaignCached, this);
                ContactListConfigurations = configurations.GetConfigurationList();
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        private void InitializePolicySets(ININ.IceLib.Connection.Session session)
        {
            try
            {
                var configurations = new PolicySetConfigurationList(new DialerConfigurationManager(session).ConfigurationManager);
                var query_settings = configurations.CreateQuerySettings();

                query_settings.SetPropertiesToRetrieve(new[] { 
                    PolicySetConfiguration.Property.Id,
                    PolicySetConfiguration.Property.DisplayName,
                    PolicySetConfiguration.Property.RevisionLevel
                });
                configurations.StartCaching(query_settings);
                //configurations.StartCachingAsync(query_settings, OnCampaignCached, this);
                PolicySetConfigurations = configurations.GetConfigurationList();
            }
            catch(Exception e)
            {
                HttpContext.Current.Trace.Warn("Dialer", "Unable to retrieve campaigns", e);
            }
        }

        private object updating = new object();
    }
}
