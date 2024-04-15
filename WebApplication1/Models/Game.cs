using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Game
    {
        public Deck DealPlayerDeck(Deck deck)
        {
            Deck dealingCards = new Deck();
            for (int i = 0; i < 26; i++)
            {
                dealingCards.Cards.Add(deck.Draw());
            }

            return dealingCards;

        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public Deck Deck { get; set; }

        public Game(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
            Deck = new Deck();

            Player1.Row = new Dictionary<int,CenterStack>{
                {1,new CenterStack()},
                {2,new CenterStack()},
                {3,new CenterStack()},
                {4,new CenterStack()}
            };

            Player2.Row = new Dictionary<int,CenterStack>{
                {1,new CenterStack()},
                {2,new CenterStack()},
                {3,new CenterStack()},
                {4,new CenterStack()}
            };

            Player1.Deck = DealPlayerDeck(Deck);
            Player2.Deck = DealPlayerDeck(Deck);

            Player1.Row[1].Cards.Add(Player1.Deck.Draw());
            Player1.Row[2].Cards.Add(Player1.Deck.Draw());
            Player1.Row[3].Cards.Add(Player1.Deck.Draw());
            Player1.Row[4].Cards.Add(Player1.Deck.Draw());

            Player2.Row[1].Cards.Add(Player2.Deck.Draw());
            Player2.Row[2].Cards.Add(Player2.Deck.Draw());
            Player2.Row[3].Cards.Add(Player2.Deck.Draw());
            Player2.Row[4].Cards.Add(Player2.Deck.Draw());
        }
    }
}
