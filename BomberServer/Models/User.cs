using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string NickName { get; set; } = "";
        public int Wins { get; set; } = 0;

        public User()
        {
        }
        public User(int Id, string NickName, int Wins)
        {
            this.Id = Id;
            this.NickName = NickName;
            this.Wins = Wins;
        }
    }
}
