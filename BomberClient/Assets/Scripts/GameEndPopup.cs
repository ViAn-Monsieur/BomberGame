using UnityEngine;
using TMPro;

public class GameEndPopup : MonoBehaviour
{
    public static GameEndPopup Instance;

    public GameObject panel;
    public TMP_Text winnerText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(int winner)
    {
        panel.SetActive(true);
        winnerText.text = $"🏆 Player {winner} WIN!";
        Time.timeScale = 0f; 
    }

    public void OnOK()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
        NetTcpClient.Instance.Send("{\"type\":\"menu\"}");
    }
}
