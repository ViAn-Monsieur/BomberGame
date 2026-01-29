namespace BomberServer.Core
{
    public enum RoomType
    {
        Solo2,   // 1v1
        Solo4,   // 4 người FFA
        Team2v2  // 2 đội, mỗi đội 2 người
    }

    public enum RoomState
    {
        Waiting,
        Playing,
        Finished
    }
}