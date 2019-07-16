using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Game
    {
        public PlayerDeck DealPlayerDeck(Deck deck)
        {
            PlayerDeck dealingCards = new PlayerDeck();
            for (int i = 0; i < 26; i++)
            {
                dealingCards.Cards.Add(deck.Cards.Pop());
            }

            return dealingCards;

        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public CenterStackRow Player1Row { get; set; }
        public CenterStackRow Player2Row { get; set; }

        public Deck Deck { get; set; }

        public Game(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
            Deck = new Deck();

            Player1Row = new CenterStackRow();
            Player2Row = new CenterStackRow();

            Player1.Deck = DealPlayerDeck(Deck);
            Player2.Deck = DealPlayerDeck(Deck);

            Player1Row.CenterStack1.Cards.Add(Player1.Deck.Draw());
            Player1Row.CenterStack2.Cards.Add(Player1.Deck.Draw());
            Player1Row.CenterStack3.Cards.Add(Player1.Deck.Draw());
            Player1Row.CenterStack4.Cards.Add(Player1.Deck.Draw());

            Player2Row.CenterStack1.Cards.Add(Player2.Deck.Draw());
            Player2Row.CenterStack2.Cards.Add(Player2.Deck.Draw());
            Player2Row.CenterStack3.Cards.Add(Player2.Deck.Draw());
            Player2Row.CenterStack4.Cards.Add(Player2.Deck.Draw());
        }

    }
}
