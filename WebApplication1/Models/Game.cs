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
            for (int i = 0; i < 20; i++)
            {
                dealingCards.Cards.Add(deck.Cards.Pop());
            }

            return dealingCards;

        }
        public SpeedFlip DealSpeedFlip(Deck deck)
        {
            SpeedFlip dealingCards = new SpeedFlip();
            for (int i = 0; i < 5; i++)
            {
                dealingCards.Cards.Add(deck.Cards.Pop());
            }

            return dealingCards;
        }
        public SpeedPile DealSpeedPile(Deck deck)
        {
            SpeedPile dealingCards = new SpeedPile();
            dealingCards.Cards.Add(deck.Cards.Pop());
            return dealingCards;
        }

        public PlayerHand DealHand(PlayerDeck playerDeck)
        {
            PlayerHand dealingCards = new PlayerHand();
            for (int i = 0; i < 5; i++)
            {
                dealingCards.Cards.Add(playerDeck.Cards.Pop());
            }

            return dealingCards;
        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public Deck Deck { get; set; }
        public SpeedFlip SpeedFlipL { get; set; }
        public SpeedFlip SpeedFlipR { get; set; }
        public SpeedPile SpeedPileL { get; set; }
        public SpeedPile SpeedPileR { get; set; }

        public Game(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
            Deck = new Deck();
            SpeedFlipL = DealSpeedFlip(Deck);
            SpeedFlipR = DealSpeedFlip(Deck);
            Player1.Deck = DealPlayerDeck(Deck);
            Player1.Hand = DealHand(Player1.Deck);
            Player2.Deck = DealPlayerDeck(Deck);
            Player2.Hand = DealHand(Player2.Deck);
            SpeedPileL = DealSpeedPile(Deck);
            SpeedPileR = DealSpeedPile(Deck);
        }

    }
}
