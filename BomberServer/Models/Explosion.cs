using System;
namespace BomberServer.Models
{
    public class Explosion
    {
        public Guid ExplosionId { get; } = Guid.NewGuid();
        public List<(int X, int Y)> Cells { get; } = new ();
        public float Lifetime { get; } = 1.0f;
        public float ElapsedTime { get; private set; } = 0f;
        public bool IsExpired => ElapsedTime >= Lifetime;
        public Explosion(IEnumerable<(int X, int Y)> cells)
        {
            Cells.AddRange(cells);
        }
        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
        }
        /// <summary>
        /// Tạo danh sách ô nổ theo bomb power.
        /// Wall chặn luôn, Brick chặn và bị phá.
        /// </summary>
        public static Explosion Create(GameMap gameMap, int centerX, int centerY, int power, out List<(int x, int y)> bricksDestroyed)
        { 
            bricksDestroyed = new List<(int x, int y)>();

            var cells = new HashSet<(int x, int y)> { (centerX, centerY) };

            //no theo bon huong
            var dirs = new (int dx, int dy)[]
            {
                (1, 0), // right
                (-1, 0), // left
                (0, 1), // down
                (0, -1) // up
            };
            foreach (var (dx, dy) in dirs)
            {
                for (int i = 1; i <= power; i++)
                {
                    int nx = centerX + dx * i;
                    int ny = centerY + dy * i;
                    if (!gameMap.IsInside(nx, ny))
                    {
                        break; // ra ngoai map
                    }
                    var tile = gameMap.GetTile(nx, ny);

                    //wall: chan, khong them
                    if (tile == TileType.Wall)
                    {
                        break;
                    }
                    //them o no
                    cells.Add((nx, ny));

                    //brick: pha va chan
                    if (tile == TileType.Brick)
                    {
                        bricksDestroyed.Add((nx, ny));
                        gameMap.DestroyBrick(nx, ny);
                        break;
                    }
                }
            }
            return new Explosion(cells.ToList());
        }
    }
}