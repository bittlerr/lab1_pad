using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Broker
{
    class TransientData
    {
        static Dictionary<string, List<Subscriber>> _subscribersList = new Dictionary<string, List<Subscriber>>();

        static public Dictionary<string, List<Subscriber>> SubscribersList
        {
            get
            {
                lock (typeof(TransientData))
                {
                    return _subscribersList;
                }
            }

        }

        static public List<Subscriber> GetSubscribers(string topicName)
        {
            lock (typeof(TransientData))
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    return SubscribersList[topicName];
                }
                else
                    return null;
            }
        }

        static public void AddSubscriber(string topicName, Subscriber subscriberEndPoint)
        {
            lock (typeof(TransientData))
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (!SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Add(subscriberEndPoint);
                    }
                }
                else
                {
                    List<Subscriber> newSubscribersList = new List<Subscriber>();
                    newSubscribersList.Add(subscriberEndPoint);
                    SubscribersList.Add(topicName, newSubscribersList);
                }
            }

        }

        static public void RemoveSubscriber(string topicName, Subscriber subscriberEndPoint)
        {
            lock (typeof(TransientData))
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Remove(subscriberEndPoint);
                    }
                }
            }
        }

    }
}
