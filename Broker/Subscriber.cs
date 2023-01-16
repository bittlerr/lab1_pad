using System.Net.Sockets;

namespace Broker
{
    public class Subscriber
    {
        public string ID { get; set; }
        public Socket Client { get; set; }
    }
}
