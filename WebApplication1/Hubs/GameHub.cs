using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Hubs
{
    public class GameHub : Hub
    {

        private static readonly ConcurrentBag<Game> games = new ConcurrentBag<Game>();
        private static readonly ConcurrentBag<Player> players = new ConcurrentBag<Player>();






        public async Task SendCard(string card, string leftOrRight, string opponentCard)
        {
            await Clients.All.SendAsync("ReceiveCard", card, leftOrRight, opponentCard);
        }


        /*public async Task SendGame() //Test Function
        {
            Player player1 = new Player("Hi");
            Player player2 = new Player("Hello");
            Game game = new Game(player1, player2);
            for(int i = 0; i < game.Player.Deck.Cards.Count; i++) {
                await Clients.All.SendAsync("ReceiveGame", game.Player.Deck.Cards[i].Value + game.Player.Deck.Cards[i].Suit);
            }
            
        }*/
    }
}