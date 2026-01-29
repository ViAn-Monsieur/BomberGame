using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using UnityEngine;
using System;

public class NetUdpClient : MonoBehaviour
{
    public static NetUdpClient Instance;
    UdpClient udp;
    ConcurrentQueue<string> packets = new ConcurrentQueue<string>();

    bool running;

    public int MyPlayerId;

    void Awake()
    {
        Debug.Log("NetUdpClient Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    public void Connect()
    {
        if (MyPlayerId <= 0)
        {
            Debug.LogError("UDP Connect called before PlayerId assigned!");
            return;
        }

        if (udp != null)
            return;

        udp = new UdpClient();
        udp.Connect("127.0.0.1", 8888);

        var pkt = new UdpBindPacket
        {
            type = "udp_bind",
            playerId = MyPlayerId
        };

        string json = JsonUtility.ToJson(pkt);

        Debug.Log("UDP SEND: " + json);

        Send(json);

        running = true;
        StartReceive();
    }

    async void StartReceive()
    {
        Debug.Log("UDP Receive Loop Started");

        while (running)
        {
            try
            {
                var result = await udp.ReceiveAsync();
                string json = Encoding.UTF8.GetString(result.Buffer);

                packets.Enqueue(json);
            }
            catch
            {
                break;
            }
        }
    }

    void Update()
    {
        while (packets.TryDequeue(out var msg))
        {
            if (PlayerManager.Instance != null)
                PlayerManager.Instance.ApplySnapshot(msg);
        }
    }

    public void Send(string json)
    {
        if (udp == null) return;

        byte[] data = Encoding.UTF8.GetBytes(json);
        udp.Send(data, data.Length);
    }

    public void Reset()
    {
        udp?.Close();
        udp = null;
        running = false;
    }

}

[System.Serializable]
public class UdpBindPacket
{
    public string type;
    public int playerId;
}
