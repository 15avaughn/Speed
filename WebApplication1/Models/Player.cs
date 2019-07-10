using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Player
    {
        public Player(string name, string hash)
        {
            Name = name;
            Hash = hash;
            Id = Guid.NewGuid().ToString("d");
        }

        public string ConnectionId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Group { get; set; }
        public bool IsPlaying { get; set; }

        public PlayerDeck Deck { get; set; }
        public PlayerHand Hand { get; set; }
    }
}
