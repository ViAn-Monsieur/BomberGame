using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberServer.Models;
using Microsoft.Data.Sqlite;

namespace BomberServer
{
    public static class Database
    {
        static string connectionString = "Data Source=bomberserver.db";
        public static SqliteConnection Open()
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }
        public static void Init()
        {
            using var db = Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS users(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL,
                nickname TEXT NOT NULL,
                wins INTEGER DEFAULT 0
                );";
            cmd.ExecuteNonQuery();
        }
        public static User Login(string user, string pass)
        {
            using var db = Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"SELECT id,nickname,wins
                                FROM users
                                WHERE username=@u and password=@p";
            cmd.Parameters.AddWithValue("@u", user);
            cmd.Parameters.AddWithValue("@p", pass);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User(r.GetInt32(0), r.GetString(1), r.GetInt32(2));
        }
        public static bool Register(string user, string pass, string nick)
        {
            var db = Open();
            //check ton tai
            var check = db.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM users WHERE username=@u";
            check.Parameters.AddWithValue("@u", user);

            long exist = (long)check.ExecuteScalar();
            if (exist > 0)
                return false;
            //insert
            var cmd = db.CreateCommand();
            cmd.CommandText = @"INSERT INTO users(username,password,nickname,wins)
              VALUES(@u,@p,@n,0)";
            cmd.Parameters.AddWithValue("@u", user);
            cmd.Parameters.AddWithValue("@p", pass);
            cmd.Parameters.AddWithValue("@n", nick);

            cmd.ExecuteNonQuery();
            return true;
        }
        public static void AddWin(int userId)
        {
            using var db = Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"UPDATE users
                                SET wins = wins + 1
                                WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }
        public static List<(string nick, int wins)> GetTop10()
        {
            var list = new List<(string, int)> ();
            using var db = Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"SELECT nickname, wins
                                FROM users
                                ORDER BY wins DESC
                                LIMIT 10";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add((r.GetString(0), r.GetInt32(1)));
            }
            return list;
        }
    }
}
