using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.Models;

namespace SignalRChat.Hubs
{
    public class StreamHub : Hub
    {
        public void UpdateData(WhatsHappeningModel streamInfo, string msg)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(streamInfo, msg);
        }

    }
}