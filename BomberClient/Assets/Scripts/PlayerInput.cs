using System;

[Flags]
public enum PlayerInput
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    PlaceBomb = 1 << 4
}
