using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using Microsoft.Owin;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using RB.OrleansClient;
using SignalRChat.Hubs;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        //public void ConfigurationCCCC(IAppBuilder app)
        //{
        //    GlobalHost.DependencyResolver.Register(
        //            typeof(StreamHub),
        //            () => new StreamHub(MakeFactory()));

        //    app.MapSignalR();
        //}


        private IClientGrainFactory MakeFactory()
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
        }



        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR();


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







            // STANDARD SIGNALR SETUP:

            // Get your HubConfiguration. In OWIN, you'll create one
            // rather than using GlobalHost.
            var config = new HubConfiguration();
            config.EnableDetailedErrors = true;
            //config.EnableJavaScriptProxies = false;

            // Register your SignalR hubs.
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.Resolver = new AutofacDependencyResolver(container);

            // OWIN SIGNALR SETUP:

            // Register the Autofac middleware FIRST, then the standard SignalR middleware.
            app.UseAutofacMiddleware(container);
            app.MapSignalR("/signalr", config);

            // To add custom HubPipeline modules, you have to get the HubPipeline
            // from the dependency resolver, for example:
            //var hubPipeline = config.Resolver.Resolve<IHubPipeline>();
            //hubPipeline.AddModule(new MyPipelineModule());
        }
    }
}