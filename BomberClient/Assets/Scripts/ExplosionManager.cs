using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance;

    public GameObject explosionPrefab;
    public AudioClip noBoomSound;
    AudioSource audio;
    HashSet<Vector2Int> active = new();

    void Awake()
    {
        Instance = this;
        audio = GetComponent<AudioSource>();
    }

    public void ApplyExplosions(List<ExplosionCellState> list)
    {
        if (list == null || list.Count == 0)
            return;
        foreach (var e in list)
        {
            var key = new Vector2Int(e.X, e.Y);

            // đã spawn rồi → bỏ
            if (active.Contains(key))
                continue;

            Spawn(e, list);
            active.Add(key);
        }
    }

    void Spawn(ExplosionCellState e, List<ExplosionCellState> list)
    {
        Vector3 pos = GridToWorld(e.X, e.Y);

        bool L = list.Exists(c => c.X == e.X - 1 && c.Y == e.Y);
        bool R = list.Exists(c => c.X == e.X + 1 && c.Y == e.Y);
        bool U = list.Exists(c => c.X == e.X && c.Y == e.Y - 1);
        bool D = list.Exists(c => c.X == e.X && c.Y == e.Y + 1);

        int links = (L ? 1 : 0) + (R ? 1 : 0) + (U ? 1 : 0) + (D ? 1 : 0);

        string anim = "ExplosionMiddle";
        Quaternion rot = Quaternion.identity;

        // center
        if ((L && R) || (U && D))
            anim = "ExplosionStart";

        // end
        else if (links == 1)
        {
            anim = "ExplosionEnd";

            if (R) rot = Quaternion.Euler(0, 0, 180);
            else if (L) rot = Quaternion.identity;
            else if (U) rot = Quaternion.Euler(0, 0, -90);
            else if (D) rot = Quaternion.Euler(0, 0, 90);
        }

        var go = Instantiate(explosionPrefab, pos, rot);
        go.GetComponent<Animator>().Play(anim);
        if (audio && noBoomSound)
            audio.PlayOneShot(noBoomSound);

        float explosionLife = 0.4f;

        Destroy(go, explosionLife);
        StartCoroutine(RemoveKey(e.X, e.Y, explosionLife));

    }

    System.Collections.IEnumerator RemoveKey(int x, int y, float t)
    {
        yield return new WaitForSeconds(t);
        active.Remove(new Vector2Int(x, y));
    }

    Vector3 GridToWorld(int x, int y)
    {
        float ox = (MapLoader.Instance.Width - 1) / 2f;
        float oy = (MapLoader.Instance.Height - 1) / 2f;
        return new Vector3(x - ox, oy - y, 0);
    }
}
