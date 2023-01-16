using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Receiver
{
    class MainTest
    {
        Socket _client;
        EndPoint _remoteEndPoint;
        byte[] _data;
        int _recv;
        bool _isReceivingStarted = false;

        string txtTopicName = "Chisinau";

        public MainTest()
        {

            IPAddress serverIPAddress = IPAddress.Parse("127.0.1.1");
            int serverPort = 34000;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);

            if (!_client.Connected)
                _client.Connect(_remoteEndPoint);

            string Command = "Connect";

            string message = Command + "," + "Cleopatra";

            _client.Send(Encoding.ASCII.GetBytes(message));

            if (_isReceivingStarted == false)
            {
                _isReceivingStarted = true;
                _data = new byte[1024];

                new Thread(() => {
                    ReceiveDataFromServer();
                }).Start();
            }


        }

        public static void Main()
        {
            MainTest mt = new MainTest();

            while (true)
            {
                string chooice = Console.ReadLine();

                if (chooice == "s")
                    mt.Subscribe();

                if (chooice == "u")
                    mt.Unsubscribe();

                if (chooice == "q")
                    break;
            }
          

            Console.ReadLine();
        }



        private void Unsubscribe()
        {
            try
            {
                string topicName = txtTopicName.Trim();
                if (string.IsNullOrEmpty(topicName))
                {
                    Console.WriteLine("Please Enter a Topic Name");
                    return;
                }
                string command = "UnSubscribe";

                string message = command + "," + topicName;
                _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);
   
            }
            catch
            { }
            

        }

        void ReceiveDataFromServer()
        {
           
            while (true)
            {
                byte[] local = new byte[1024];
                int received = 0;
                Console.WriteLine("Await");
                try
                {
                    if (!_client.Connected)
                        Console.WriteLine("Not connected!");

                    received = _client.Receive(local);
                    Console.WriteLine("Received");

                    string msg = Encoding.ASCII.GetString(local, 0, received);
                    Console.WriteLine(msg);

                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
                

            }
        }

        private void Subscribe()
        {
            if(!_client.Connected)
                _client.Connect(_remoteEndPoint);

            string topicName = txtTopicName.Trim();
            if (string.IsNullOrEmpty(topicName))
            {
                Console.WriteLine("Please enter a topic name...");
                return;
            }
     
            string Command = "Subscribe";

            string message = Command + "," + topicName + "," + "Maria";
           
            _client.Send(Encoding.ASCII.GetBytes(message));

            Console.WriteLine(message);

            
           
        }

    
    }
}
