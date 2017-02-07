using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.Models;

namespace SignalRChat.Hubs
{
    public class PumperHub : Hub
    {






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
            Clients.All.addNewMessageToPage(streamInfo, msg);
        }

    }
}