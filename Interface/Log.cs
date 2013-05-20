using System;
using System.IO;
using System.Text;

namespace Interface
{
    class Log
    {
        public static void Send(Stream stream, Int32 evnt, String msg)
        {
            Write("->", stream, evnt, msg);
        }

        public static void Recieved(Stream stream, Int32 evnt, String msg)
        {
            Write("<-", stream, evnt, msg);
        }

        static void Write(String dir, Stream stream, Int32 evnt, String msg)
        {
            if (stream != null && stream.CanWrite)
            {
                String s = String.Format("{0} | {1} | {2} | {3}\r\n", dir, DateTime.Now.ToString("s"), Events.EventToString(evnt), msg);
                byte[] d = Encoding.UTF8.GetBytes(s);

                try
                {
                    stream.Write(d, 0, d.Length);
                    stream.Flush();
                }
                catch { }
            }
        }
    }
}
