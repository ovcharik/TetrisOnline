using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Any, 5555);
            server.Start();
        }
    }
}
