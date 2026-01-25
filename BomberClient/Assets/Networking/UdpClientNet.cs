using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UdpClientNet
{
    private UdpClient udp;
    private IPEndPoint serverEP;

    public void Connect(string ip, int port)
    {
        udp = new UdpClient();
        serverEP = new IPEndPoint(IPAddress.Parse(ip), port);

        SendHello();
        Debug.Log("UDP connected");
    }

    private void SendHello()
    {
        udp.Send(new byte[] { 1 }, 1, serverEP);
    }

    public void SendMove(int dx, int dy)
    {
        byte[] data = new byte[]
        {
            2,
            (byte)dx,
            (byte)dy
        };

        udp.Send(data, data.Length, serverEP);
    }

    public void Close()
    {
        udp.Close();
    }
}
