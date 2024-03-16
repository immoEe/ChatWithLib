using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryCustomClient
{
    public enum MessageType
    {
        ServiceMessage,
        RegularMessage,
        LoginMessage
    }
    public class MessageProtocol
    {
        public MessageType MessageType { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
    }
}
