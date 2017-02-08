using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using RB.OrleansClient;
using SignalRChat.Hubs;
using SignalRChat.Models;
using Orleans.Streams;

namespace SignalRChat.Controllers
{
    public class StreamController : Controller
    {
        private IClientGrainFactory _factory;

        //public StreamController(IClientGrainFactory factory)
        //public StreamController()
        //{
        //    var config = new ClientConfiguration();
        //    config.GatewayProvider = ClientConfiguration.GatewayProviderType.SqlServer;
        //    config.DataConnectionString =
        //        @"Server=NCI-R5ESQL01.dev-r5ead.net\MSSQLSVR02;Database=orleans;User ID=orleans;password=orleans;";
        //    config.DeploymentId = "R5Ent-v1.0";

        //    config.AddSimpleMessageStreamProvider("NCI-BRC");
        //    config.AddSimpleMessageStreamProvider("NCI-PCC");

        //    config.DefaultTraceLevel = Severity.Warning;


        //    IClientGrainFactory factory = new ClientGrainFactory(config);

        //    this._factory = factory;
        //}

        public StreamController(IClientGrainFactory factory)
        {
            this._factory = factory;
        }


        // GET: Stream
        public async Task<ActionResult> Index()
        {

            var factory = await this._factory.GetGrainFactoryAsync();
            var management = factory.GetGrain<IManagementGrain>(0);

            var grainStats = await management.GetSimpleGrainStatistics();

            var details =
                await
                    management.GetDetailedGrainStatistics(new string[] { "Orleans.Streams.PubSubRendezvousGrain" });

            var sd = details.Select(o =>
            {
                string keyext;
                Guid facilityId = o.GrainIdentity.GetPrimaryKey(out keyext);
                var streamNamespace = keyext.Split('_');
                return new WhatsHappeningModel()
                {
                    NameSpace = streamNamespace[1],
                    ProviderName = streamNamespace[0],
                    StreamId = facilityId
                };
            });
            return View(sd);
        }

        // GET: Stream/Details/5
        public async Task<ActionResult> Details(WhatsHappeningModel strInfo)
        {
            if (MvcApplication._subscriptions.ContainsKey(strInfo))
            {
                //uh we have it?
            }
            else
            {

                var streamProv = await this._factory.GetStreamProviderAsync(strInfo.ProviderName);

                var stream = streamProv.GetStream<object>(strInfo.StreamId, strInfo.NameSpace);

                var streamHandle = await stream.SubscribeAsync((o, token) =>
                    {
                        dynamic newDynamicObject = o;
                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.Write($"[{DateTime.Now.ToString("yyyy-M-d HH:mm:ss.FF")}]  ");
                        //Console.ForegroundColor = ConsoleColor.White;

                        //Console.WriteLine(newDynamicObject.ToString());
                        //Console.WriteLine(o.ToString());
            var context = GlobalHost.ConnectionManager.GetHubContext<StreamHub>();


                        context.Clients.All.UpdateData(strInfo, o.ToString());


                        return TaskDone.Done;
                    }, exception =>
                    {
            var context = GlobalHost.ConnectionManager.GetHubContext<StreamHub>();

                        context.Clients.All.UpdateData(strInfo, exception.ToString());


                        //Console.WriteLine(exception);
                        return TaskDone.Done;
                    },
                    () => TaskDone.Done);

                MvcApplication._subscriptions.Add(strInfo, streamHandle);
            }




            return View(strInfo);
        }




    }
}
