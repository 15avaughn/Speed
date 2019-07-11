using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Player
    {
        public Player(string name)
        {
            Name = name;
        }

        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public bool IsPlaying { get; set; }

        public Player Opponent { get; set; }

        public DateTime RegisterTime { get; set; }

        public bool IsSearchingOpponent { get; set; }

        public PlayerDeck Deck { get; set; }
        public PlayerHand Hand { get; set; }
    }
}
