using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Interface;
using Interface.Json;

using Newtonsoft.Json;

namespace ServerTest
{
    class Program
    {
        private static Socket listener;

        static void Main(string[] args)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] ips;
            try
            {
                ips = Dns.GetHostAddresses("localhost");
                listener.Connect(ips, 5555);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error: {0} - {1}", e.ErrorCode, e.Message);
                Console.Write("Press any key...");
                Console.ReadKey(true);
                return;
            }

            // resiver
            Thread th = new Thread(delegate()
                {
                    while (listener.Connected)
                    {
                        try
                        {
                            Int32 e;
                            Int32 s;
                            String j;

                            byte[] buffer = new byte[4];
                            listener.Receive(buffer, 4, SocketFlags.Partial);
                            e = BitConverter.ToInt32(buffer, 0);

                            buffer = new byte[4];
                            listener.Receive(buffer, 4, SocketFlags.Partial);
                            s = BitConverter.ToInt32(buffer, 0);

                            buffer = new byte[s];
                            listener.Receive(buffer, s, SocketFlags.Partial);
                            j = Encoding.UTF8.GetString(buffer);

                            Console.WriteLine("{0}: {1}", Events.EventToString(e), j);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine("Error {0} - {1}", e.ErrorCode, e.Message);
                        }
                    }
                });
            th.Start();

            Console.Write("Input name: ");
            String name = Console.ReadLine();

            JsonBaseObject json = new JsonBaseObject();
            json.String = name;
            String s_json = JsonConvert.SerializeObject(json);
            Console.WriteLine("Json: {0}", s_json);
            Send(Events.SIGN_IN, s_json);

            name = Console.ReadLine();
            listener.Close();
            Console.Write("Press any key...");
            Console.ReadKey(true);
        }

        public static void Send(Int32 e, String j)
        {
            try
            {
                byte[] json = Encoding.UTF8.GetBytes(j);
                byte[] size = BitConverter.GetBytes(json.Length);
                byte[] even = BitConverter.GetBytes(e);

                Stream s = new MemoryStream();
                s.Write(even, 0, even.Length);
                s.Write(size, 0, size.Length);
                s.Write(json, 0, json.Length);

                byte[] data = new byte[s.Length];
                s.Position = 0;
                s.Read(data, 0, data.Length);

                listener.Send(data);
            }
            catch { }
        }
    }
}
