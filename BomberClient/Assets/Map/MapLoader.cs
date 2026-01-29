using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MapLoader : MonoBehaviour
{
    public static MapLoader Instance;

    public Transform mapRoot;
    public GameObject wallPrefab;
    public GameObject brickPrefab;
    public GameObject groundPrefab;

    float tileSize = 1f;
    bool loaded = false;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Dictionary<(int, int), Brick> bricks = new();
    public HashSet<(int, int)> walls = new();
    public bool IsReady { get; private set; }

    void Awake()
    {
        Instance = this;

        if (mapRoot == null)
            mapRoot = transform;
    }

    public void Load(string json)
    {
        Debug.Log("MAP JSON RECEIVED:");
        Debug.Log(json);

        bricks.Clear();
        walls.Clear();

        foreach (Transform c in mapRoot)
            Destroy(c.gameObject);

        var packet = JsonConvert.DeserializeObject<MapPacket>(json);
        if (packet == null || packet.map == null || packet.map.Tiles == null)
        {
            Debug.LogError("INVALID MAP");
            return;
        }

        loaded = true;
        if (packet == null || packet.map == null)
        {
            Debug.LogError("INVALID MAP PACKET:\n" + json);
            return;
        }

        MapData map = packet.map;

        Width = map.Width;
        Height = map.Height;

        float ox = (Width - 1) / 2f;
        float oy = (Height - 1) / 2f;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int t = map.Tiles[y][x];

                Vector3 pos = new Vector3(
                    (x - ox) * tileSize,
                    (-y + oy) * tileSize,
                    0
                );

                Instantiate(groundPrefab, pos, Quaternion.identity, mapRoot);

                if (t == 1)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity, mapRoot);
                    walls.Add((x, y));
                }
                else if (t == 2)
                {
                    var go = Instantiate(brickPrefab, pos, Quaternion.identity, mapRoot);
                    var brick = go.GetComponent<Brick>();
                    brick.x = x;
                    brick.y = y;
                    bricks[(x, y)] = brick;
                    BrickManager.Instance.Register(brick);
                }
            }
        }
        IsReady = true;

        mapRoot.localPosition = Vector3.zero;

        FitCamera();
    }

    void FitCamera()
    {
        Camera cam = Camera.main;
        cam.orthographic = true;

        float worldWidth = Width * tileSize;
        float worldHeight = Height * tileSize;

        float sizeY = worldHeight / 2f;
        float sizeX = worldWidth / 2f / cam.aspect;

        cam.orthographicSize = Mathf.Max(sizeX, sizeY);
        cam.transform.position = new Vector3(0, 0, -10);
    }
    public void Clear()
    {
        foreach (Transform c in mapRoot)
            Destroy(c.gameObject);

        bricks.Clear();
        walls.Clear();

        Width = 0;
        Height = 0;
        IsReady = false;

        Debug.Log("Map cleared");
    }

}

[System.Serializable]
public class MapData
{
    public int Width;
    public int Height;
    public int[][] Tiles;
}

[System.Serializable]
public class MapPacket
{
    public string type;
    public MapData map;
}
