using AgentHelp;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Sender
{
    class MainTest
    {
        static Socket _client;
        static EndPoint _remoteEndPoint;

        // TO DO
         Message message = new Message();
       
        public MainTest()
        {
            message.Command = "Publish";
            message.Topic = "chisinau";
            message.Text = "This is simple text";


            IPAddress serverIPAddress = IPAddress.Parse("127.0.0.1");
            int serverPort = 34001;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            _client.Connect(_remoteEndPoint);

        }

        public static void Main()
        {

            MainTest mt = new MainTest();

            while (true)
            {
                if (Console.ReadLine() == "p")
                {
                    mt.PostMessage();
                }
                else
                    break;
            }


            Console.ReadKey();
        }

        private void PostMessage()
        {
            message.Topic = message.Topic.Trim();

            if (string.IsNullOrEmpty(message.Topic))
            {
                Console.WriteLine("Please enter a topic name...");
                return;
            }

            SendASingleEvent();
        }

        private void SendASingleEvent()
        {
            string message = this.message.Serialize();
            _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);
          
        }
 
    }
}
