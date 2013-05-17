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
        public Direction Direction { get; set; }

        public String Time { get { return DateTime.ToShortTimeString(); } }

        public Message() { }
    }

    public enum Direction
    {
        Input,
        Output
    }
}
