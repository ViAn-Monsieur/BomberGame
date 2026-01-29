using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetTcpClient : MonoBehaviour
{
    public static NetTcpClient Instance;

    TcpClient client;
    NetworkStream stream;
    bool loggedIn = false;
    bool loadingGame = false;

    byte[] buffer = new byte[8192];

    ConcurrentQueue<string> packets = new ConcurrentQueue<string>();

    void Awake()
    {
        Debug.Log("NetTcpClient Awake");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    void Start()
    {
    }

    bool connected = false;

    public void Connect()
    {
        if (client != null && client.Connected) return;

        connected = true;

        client = new TcpClient();
        client.Connect("127.0.0.1", 7777);

        stream = client.GetStream();

        stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);

        Debug.Log("TCP connected");
    }

    string recvBuffer = "";

    void OnRead(IAsyncResult ar)
    {
        int size = stream.EndRead(ar);
        if (size <= 0) return;

        recvBuffer += Encoding.UTF8.GetString(buffer, 0, size);

        while (recvBuffer.Contains("\n"))
        {
            int idx = recvBuffer.IndexOf("\n");
            string msg = recvBuffer.Substring(0, idx).Trim();
            recvBuffer = recvBuffer.Substring(idx + 1);

            if (!string.IsNullOrEmpty(msg))
            {
                packets.Enqueue(msg);
            }
        }

        stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
    }


    void Update()
    {
        while (packets.TryDequeue(out var msg))
        {
            Debug.Log("TCP RAW: " + msg);

            if (string.IsNullOrEmpty(msg))
                continue;

            var basePkt = JsonConvert.DeserializeObject<BasePacket>(msg);
            if (basePkt?.type == null)
                continue;
            if (basePkt == null || string.IsNullOrEmpty(basePkt.type))
            {
                Debug.LogError("INVALID PACKET: " + msg);
                continue;
            }

            switch (basePkt.type)
            {
                case "login_success":
                    {
                        Debug.Log("LOGIN OK");
                        loggedIn = true;
                        break;
                    }

                case "login_failed":
                    Debug.Log("LOGIN FAIL");
                    break;

                case "welcome":
                    {
                        if (!loggedIn)
                        {
                            Debug.LogError("RECEIVED WELCOME BEFORE LOGIN SUCCESS");
                            break;
                        }
                        var w = JsonConvert.DeserializeObject<WelcomePacket>(msg);

                        Debug.Log("My PlayerId = " + w.playerId);

                        NetUdpClient.Instance.MyPlayerId = w.playerId;
                        NetUdpClient.Instance.Connect();
                        break;
                    }

                case "map":
                    {
                        if (SceneManager.GetActiveScene().name != "BomberGame")
                        {
                            if (!loadingGame)
                            {
                                loadingGame = true;
                                StartCoroutine(LoadBomber());
                            }

                            StartCoroutine(WaitForGameManager(msg));
                            break;
                        }

                        GameManager.Instance?.OnPacket(msg);
                        break;
                    }

                case "menu":
                    {
                        Debug.Log("SHOW MENU");
                        loadingGame = false;
                        SceneManager.LoadScene("Menu");

                        break;
                    }
                case "game_end":
                    {
                        loadingGame = false;
                        var end = JsonConvert.DeserializeObject<GameEndPacket>(msg);
                        NetUdpClient.Instance.Reset();
                        NetUdpClient.Instance.MyPlayerId = 0;
                        GameEndPopup.Instance.Show(end.winner);

                        break;
                    }
                case "ranking":
                    {
                        var packet = JsonConvert.DeserializeObject<RankingPacket>(msg);

                        RankingUI.Instance.Show(packet.players);

                        Debug.Log("Ranking received: " + packet.players.Length);

                        break;
                    }
                default:
                    {
                        Debug.Log("TCP PACKET: " + msg);
                        if (SceneManager.GetActiveScene().name == "BomberGame")
                            GameManager.Instance?.OnPacket(msg);

                        break;
                    }

            }
        }
    }

    IEnumerator WaitForGameManager(string msg)
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        GameManager.Instance.OnPacket(msg);
    }


    IEnumerator LoadBomber()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("BomberGame");

        while (!op.isDone)
            yield return null;
        loadingGame = false;
        Debug.Log("BomberGame fully loaded");
    }


    IEnumerator ReturnMenu()
    {
        yield return null;

        SceneManager.LoadScene("Menu");
    }

    public void Send(string json)
    {
        if (client == null || !client.Connected)
        {
            Connect();
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(json + "\n");
        stream.Write(data, 0, data.Length);
    }

    public void ResetConnection()
    {
        Debug.Log("RESET TCP");

        connected = false;

        packets = new ConcurrentQueue<string>();

        try
        {
            stream?.Close();
            client?.Close();
        }
        catch { }

        stream = null;
        client = null;
    }

}
public class WelcomePacket
{
    public string type;
    public int playerId;
}
public class BasePacket
{
    public string type;
}
public class GameEndPacket
{
    public string type;
    public int winner;
}
public class RankingPacket
{
    public string type;
    public RankPlayer[] players;
}

public class RankPlayer
{
    public string nick;
    public int wins;
}
