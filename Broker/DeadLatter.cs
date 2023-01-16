using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Broker
{
    public class DeadLatter
    {
        private Dictionary<string, List<string>> _deadLatterDictonary;

        public DeadLatter()
        {
            _deadLatterDictonary = new Dictionary<string, List<string>>();
        }

        public void Add(string id, string message)
        {
            List<string> messagesOfId = GetValue(id);
            if (messagesOfId != null)
            {
                messagesOfId.Add(message);
            } else
            {
                messagesOfId = new List<string>();
                messagesOfId.Add(message);
                _deadLatterDictonary.Add(id, messagesOfId);
            }
        }

        public void Check(string id, Socket client)
        {
            List<string> messagesOfId = GetValue(id);

            if(messagesOfId != null)
            {
                List<string> succeseful = new List<string>();
                foreach(string message in messagesOfId)
                {
                    try
                    {
                        client.Send(Encoding.ASCII.GetBytes(message));
                        succeseful.Add(message);
                    } catch {  }
                }

                foreach(string succes in succeseful)
                {
                    messagesOfId.Remove(succes);
                }
               
            }
        }
        private List<string> GetValue(string id)
        {
            List<string> messagesOfId;
            _deadLatterDictonary.TryGetValue(id, out messagesOfId);
            return messagesOfId;
        }
    }
}
