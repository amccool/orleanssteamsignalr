﻿using System.IO;
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

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR();


            var builder = new ContainerBuilder();






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