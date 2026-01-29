using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MapLoader map;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {
        map = FindObjectOfType<MapLoader>();

        if (map == null)
        {
            Debug.LogError("MapLoader NOT FOUND in scene!");
            return;
        }

        Debug.Log("Connecting TCP...");

        // CHỈ TCP
        NetTcpClient.Instance.Connect();
    }

    // chỉ nhận GAME STATE
    public void OnPacket(string json)
    {
        Debug.Log("GameManager OnPacket: " + json.Substring(0, Mathf.Min(50, json.Length)) + "...");
        PlayerManager.Instance.ApplySnapshot(json);
    }
}
