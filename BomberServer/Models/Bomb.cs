using System;

namespace BomberServer.Models
{
    public class Bomb
    {
        public Guid BombId { get; } = Guid.NewGuid();
        public int OwnerId { get; }
        public int X { get; }
        public int Y { get; }
        public int Power { get; }
        public float FuseTime { get; private set; }
        public float ElapsedTime { get; private set; } = 0f;
        public bool IsExploded { get; private set; } = false;

        public Bomb(int ownerId, int x, int y, int power, float fuseTime=2.0f)
        {
            OwnerId = ownerId;
            X = x;
            Y = y;
            Power = power;
            FuseTime = fuseTime;
        }
        public bool Update(float deltaTime)
        {
            if (IsExploded)
                return false;

            ElapsedTime += deltaTime;
            if (ElapsedTime >= FuseTime)
            {
                IsExploded = true;
                return true; //kich no
            }
            return false;
        }
        //thoi gian cho
        public float GetRemainingTime()
        {
            return Math.Max(0f, FuseTime - ElapsedTime);
        }
        public void MarkExploded()
        {
            IsExploded = true;
        }
        public void ForceExplode()
        {
            FuseTime = 0;
        }

    }
}
