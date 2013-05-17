using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        private String _Color;
        public String Color
        {
            get { return _Color; }
        }

        public User()
        {
            _Color = Colors.GetColor();
        }
    }
}
