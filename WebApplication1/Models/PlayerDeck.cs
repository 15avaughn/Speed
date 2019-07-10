using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class PlayerDeck
    {
        private List<Card> _cards = new List<Card>();
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }
        public Card Draw()
        {
            return _cards.Pop();
        }
        
    }
}
