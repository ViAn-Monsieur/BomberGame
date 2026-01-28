using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BomberServer.Models;

namespace BomberServer.Core
{
    public class RoomManager
    {
        private readonly Dictionary<int, Room> _rooms = new();
        private int _nextRoomId = 1;

        // =========================
        // TẠO PHÒNG RIÊNG
        // =========================
        public Room CreateRoom(RoomType type, GameMap map)
        {
            int id = _nextRoomId++;
            var room = new Room(id, type, map);
            _rooms[id] = room;

            Console.WriteLine($"[RoomManager] Created Room #{id} Type={type}");
            return room;
        }

        // =========================
        // GHÉP NGẪU NHIÊN
        // =========================
        public Room JoinRandomRoom(RoomType type, Func<GameMap> mapFactory)
        {
            // 1. tìm phòng chờ
            foreach (var room in _rooms.Values)
            {
                if (room.Type == type &&
                    room.State == RoomState.Waiting &&
                    !room.IsFull)
                {
                    return room; // dùng map của room
                }
            }

            // 2. không có phòng → TẠO PHÒNG MỚI + RANDOM MAP
            var map = mapFactory();
            return CreateRoom(type, map);
        }

        // =========================
        // Tham gia phong bằng ID
        // =========================
        public Room? JoinRoomById(int roomId)
        {
            if (_rooms.TryGetValue(roomId, out var room))
            {
                if (room.State != RoomState.Waiting)
                    throw new Exception("Room already started");
                if (room.IsFull)
                    throw new Exception("Room is full");
                return room;
            }
            return null;
        }

        public Room? GetRoom(int roomId)
            => _rooms.TryGetValue(roomId, out var r) ? r : null;

        public void RemoveRoom(int roomId)
        {
            if (_rooms.Remove(roomId))
                Console.WriteLine($"[RoomManager] Removed Room #{roomId}");
        }

        public IEnumerable<Room> AllRooms => _rooms.Values;

        // =========================
        // THÊM NGƯỜI CHƠI
        // =========================
        public void AddPlayerToRoom(Room room, Player player)
        {
            if (room.IsFull)
                throw new Exception("Room is full");

            player.RoomId = room.RoomId;

            // team logic
            if (room.Type == RoomType.Team2v2)
            {
                var team = room.Teams[1].Count <= room.Teams[2].Count ? 1 : 2;
                room.Teams[team].Add(player.Id);
                player.TeamId = team;
            }

            room.AddPlayer(player);

            Console.WriteLine(
                $"[Room #{room.RoomId}] Player {player.Id} joined (Team={player.TeamId})"
            );
        }

        // =========================
        // RỜI PHÒNG
        // =========================
        public void LeaveRoom(Player player)
        {
            if (!_rooms.TryGetValue(player.RoomId, out var room))
                return;

            room.Players.Remove(player.Id);
            room.RemovePlayer(player.Id);

            if (player.TeamId != 0)
                room.Teams[player.TeamId].Remove(player.Id);

            Console.WriteLine($"Player {player.Id} left Room #{room.RoomId}");

            if (room.Players.Count == 0)
                RemoveRoom(room.RoomId);
        }
    }
}