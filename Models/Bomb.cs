namespace BomberServer.Models
{
    public class Bomb
    {
        public int OwnerId { get; }
        public int X { get; }
        public int Y { get; }
        public int Range { get; }

        public void Update(float deltaTime) { }
        public bool ShouldExplode() { return false; }
    }
}
