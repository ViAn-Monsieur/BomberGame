using System.Net.Sockets;
using UnityEngine;

public class TcpClientNet
{
    private TcpClient client = new TcpClient();

    public void Connect(string ip, int port)
    {
        client.Connect(ip, port);
        Debug.Log("TCP connected");
    }

    public void Close()
    {
        client.Close();
    }
}
