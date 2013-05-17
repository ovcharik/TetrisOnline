using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class Message
    {
        public String Data { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
    }
}
