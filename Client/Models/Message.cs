using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Message
    {
        public String Data { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public MessageType Type { get; set; }

        public String Time { get { return DateTime.ToShortTimeString(); } }

        public Message() { }

        public enum MessageType
        {
            Input,
            Output,
            Status
        }
    }
}
