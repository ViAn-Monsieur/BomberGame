using System.Net;
using System.Net.Sockets;
using BomberServer.Core;
using System.Collections.Concurrent;

namespace Networking
{
    public class UdpServer
    {
        private readonly UdpClient udp;
        private readonly GameServer gameServer;

        public UdpServer(int port, GameServer server)
        {
            udp = new UdpClient(port);
            gameServer = server;

            // Disable UDP connection reset on Windows (fix 10054)
            udp.Client.IOControl(
                (IOControlCode)0x9800000C,
                new byte[] { 0, 0, 0, 0 },
                null
            );
        }

        public void Start()
        {
            udp.BeginReceive(OnReceive, null);
            Console.WriteLine("UDP Server started");
        }

        void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ep = null!;
                byte[] data = udp.EndReceive(ar, ref ep);

                gameServer.OnUdpPacket(ep, data);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("UDP socket closed: " + ex.SocketErrorCode);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("UDP ERROR:");
                Console.WriteLine(ex);
            }
            finally
            {
                try
                {
                    udp.BeginReceive(OnReceive, null);
                }
                catch { }
            }
        }

        public void Send(IPEndPoint ep, byte[] data)
        {
            udp.Send(data, data.Length, ep);
        }

        public void SendToMany(IEnumerable<IPEndPoint> eps, byte[] data)
        {
            foreach (var ep in eps)
                udp.Send(data, data.Length, ep);
        }
    }
}
