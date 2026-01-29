using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void JoinSolo() => SendJoin("solo2");
    public void JoinFour() => SendJoin("solo4");
    public void JoinTeam() => SendJoin("team2v2");

    void SendJoin(string mode)
    {
        if (NetTcpClient.Instance == null)
        {
            var tcp = FindObjectOfType<NetTcpClient>();

            if (tcp == null)
            {
                tcp = new GameObject("Networking").AddComponent<NetTcpClient>();
            }

            tcp.Connect();
        }

        string json = $"{{\"type\":\"join\",\"mode\":\"{mode}\"}}";
        NetTcpClient.Instance.Send(json);
    }
}
