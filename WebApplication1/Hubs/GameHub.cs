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
       
        private static readonly ConcurrentDictionary<String, Player> players = new ConcurrentDictionary<String, Player>();
        private static readonly ConcurrentDictionary<String, Game> games = new ConcurrentDictionary<String, Game>();

        public override Task OnConnectedAsync()
        {
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            
            var game = games?.Values.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            if (game == null)
            {
                
                var playerWithoutGame = players.Values.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (playerWithoutGame != null)
                {

                    players.TryRemove(playerWithoutGame.ConnectionId, out _);
                }

                return null;
            }

            
            if (game != null)
            {
                games.TryRemove(game.Player1.ConnectionId, out _);
            }

            var player = game.Player1.ConnectionId == Context.ConnectionId ? game.Player1 : game.Player2;

            if (player == null)
            {
                return null;
            }

            players.TryRemove(player.ConnectionId, out _);

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
        
        public void OnRegistrationComplete(string connectionId)
        {
            this.Clients.Client(connectionId).SendAsync("registrationComplete");
        }

        
        public void RegisterPlayer(string name)
        {
            var player = players?.Values.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                player = new Player(name) { ConnectionId = Context.ConnectionId, IsPlaying = false, IsSearchingOpponent = false, RegisterTime = DateTime.UtcNow };
                players.TryAdd(player.ConnectionId, player);
            }
            else
            {
                player.IsPlaying = false;
                player.IsSearchingOpponent = false;
            }

            this.OnRegistrationComplete(Context.ConnectionId);
        }

        public void FindOpponent()
        {
            var player = players.Values.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                Clients.Client(Context.ConnectionId).SendAsync("test");
                return;
            }

            player.IsSearchingOpponent = true;

            var opponent = players.Values.Where(x => x.ConnectionId != Context.ConnectionId && x.IsSearchingOpponent && !x.IsPlaying).OrderBy(x => x.RegisterTime).FirstOrDefault();
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

            games.TryAdd(player.ConnectionId, new Game(player, opponent));
        }

        //All need to be Async
        //Deal Center Stacks
            //only worry about sending to client

        //Change stack once placed (push to CenterStack)
            //calls pair checking
            //do nothing if not a valid pair

        //Check pairs
            //reset button if a button was previously pressed

        //button logic
            // check if both are clicked
            // if both are checked 
                //add center stacks to decks
                // deal stacks again

















        public async Task SendCard(string card, string leftOrRight, string opponentCard) //Also Test Function (for now).
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