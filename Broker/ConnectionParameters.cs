using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Broker
{
    public class ConnectionParameters
    {
        public string Id { get; set; }
        public Socket Server { get; set; }
        public string Message { get; set; }
        public List<Subscriber> SubscriberListForThisTopic { get; set; }

    }
}
