using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using RB.OrleansClient;
using SignalRChat.Models;
using Orleans.Streams;


namespace SignalRChat.Hubs
{
    public class StreamHub : Hub
    {
        private IClientGrainFactory _factory;
        private List<StreamSubscriptionHandle<object>> handles = new List<StreamSubscriptionHandle<object>>();

        public StreamHub(IClientGrainFactory factory) : base()
        {
            this._factory = factory;
        }

        //private Task _me;

        //public StreamHub() : base()
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

        //    var strInfo = new WhatsHappeningModel()
        //    {
        //        StreamId = Guid.Parse("b01efcfa-ee7b-4e72-9f06-edacebd79f8f"),
        //        NameSpace = "1*902",
        //        ProviderName = "NCI-PCC"
        //    };


        //    this._me = Task.Run(async () =>
        //    {


        //        var streamProv = await factory.GetStreamProviderAsync(strInfo.ProviderName);

        //        var stream = streamProv.GetStream<object>(strInfo.StreamId, strInfo.NameSpace);

        //        await stream.SubscribeAsync((o, token) =>
        //            {
        //                dynamic newDynamicObject = o;
        //                //Console.ForegroundColor = ConsoleColor.Yellow;
        //                //Console.Write($"[{DateTime.Now.ToString("yyyy-M-d HH:mm:ss.FF")}]  ");
        //                //Console.ForegroundColor = ConsoleColor.White;

        //                //Console.WriteLine(newDynamicObject.ToString());
        //                //Console.WriteLine(o.ToString());


        //                UpdateData(strInfo, o.ToString());


        //                return TaskDone.Done;
        //            }, exception =>
        //            {

        //                UpdateData(strInfo, exception.ToString());


        //                //Console.WriteLine(exception);
        //                return TaskDone.Done;
        //            },
        //            () => TaskDone.Done);
        //    });

        //}






        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void UpdateData(WhatsHappeningModel streamInfo, string msg)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage($"{streamInfo.StreamId}-{streamInfo.ProviderName}-{streamInfo.NameSpace}", msg);
        }


        public async Task SubscribeToStream(string guid, string provider, string nameSpace)
        {
            WhatsHappeningModel strInfo = new WhatsHappeningModel()
            {
                 StreamId = Guid.Parse(guid),
                 ProviderName = provider,
                 NameSpace = nameSpace
            };
            var streamProv = await this._factory.GetStreamProviderAsync(strInfo.ProviderName);

            var stream = streamProv.GetStream<object>(strInfo.StreamId, strInfo.NameSpace);

            this.handles.Add( await stream.SubscribeAsync((o, token) =>
                {
                    dynamic newDynamicObject = o;
                            //Console.ForegroundColor = ConsoleColor.Yellow;
                            //Console.Write($"[{DateTime.Now.ToString("yyyy-M-d HH:mm:ss.FF")}]  ");
                            //Console.ForegroundColor = ConsoleColor.White;

                            //Console.WriteLine(newDynamicObject.ToString());
                            //Console.WriteLine(o.ToString());


                            UpdateData(strInfo, o.ToString());


                    return TaskDone.Done;
                }, exception =>
                {

                    UpdateData(strInfo, exception.ToString());


                            //Console.WriteLine(exception);
                            return TaskDone.Done;
                },
                () => TaskDone.Done)
                );

        }

    }
}