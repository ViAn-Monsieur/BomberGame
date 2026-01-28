using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BomberServer.Core;
using BomberServer.Models;
//using Newtonsoft.Json;

namespace Networking
{
    public class ClientSession
    {
        public int Id { get; }
        GameServer GameServer;
        public TcpClient Tcp { get; }
        NetworkStream stream;
        byte[] buffer = new byte[4096];
        public User? User { get; set; }
        public Player? Player { get; set; }
        public ClientSession(int id, TcpClient tcp, GameServer gameServer)
        {
            Id = id;
            Tcp = tcp;
            stream = Tcp.GetStream();
            GameServer = gameServer;
            BeginReceive();
        }

        private void BeginReceive()
        {
            stream.BeginRead(buffer, 0, buffer.Length, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar)
        {
            int len = 0;
            try { 
                len = stream.EndRead(ar);
                if (len <= 0)
                {
                    Disconnect();
                    return;
                }
            }
            catch
            {
               Disconnect();
                return;
            }
            string msg = Encoding.UTF8.GetString(buffer, 0, len);
            foreach (var line in msg.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                HandlePacket(line);
            }
            BeginReceive();
        }

        private void HandlePacket(string json)
        {
            var packet = JsonSerializer.Deserialize<JsonElement>(json);
            string type = packet.GetProperty("type").GetString() ?? "";

            switch (type)
            {
                case "login":
                    {
                        string username = packet.GetProperty("username").GetString()!;
                        string password = packet.GetProperty("password").GetString()!;

                        var user = BomberServer.Database.Login(username, password);

                        if (user == null)
                        {
                            Send(new { type = "login_failed" });
                            return;
                        }

                        User = user;

                        Player = GameServer.OnClientConnected(this);

                        Send(new
                        {
                            type = "login_success",
                            id = user.Id,
                            nick = user.NickName,
                            wins = user.Wins
                        });

                        Send(new
                        {
                            type = "welcome",
                            playerId = Player.Id
                        });

                        break;
                    }

                case "register":
                    {
                        string username = packet.GetProperty("username").GetString()!;
                        string password = packet.GetProperty("password").GetString()!;
                        string nickname = packet.GetProperty("nickname").GetString()!;

                        bool ok = BomberServer.Database.Register(username, password, nickname);

                        Send(new
                        {
                            type = ok ? "register_success" : "register_failed"
                        });

                        break;
                    }
            }
        }
        private void Disconnect()
        {
            Console.WriteLine($"Client {Id} disconnected");

            if (Player != null)
                GameServer.OnClientDisconnected(this);

            Tcp.Close();
            TcpServer.Clients.Remove(Id);
        }

        public void BindUdp(IPEndPoint ep)
        {
            if (Player == null)
                return;

            if (Player.RemoteEndPoint == null)
            {
                Player.RemoteEndPoint = ep;
                Console.WriteLine($"Client {Id} bind UDP {ep}");
            }
        }

        public void Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            stream.Write(data, 0, data.Length);
        }
        public void Send(object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            Send(json);
        }
    }
}
