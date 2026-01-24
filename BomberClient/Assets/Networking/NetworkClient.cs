using UnityEngine;
using System.Net;

public class NetworkClient : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int tcpPort = 7777;
    public int udpPort = 8888;

    TcpClientNet tcp;
    UdpClientNet udp;

    void Start()
    {
        tcp = new TcpClientNet();
        udp = new UdpClientNet();

        tcp.Connect(serverIP, tcpPort);
        udp.Connect(serverIP, udpPort);
    }

    void Update()
    {
        SendMoveInput();
    }

    void OnApplicationQuit()
    {
        tcp?.Close();
        udp?.Close();
    }

    void SendMoveInput()
    {
        int dx = 0;
        int dy = 0;

        if (Input.GetKey(KeyCode.W)) dy = -1;
        if (Input.GetKey(KeyCode.S)) dy = 1;
        if (Input.GetKey(KeyCode.A)) dx = -1;
        if (Input.GetKey(KeyCode.D)) dx = 1;

        if (dx == 0 && dy == 0)
            return;

        udp.SendMove(dx, dy);
    }
}
