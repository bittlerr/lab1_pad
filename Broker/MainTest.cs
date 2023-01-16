using System;

namespace Broker
{
    class MainTest
    {
        public static void Main()
        {
            DeadLatter _deadLetterChannel = new DeadLatter();
            SubscriberService subscriberService = new SubscriberService(_deadLetterChannel);
            subscriberService.StartSubscriberService();

            PublisherService publisherService = new PublisherService(_deadLetterChannel);
            publisherService.StartPublisherService();

            Console.ReadLine();
        }
    }
}
