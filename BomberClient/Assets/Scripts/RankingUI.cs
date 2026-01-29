using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class RankingUI : MonoBehaviour
{
    public static RankingUI Instance;

    public Transform content;
    public GameObject rowPrefab;
    public GameObject menuButtons;
    public GameObject btnMenu;
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        btnMenu.SetActive(false);
    }

    public void Show(RankPlayer[] players)
    {
        menuButtons.SetActive(false);   // ẨN MENU
        btnMenu.SetActive(true);
        gameObject.SetActive(true);     // HIỆN RANK

        foreach (Transform c in content)
            Destroy(c.gameObject);

        int rank = 1;

        foreach (var p in players)
        {
            var row = Instantiate(rowPrefab, content);

            var texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = rank.ToString();
            texts[1].text = p.nick;
            texts[2].text = p.wins.ToString();

            rank++;
        }
    }

    public void OnClickRank()
    {
        NetTcpClient.Instance.Send(
            JsonConvert.SerializeObject(new
            {
                type = "ranking"
            })
        );
    }
    public void BackMenu()
    {
        gameObject.SetActive(false);
        menuButtons.SetActive(true);
    }

}
