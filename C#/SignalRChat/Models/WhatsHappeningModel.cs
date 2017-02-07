using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class WhatsHappeningModel
    {
        public Guid StreamId { get; set; }
        public string ProviderName { get; set; }
        public string NameSpace { get; set; }
    }
}