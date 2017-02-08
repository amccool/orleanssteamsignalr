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


        public override bool Equals(object obj)
        {
            WhatsHappeningModel x = (WhatsHappeningModel) obj;

            return (this.StreamId.Equals(x.StreamId) && this.ProviderName.Equals(x.ProviderName) && this.NameSpace.Equals(x.NameSpace));
        }

        public override int GetHashCode()
        {
            return this.StreamId.GetHashCode() ^ this.ProviderName.GetHashCode() ^ this.NameSpace.GetHashCode();
        }
    }
}