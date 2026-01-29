using TMPro;
using UnityEngine;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField nickname;
    public GameObject loginPanel;
    public GameObject registerPanel;

    public void OnRegister()
    {
        string json =
$@"{{""type"":""register"",""username"":""{username.text}"",""password"":""{password.text}"",""nickname"":""{nickname.text}""}}";

        NetTcpClient.Instance.Send(json);
    }

    public void BackLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }
}
