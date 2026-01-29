using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    // Kéo 4 prefab vào đây trong Inspector
    public List<GameObject> playerPrefabs = new List<GameObject>();

    Dictionary<int, NetPlayer> players = new Dictionary<int, NetPlayer>();

    // pool prefab chưa dùng
    List<GameObject> availablePrefabs = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        availablePrefabs = new List<GameObject>(playerPrefabs);
    }


    public void ApplySnapshot(string json)
    {
        Debug.Log("[PlayerManager] ApplySnapshot called");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("JSON NULL or EMPTY");
            return;
        }

        json = json.Substring(json.IndexOf('{'));

        Debug.Log("[PlayerManager] Raw json: " + json);

        GameState state = JsonConvert.DeserializeObject<GameState>(json);

        if (state == null)
        {
            Debug.LogError("GameState deserialize FAILED");
            return;
        }

        Debug.Log($"GameState OK | players:{state.players?.Count} bombs:{state.bombs?.Count}");

        // ================= MAP =================
        if (state.map != null)
        {
            Debug.Log("Map exists");

            if (MapLoader.Instance == null)
            {
                Debug.LogWarning("MapLoader NOT READY");
                return;
            }

            MapPacket packet = new MapPacket
            {
                type = "map",
                map = state.map
            };

            MapLoader.Instance.Load(JsonConvert.SerializeObject(packet));
        }

        // ================= PLAYERS =================
        if (state.players == null || state.players.Count == 0)
        {
            Debug.Log("Snapshot empty players");
            return;
        }

        foreach (var p in state.players)
        {
            Debug.Log($"Processing player {p.id}");

            if (!players.ContainsKey(p.id))
            {
                Debug.Log($"Spawning player {p.id}");
                Spawn(p);
            }

            if (players.TryGetValue(p.id, out var np))
            {
                if (np == null)
                {
                    Debug.LogError($"NetPlayer {p.id} IS NULL");
                    continue;
                }

                if (p.alive)
                {
                    np.SetPosition(p.x, p.y);
                }
            }
        }

        // ================= BOMBS =================
        if (BombManager.Instance != null && state.bombs != null)
            BombManager.Instance.ApplyBombs(state.bombs);

        // ================= EXPLOSIONS =================
        if (ExplosionManager.Instance != null && state.Explosions != null)
            ExplosionManager.Instance.ApplyExplosions(state.Explosions);

        // ================= BRICKS =================
        if (BrickManager.Instance != null && state.bricks != null)
            BrickManager.Instance.ApplyBricks(state.bricks);

        Debug.Log("[PlayerManager] Snapshot applied");
    }


    void Spawn(PlayerState ps)
    {
        if (MapLoader.Instance == null)
        {
            Debug.LogWarning("Spawn skipped - MapLoader not ready");
            return;
        }

        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("No player prefab left!");
            return;
        }

        Debug.Log($"Spawning Player {ps.id} at ({ps.x}, {ps.y})");

        float ox = (MapLoader.Instance.Width - 1) / 2f;
        float oy = (MapLoader.Instance.Height - 1) / 2f;

        Vector3 pos = new Vector3(
            ps.x - ox,
            oy - ps.y,
            0
        );

        // random prefab chưa dùng
        int idx = Random.Range(0, availablePrefabs.Count);
        GameObject prefab = availablePrefabs[idx];
        availablePrefabs.RemoveAt(idx);

        var go = Instantiate(prefab, pos, Quaternion.identity);

        NetPlayer np = go.GetComponent<NetPlayer>();
        np.Init(ps.id);

        players[ps.id] = np;
    }
}
