using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public static BrickManager Instance;

    Dictionary<Vector2Int, Brick> bricks = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(Brick b)
    {
        var key = new Vector2Int(b.x, b.y);

        if (!bricks.ContainsKey(key))
            bricks[key] = b;
    }

    public void ApplyBricks(List<BrickDestroyedState> list)
    {
        foreach (var s in list)
        {
            var key = new Vector2Int(s.X, s.Y);

            if (!bricks.TryGetValue(key, out var brick))
            {
                Debug.Log($"Brick NOT FOUND at {key}");
                continue;
            }

            brick.DestroyBrick();
            bricks.Remove(key);
        }
    }
    public void Clear()
    {
        bricks.Clear();
    }
}
