using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Streams;
using RB.OrleansClient;
using SignalRChat.Models;

namespace SignalRChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static Dictionary<WhatsHappeningModel, StreamSubscriptionHandle<object>> _subscriptions =
            new Dictionary<WhatsHappeningModel, StreamSubscriptionHandle<object>>();



        protected void Application_Start()
        {


            var builder = new ContainerBuilder();

            builder
                .Register(c =>
                {
                    ClientConfiguration clientConfiguration = null;
                    try
                    {
                        var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ClientConfiguration.xml");
                        if (File.Exists(mappedPath))
                        {
                            clientConfiguration = ClientConfiguration.LoadFromFile(mappedPath);

                        }
                        else
                        {
                            clientConfiguration = new ClientConfiguration();
                            clientConfiguration.GatewayProvider = ClientConfiguration.GatewayProviderType.SqlServer;
                            clientConfiguration.DataConnectionString =
                                @"Server=NCI-R5ESQL01.dev-r5ead.net\MSSQLSVR02;Database=orleans;User ID=orleans;password=orleans;";
                            clientConfiguration.DeploymentId = "R5Ent-v1.0";

                            clientConfiguration.AddSimpleMessageStreamProvider("NCI-BRC");
                            clientConfiguration.AddSimpleMessageStreamProvider("NCI-PCC");

                            clientConfiguration.DefaultTraceLevel = Severity.Warning;

                        }
                    }
                    catch
                    {
                    }

                    return new ClientGrainFactory(clientConfiguration);
                })
                .As<IClientGrainFactory>()
                .SingleInstance();




            // You can register controllers all at once using assembly scanning...
            builder.RegisterControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();






            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));







            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        
    }
}
