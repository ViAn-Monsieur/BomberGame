using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public class TcpServer
    {
        TcpListener listener;
        public static Dictionary<int, ClientSession> Clients = new();
        static int nextId = 1;

        public TcpServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(OnAccept, null);
            Console.WriteLine("TCP Server started");
        }

        void OnAccept(IAsyncResult ar)
        {
            TcpClient client = listener.EndAcceptTcpClient(ar);
            int id = nextId++;

            ClientSession session = new ClientSession(id, client);
            Clients[id] = session;

            Console.WriteLine($"TCP client connected: {id}");
            listener.BeginAcceptTcpClient(OnAccept, null);
        }
    }
}
