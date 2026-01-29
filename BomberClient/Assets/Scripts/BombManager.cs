using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    public GameObject bombPrefab;
    public AudioClip datBombSound;
    AudioSource audioSource;
    Dictionary<(int, int), GameObject> bombs = new();

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void ApplyBombs(List<BombState> list)
    {
        HashSet<(int, int)> serverBombs = new();

        foreach (var b in list)
        {
            var key = (b.x, b.y);
            serverBombs.Add(key);

            if (!bombs.ContainsKey(key))
                SpawnBomb(b);
        }

        foreach (var k in new List<(int, int)>(bombs.Keys))
        {
            if (!serverBombs.Contains(k))
            {
                Destroy(bombs[k]);
                bombs.Remove(k);
            }
        }
    }

    void SpawnBomb(BombState b)
    {
        float ox = (MapLoader.Instance.Width - 1) / 2f;
        float oy = (MapLoader.Instance.Height - 1) / 2f;

        Vector3 pos = new Vector3(
            b.x - ox,
            oy - b.y,
            0
        );
        audioSource.PlayOneShot(datBombSound);
        var go = Instantiate(bombPrefab, pos, Quaternion.identity);
        bombs[(b.x, b.y)] = go;
    }
    public void Clear()
    {
        bombs.Clear();
    }
}
