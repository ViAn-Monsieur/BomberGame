using System.Net.Sockets;
using UnityEngine;

public class InputSender : MonoBehaviour
{
    public int playerId = 1;
    static PlayerInput last = PlayerInput.None;
    void Start()
    {
        Debug.Log("InputSender ACTIVE");
    }

    void Update()
    {
        PlayerInput input = PlayerInput.None;

        if (Input.GetKey(KeyCode.W)) input |= PlayerInput.Up;
        if (Input.GetKey(KeyCode.S)) input |= PlayerInput.Down;
        if (Input.GetKey(KeyCode.A)) input |= PlayerInput.Left;
        if (Input.GetKey(KeyCode.D)) input |= PlayerInput.Right;

        // movement
        if (input != last)
        {
            last = input;

            SendInput(input);
        }

        // bomb riêng
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendInput(PlayerInput.PlaceBomb);
        }
    }

    void SendInput(PlayerInput input)
    {
        var pkt = new InputPacket
        {
            playerId = NetUdpClient.Instance.MyPlayerId,
            input = (int)input
        };

        string json = JsonUtility.ToJson(pkt);

        Debug.Log("SEND INPUT " + json);
        NetUdpClient.Instance.Send(json);
    }

}

[System.Serializable]
public class InputPacket
{
    public string type = "input";
    public int playerId;
    public int input;
}
