using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BomberServer.Models
{
    public static class GameMapLoader
    {
        public static GameMap LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            JObject data = JObject.Parse(json);

            int width = data["width"]!.Value<int>();
            int height = data["height"]!.Value<int>();

            var map = new GameMap(width, height);

            JArray tiles = (JArray)data["tiles"]!;

            for (int y = 0; y < height; y++)
            {
                JArray row = (JArray)tiles[y];
                for (int x = 0; x < width; x++)
                {
                    int tile = row[x]!.Value<int>();
                    map.SetTile(x, y, (TileType)tile);
                }
            }

            return map;
        }
    }
}
