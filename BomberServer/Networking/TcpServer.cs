using System.Net;
using System.Net.Sockets;
using BomberServer.Core;

namespace Networking
{
    public class TcpServer
    {
        TcpListener listener;
        GameServer gameServer;

        public static Dictionary<int, ClientSession> Clients = new();
        static int nextId = 1;

        public TcpServer(int port, GameServer server)
        {
            listener = new TcpListener(IPAddress.Any, port);
            gameServer = server;
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

            var session = new ClientSession(id, client);
            Clients[id] = session;

            Console.WriteLine($"TCP client connected: {id}");

            // ⭐ GẮN PLAYER THẬT
            gameServer.OnClientConnected(session);

            listener.BeginAcceptTcpClient(OnAccept, null);
        }
    }
}
