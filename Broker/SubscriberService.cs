using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

namespace Broker
{
    class SubscriberService
    {
        private  DeadLatter _deadLetterChannel;

        public SubscriberService(DeadLatter _deadLetterChannel)
        {
            this._deadLetterChannel = _deadLetterChannel;
        }
        public void StartSubscriberService()
        {
            new Thread(() => {
                HostSubscriberService();
            }).Start();
        }
        private void HostSubscriberService()
        {
            IPAddress ipV4 = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEP = new IPEndPoint(ipV4, 34000);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(localEP);
            
            StartListening(server);

        }
        private void StartListening(Socket server)
        {
            server.Listen(10203);
            while (true)
            {
                Socket client = server.Accept();
                Console.WriteLine("Accept: " + client.RemoteEndPoint);
                new Thread(() => perConnection(client)).Start();
            
            }
        }

        private void perConnection(Socket client)
        {
 
            int recv = 0;
            byte[] data = new byte[1024];


            while (true)
            {
                recv = 0;
                data = new byte[1024];

                try
                {
                    recv = client.Receive(data);
                }
                catch (SocketException e)
                {
                    break;
                }

                string messageSendFromClient = Encoding.ASCII.GetString(data, 0, recv);
                string[] messageParts = messageSendFromClient.Split(",".ToCharArray());

                Console.WriteLine(messageSendFromClient); 

                if (!string.IsNullOrEmpty(messageParts[0]))
                {
                    switch (messageParts[0])
                    {
                        case "Connect":

                            _deadLetterChannel.Check(messageParts[1], client);
                            break;

                        case "Subscribe":

                            if (!string.IsNullOrEmpty(messageParts[1]))
                            {
                                Subscriber subscriber = new Subscriber();
                                subscriber.ID = messageParts[2];
                                subscriber.Client = client;
                                TransientData.AddSubscriber(messageParts[1], subscriber);
                                _deadLetterChannel.Check(messageParts[2], client);
                            }
                            break;

                        case "UnSubscribe":

                            if (!string.IsNullOrEmpty(messageParts[1]))
                            {
                                Subscriber subscriber = new Subscriber();
                                subscriber.ID = messageParts[2];
                                subscriber.Client = client;
                                TransientData.RemoveSubscriber(messageParts[1], subscriber);
                            }
                            break;
                    }
                }

            }
        }

    }
}
