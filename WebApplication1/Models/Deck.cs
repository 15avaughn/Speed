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

        static readonly Random Random = new Random();

        public void Shuffle()
        {
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                Card value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
        }

        public Card Draw()
        {
            Card drawnCard = _cards[0];
            _cards.RemoveAt(0);
            return drawnCard;
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
            Shuffle();
        }
    }
        
}
