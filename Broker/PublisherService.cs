using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System;

namespace Broker
{
    class PublisherService
    {
        private DeadLatter _deadLetterChannel;

        public PublisherService(DeadLatter _deadLetterChannel)
        {
            this._deadLetterChannel = _deadLetterChannel;
        }

        public void StartPublisherService()
        {
            new Thread(() => {
                HostPublisherService();
            }).Start();
        }

        private void HostPublisherService()
        {
            IPAddress ipV4 = IPAddress.Parse("127.0.0.1");       
            IPEndPoint localEP = new IPEndPoint(ipV4, 34001);
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
                new Thread(() => perConnection(client)).Start();              
            }
                     
        }

        private void perConnection(Socket client)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int recv = 0;
            byte[] data = new byte[1024];
            while (true)
            {
                try
                {
                    recv = 0;
                    data = new byte[1024];
                    recv = client.Receive(data);

                    string messageSendFromClient = Encoding.ASCII.GetString(data, 0, recv);
                    string command = Regex.Match(messageSendFromClient, "<Command>(.*?)</Command>").Groups[1].Value;
                    string topicName = Regex.Match(messageSendFromClient, "<Topic>(.*?)</Topic>").Groups[1].Value;
                    string text = Regex.Match(messageSendFromClient, "<Text>(.*?)</Text>").Groups[1].Value;

                    Console.WriteLine(messageSendFromClient);
                    Console.WriteLine("Regex: " + command);

                    if (!string.IsNullOrEmpty(command))
                    {
                        if (command == "Publish")
                        {
                            if (!string.IsNullOrEmpty(topicName))
                            {

                                string message = "Topic: " + topicName + "\nText: " + text;

                                List<Subscriber> subscriberListForThisTopic = TransientData.GetSubscribers(topicName);

                                ConnectionParameters connectionParameters = new ConnectionParameters();
                                connectionParameters.Server = client;
                                connectionParameters.Message = message;
                                connectionParameters.SubscriberListForThisTopic = subscriberListForThisTopic;

                                Publish(connectionParameters);

                            }

                        }
                    }
                }
                catch
                { }

            }
        }

        public void Publish(ConnectionParameters connParam)
        {
            ConnectionParameters connectionParameters = connParam;
            Socket server = connectionParameters.Server;
            string message = connectionParameters.Message;
            List<Subscriber> subscriberListForThisTopic = connectionParameters.SubscriberListForThisTopic;

            int messagelength = message.Length;
            Console.WriteLine("Try publish");
      
            if (subscriberListForThisTopic != null)
            {
                foreach (Subscriber sub in subscriberListForThisTopic)
                {
                    Console.WriteLine("Try to publish to " + sub.Client.RemoteEndPoint);
                    try
                    {
                       sub.Client.Send(Encoding.ASCII.GetBytes(message));
                    }
                    catch
                    {
                        Console.WriteLine("Add to Dead letter: " + sub.ID);
                        _deadLetterChannel.Add(sub.ID, message);
                    }
         
                    
                }
            }
        }


    }  
}
