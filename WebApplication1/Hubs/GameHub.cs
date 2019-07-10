using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendCard(string card, string leftOrRight, string opponentCard)
        {
            await Clients.All.SendAsync("ReceiveCard", card, leftOrRight, opponentCard);
        }
        public async Task SendGame()
        {
            Player player1 = new Player("Hi", "fadfas");
            Player player2 = new Player("Hello", "test");
            Game game = new Game(player1, player2);
            for(int i = 0; i < game.Deck.Cards.Count; i++) {
                await Clients.All.SendAsync("ReceiveGame", game.Deck.Cards[i].Value + game.Deck.Cards[i].Suit);
            }
            
        }
    }
}