using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject hardWallPrefab;
    public GameObject softWallPrefab;

    public Transform mapRoot;

    int width;
    int height;
    int[,] tiles;

    public void Render(int[,] mapTiles)
    {
        tiles = mapTiles;
        height = tiles.GetLength(0);
        width = tiles.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x, -y, 0);

                // luôn có floor
                Instantiate(floorPrefab, pos, Quaternion.identity, mapRoot);

                switch (tiles[y, x])
                {
                    case 1:
                        Instantiate(hardWallPrefab, pos, Quaternion.identity, mapRoot);
                        break;

                    case 2:
                        Instantiate(softWallPrefab, pos, Quaternion.identity, mapRoot);
                        break;
                }
            }
        }
    }
}

