using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    static class Colors
    {
        static private int i = 0;
        static private String[] _ColorsArray = new String[]
        {
            "#5D8AA8",
            "#E32636",
            "#FFBF00",
            "#9966CC",
            "#8DB600",
            "#3B444B",
            "#007FFF",
            "#3D2B1F",
            "#FE6F5E",
            "#000000",
            "#0095B6",
            "#B5A642",
            "#CC5500",
            "#B8860B",
            "#03C03C",
            "#2F4F4F",
            "#B22222"
        };

        static public String GetColor()
        {
            String c =  _ColorsArray[i];
            i++;
            if (i == _ColorsArray.Length) i = 0;
            return c;
        }
    }
}
