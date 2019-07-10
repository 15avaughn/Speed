using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Deck
    {
        private List<Card> _cards = new List<Card>();
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }

        public Deck()
        {
            for (int i = 1; i <= 13; i++)
            {
                _cards.Add(new Card()
                {
                    Value = i,
                    Suit = "Diamonds"
                });
                _cards.Add(new Card()
                {
                    Value = i,
                    Suit = "Hearts"
                });
                _cards.Add(new Card()
                {
                    Value = i,
                    Suit = "Clubs"
                });
                _cards.Add(new Card()
                {
                    Value = i,
                    Suit = "Spades"
                });
            }
            _cards.Shuffle();
        }
    }
        
}
