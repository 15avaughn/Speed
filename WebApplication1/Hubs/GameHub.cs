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

        private static readonly ConcurrentBag<Player> players = new ConcurrentBag<Player>();
        private static readonly ConcurrentBag<Game> games = new ConcurrentBag<Game>();

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            
            var game = games?.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            if (game == null)
            {
                
                var playerWithoutGame = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (playerWithoutGame != null)
                {
                    
                    Remove<Player>(players, playerWithoutGame);
                }

                return null;
            }

            
            if (game != null)
            {
                Remove<Game>(games, game);
            }

            var player = game.Player1.ConnectionId == Context.ConnectionId ? game.Player1 : game.Player2;

            if (player == null)
            {
                return null;
            }

            Remove<Player>(players, player);

            if (player.Opponent != null)
            {
                return OnOpponentDisconnected(player.Opponent.ConnectionId, player.Name);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task OnOpponentDisconnected(string connectionId, string playerName)
        {
            return Clients.Client(connectionId).SendAsync("opponentDisconnected", playerName);
        }
        
        public void OnRegisterationComplete(string connectionId)
        {
            //// Notify this connection id that the registration is complete.
            this.Clients.Client(connectionId).SendAsync("registrationComplete");
        }

        
        public void RegisterPlayer(string name)
        {
            var player = players?.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                player = new Player(name) { ConnectionId = Context.ConnectionId, IsPlaying = false, IsSearchingOpponent = false, RegisterTime = DateTime.UtcNow };
                if (!players.Any(j => j.Name == name))
                {
                    players.Add(player);
                }
            }
            else
            {
                player.IsPlaying = false;
                player.IsSearchingOpponent = false;
            }

            this.OnRegisterationComplete(Context.ConnectionId);
        }

        public void FindOpponent()
        {
            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                return;
            }

            player.IsSearchingOpponent = true;

            var opponent = players.Where(x => x.ConnectionId != Context.ConnectionId && x.IsSearchingOpponent && !x.IsPlaying).OrderBy(x => x.RegisterTime).FirstOrDefault();
            if (opponent == null)
            {
                Clients.Client(Context.ConnectionId).SendAsync("opponentNotFound");
                return;
            }

            player.IsPlaying = true;
            player.IsSearchingOpponent = false;

            opponent.IsPlaying = true;
            opponent.IsSearchingOpponent = false;

            player.Opponent = opponent;
            opponent.Opponent = player;

            Clients.Client(Context.ConnectionId).SendAsync("opponentFound", opponent.Name);
            Clients.Client(opponent.ConnectionId).SendAsync("opponentFound", player.Name);

            games.Add(new Game(player, opponent));
        }

        private void Remove<T>(ConcurrentBag<T> players, T playerWithoutGame)
        {
            players = new ConcurrentBag<T>(players?.Except(new[] { playerWithoutGame }));
        }
















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