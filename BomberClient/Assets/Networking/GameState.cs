using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class GameState
{
    [JsonProperty("RoomId")]
    public int roomId;

    [JsonProperty("MatchId")]
    public int matchId;

    [JsonProperty("Tick")]
    public int tick;

    [JsonProperty("Players")]
    public List<PlayerState> players = new();

    [JsonProperty("Bombs")]
    public List<BombState> bombs = new();

    [JsonProperty("Explosions")]
    public List<ExplosionCellState> Explosions = new();

    [JsonProperty("Bricks")]
    public List<BrickDestroyedState> bricks = new();

    [JsonProperty("Map")]
    public MapData map;
}

[System.Serializable]
public class PlayerState
{
    [JsonProperty("Id")]
    public int id;

    [JsonProperty("NickName")]
    public string nickName;

    [JsonProperty("X")]
    public int x;

    [JsonProperty("Y")]
    public int y;

    [JsonProperty("Alive")]
    public bool alive;
}


[System.Serializable]
public class BombState
{
    [JsonProperty("OwnerId")]
    public int ownerId;

    [JsonProperty("X")]
    public int x;

    [JsonProperty("Y")]
    public int y;

    [JsonProperty("Timer")]
    public float timer;

    [JsonProperty("Power")]
    public int power;
}

[System.Serializable]
public class ExplosionCellState
{
    public int X;
    public int Y;
}
[System.Serializable]
public class BrickDestroyedState
{
    public int X;
    public int Y;
}
