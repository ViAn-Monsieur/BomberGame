using TMPro;
using UnityEngine;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public GameObject loginPanel;
    public GameObject registerPanel;

    void Start()
    {
        username.text = "";
        password.text = "";
        NetTcpClient.Instance.Connect();
    }

    public void OnLogin()
    {
        string json =
$@"{{""type"":""login"",""username"":""{username.text}"",""password"":""{password.text}""}}";

        NetTcpClient.Instance.Send(json);
    }

    public void GoRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
