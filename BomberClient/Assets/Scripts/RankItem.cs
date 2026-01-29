using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nicknameText;
    public TMP_Text winText;

    public void Set(int rank, string nick, int wins)
    {
        rankText.text = rank.ToString();
        nicknameText.text = nick;
        winText.text = wins.ToString();
    }
}
