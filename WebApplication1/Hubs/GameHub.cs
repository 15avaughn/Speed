﻿using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Hubs
{
    public class GameHub : Hub
    {
       
        private static readonly ConcurrentDictionary<String, Player> players = new ConcurrentDictionary<String, Player>();
        private static readonly ConcurrentDictionary<String, Game> games = new ConcurrentDictionary<String, Game>();

        #region Connection
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

            var game = new Game(player, opponent);

            games.TryAdd(player.ConnectionId, game);
            CheckPairs(game);
            var player1Stack1 = game.Player1.Row[1].Cards.Last().Value + game.Player1.Row[1].Cards.Last().Suit;
            var player1Stack2 = game.Player1.Row[2].Cards.Last().Value + game.Player1.Row[2].Cards.Last().Suit;
            var player1Stack3 = game.Player1.Row[3].Cards.Last().Value + game.Player1.Row[3].Cards.Last().Suit;
            var player1Stack4 = game.Player1.Row[4].Cards.Last().Value + game.Player1.Row[4].Cards.Last().Suit;
            var player2Stack1 = game.Player2.Row[1].Cards.Last().Value + game.Player2.Row[1].Cards.Last().Suit;
            var player2Stack2 = game.Player2.Row[2].Cards.Last().Value + game.Player2.Row[2].Cards.Last().Suit;
            var player2Stack3 = game.Player2.Row[3].Cards.Last().Value + game.Player2.Row[3].Cards.Last().Suit;
            var player2Stack4 = game.Player2.Row[4].Cards.Last().Value + game.Player2.Row[4].Cards.Last().Suit;
            Clients.Client(game.Player1.ConnectionId).SendAsync("playerNumber", "player1");
            Clients.Client(game.Player2.ConnectionId).SendAsync("playerNumber", "player2");
            Clients.Client(game.Player1.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
            Clients.Client(game.Player2.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
            Clients.Client(game.Player1.ConnectionId).SendAsync("cardCount", game.Player1.Deck.Cards.Count);
            Clients.Client(game.Player2.ConnectionId).SendAsync("cardCount", game.Player2.Deck.Cards.Count);
        }
        #endregion

        #region Game
        
        ///<summary>Moves dropped card to the stack if top card is a match</summary><param name="centerStack">Stack to drop to</param>
        public async Task SendCard(string centerStack) 
        {
          
            var game = games?.Values.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            string sentCard = "";
            bool receive = false;
            if(Context.ConnectionId == game.Player1.ConnectionId)
            {
                
                switch (centerStack)
                {
                    case "player2Stack1":
                        if (game.Player2.Row[1].CanBePlacedOn){
                            game.Player2.Row[1].CanBePlacedOn = false;
                            game.Player2.Row[1].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player2.Row[1].Cards.Last().Value + game.Player2.Row[1].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack2":
                        if (game.Player2.Row[2].CanBePlacedOn){
                            game.Player2.Row[2].CanBePlacedOn = false;
                            game.Player2.Row[2].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player2.Row[2].Cards.Last().Value + game.Player2.Row[2].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack3":
                        if (game.Player2.Row[3].CanBePlacedOn){
                            game.Player2.Row[3].CanBePlacedOn = false;
                            game.Player2.Row[3].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player2.Row[3].Cards.Last().Value + game.Player2.Row[3].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack4":
                        if (game.Player2.Row[4].CanBePlacedOn){
                            game.Player2.Row[4].CanBePlacedOn = false;
                            game.Player2.Row[4].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player2.Row[4].Cards.Last().Value + game.Player2.Row[4].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack1":
                        if (game.Player1.Row[1].CanBePlacedOn){
                            game.Player1.Row[1].CanBePlacedOn = false;
                            game.Player1.Row[1].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player1.Row[1].Cards.Last().Value + game.Player1.Row[1].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack2":
                        if (game.Player1.Row[2].CanBePlacedOn){
                            game.Player1.Row[2].CanBePlacedOn = false;
                            game.Player1.Row[2].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player1.Row[2].Cards.Last().Value + game.Player1.Row[2].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack3":
                        if (game.Player1.Row[3].CanBePlacedOn){
                            game.Player1.Row[3].CanBePlacedOn = false;
                            game.Player1.Row[3].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player1.Row[3].Cards.Last().Value + game.Player1.Row[3].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack4":
                        if (game.Player1.Row[4].CanBePlacedOn){
                            game.Player1.Row[4].CanBePlacedOn = false;
                            game.Player1.Row[4].Cards.Add(game.Player1.Deck.Draw());
                            sentCard = game.Player1.Row[4].Cards.Last().Value + game.Player1.Row[4].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    default:
                        break;
                }
                await Clients.Client(game.Player1.ConnectionId).SendAsync("cardCount", game.Player1.Deck.Cards.Count);
            }
            else if(Context.ConnectionId == game.Player2.ConnectionId)
            {
                
                switch (centerStack)
                {
                    case "player2Stack1":
                        if (game.Player2.Row[1].CanBePlacedOn)
                        {
                            game.Player2.Row[1].CanBePlacedOn = false;
                            game.Player2.Row[1].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player2.Row[1].Cards.Last().Value + game.Player2.Row[1].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack2":
                        if (game.Player2.Row[2].CanBePlacedOn)
                        {
                            game.Player2.Row[2].CanBePlacedOn = false;
                            game.Player2.Row[2].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player2.Row[2].Cards.Last().Value + game.Player2.Row[2].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack3":
                        if (game.Player2.Row[3].CanBePlacedOn)
                        {
                            game.Player2.Row[3].CanBePlacedOn = false;
                            game.Player2.Row[3].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player2.Row[3].Cards.Last().Value + game.Player2.Row[3].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player2Stack4":
                        if (game.Player2.Row[4].CanBePlacedOn)
                        {
                            game.Player2.Row[4].CanBePlacedOn = false;
                            game.Player2.Row[4].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player2.Row[4].Cards.Last().Value + game.Player2.Row[4].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack1":
                        if (game.Player1.Row[1].CanBePlacedOn)
                        {
                            game.Player1.Row[1].CanBePlacedOn = false;
                            game.Player1.Row[1].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player1.Row[1].Cards.Last().Value + game.Player1.Row[1].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack2":
                        if (game.Player1.Row[2].CanBePlacedOn)
                        {
                            game.Player1.Row[2].CanBePlacedOn = false;
                            game.Player1.Row[2].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player1.Row[2].Cards.Last().Value + game.Player1.Row[2].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack3":
                        if (game.Player1.Row[3].CanBePlacedOn)
                        {
                            game.Player1.Row[3].CanBePlacedOn = false;
                            game.Player1.Row[3].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player1.Row[3].Cards.Last().Value + game.Player1.Row[3].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    case "player1Stack4":
                        if (game.Player1.Row[4].CanBePlacedOn)
                        {
                            game.Player1.Row[4].CanBePlacedOn = false;
                            game.Player1.Row[4].Cards.Add(game.Player2.Deck.Draw());
                            sentCard = game.Player1.Row[4].Cards.Last().Value + game.Player1.Row[4].Cards.Last().Suit;
                            receive = true;
                        }
                        break;
                    default:
                        break;
                }
                await Clients.Client(game.Player2.ConnectionId).SendAsync("cardCount", game.Player2.Deck.Cards.Count);
            }
            if (receive)
            {
                CheckPairs(game);
                await Clients.Client(game.Player1.ConnectionId).SendAsync("ReceiveCard", sentCard, centerStack);
                await Clients.Client(game.Player2.ConnectionId).SendAsync("ReceiveCard", sentCard, centerStack);
                if(game.Player1.Deck.Cards.Count == 0)
                {
                    await Clients.Client(game.Player1.ConnectionId).SendAsync("gameOver", "won");
                    await Clients.Client(game.Player2.ConnectionId).SendAsync("gameOver", "lost");
                }
                else if(game.Player2.Deck.Cards.Count == 0)
                {
                    await Clients.Client(game.Player2.ConnectionId).SendAsync("gameOver", "won");
                    await Clients.Client(game.Player1.ConnectionId).SendAsync("gameOver", "lost");
                }
            }
            
        }


        public void CheckPairs(Game game)
        {
            //row1 card 1
            List<CenterStack> topCards = new List<CenterStack>{
                {game.Player1.Row[1]},
                {game.Player1.Row[2]},
                {game.Player1.Row[3]},
                {game.Player1.Row[4]},

                {game.Player2.Row[1]},
                {game.Player2.Row[2]},
                {game.Player2.Row[3]},
                {game.Player2.Row[4]},
            };

            var matches = topCards.GroupBy(x => x.Cards.Last().Value)
              .Where(g => g.Count() > 1)
              .ToDictionary(x => x.Key, y => y.Count());

            foreach(CenterStack i in topCards){
                if(matches.ContainsKey(i.Cards.Last().Value)){
                    i.CanBePlacedOn = true;
                }
            }

        //if it is a repeated number

            foreach (var i in game.Player1.Row.Keys) {
                if (game.Player1.Row[i].Cards.Count >= 2){
                    if (game.Player1.Row[i].Cards.Last().Value == game.Player1.Row[i].Cards[game.Player1.Row[i].Cards.Count -2].Value)
                {
                    game.Player1.Row[i].CanBePlacedOn = true;
                }
                }
            }

            foreach (var i in game.Player2.Row.Keys) {
                if (game.Player2.Row[i].Cards.Count >= 2){
                    if (game.Player2.Row[i].Cards.Last().Value == game.Player2.Row[i].Cards[game.Player2.Row[i].Cards.Count -2].Value)
                {
                    game.Player2.Row[i].CanBePlacedOn = true;
                }
                }
            }
            
         
        }

        public async Task ResetGame(Boolean WantsToReset)
        {
            var game = games?.Values.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            var gameID = game.Player1.ConnectionId;
            if(Context.ConnectionId == game.Player1.ConnectionId)
            {
                if (WantsToReset)
                    game.Player1.WantsReset = true;
                else
                    game.Player1.WantsReset = false;
                await Clients.Client(game.Player2.ConnectionId).SendAsync("resetCounter", game.Player1.WantsReset);
            }
            else if(Context.ConnectionId == game.Player2.ConnectionId)
            {
                if (WantsToReset)
                    game.Player2.WantsReset = true;
                else
                    game.Player2.WantsReset = false;
                await Clients.Client(game.Player1.ConnectionId).SendAsync("resetCounter", game.Player2.WantsReset);
            }
            if(game.Player1.WantsReset && game.Player2.WantsReset)
            {
                game.Player1.WantsReset = false;
                game.Player2.WantsReset = false;

                game.Player1.Deck.Cards.AddRange(game.Player1.Row[1].Cards);
                game.Player1.Deck.Cards.AddRange(game.Player1.Row[2].Cards);
                game.Player1.Deck.Cards.AddRange(game.Player1.Row[3].Cards);
                game.Player1.Deck.Cards.AddRange(game.Player1.Row[4].Cards);

                game.Player2.Deck.Cards.AddRange(game.Player2.Row[1].Cards);
                game.Player2.Deck.Cards.AddRange(game.Player2.Row[2].Cards);
                game.Player2.Deck.Cards.AddRange(game.Player2.Row[3].Cards);
                game.Player2.Deck.Cards.AddRange(game.Player2.Row[4].Cards);

                game.Player1.Row[1].Cards.Clear();
                game.Player1.Row[2].Cards.Clear();
                game.Player1.Row[3].Cards.Clear();
                game.Player1.Row[4].Cards.Clear();

                game.Player2.Row[1].Cards.Clear();
                game.Player2.Row[2].Cards.Clear();
                game.Player2.Row[3].Cards.Clear();
                game.Player2.Row[4].Cards.Clear();

                game.Player1.Row[1].Cards.Add(game.Player1.Deck.Draw());
                game.Player1.Row[2].Cards.Add(game.Player1.Deck.Draw());
                game.Player1.Row[3].Cards.Add(game.Player1.Deck.Draw());
                game.Player1.Row[4].Cards.Add(game.Player1.Deck.Draw());

                game.Player2.Row[1].Cards.Add(game.Player2.Deck.Draw());
                game.Player2.Row[2].Cards.Add(game.Player2.Deck.Draw());
                game.Player2.Row[3].Cards.Add(game.Player2.Deck.Draw());
                game.Player2.Row[4].Cards.Add(game.Player2.Deck.Draw());

                game.Player1.Row[1].CanBePlacedOn = false;
                game.Player1.Row[2].CanBePlacedOn = false;
                game.Player1.Row[3].CanBePlacedOn = false;
                game.Player1.Row[4].CanBePlacedOn = false;
                game.Player2.Row[1].CanBePlacedOn = false;
                game.Player2.Row[2].CanBePlacedOn = false;
                game.Player2.Row[3].CanBePlacedOn = false;
                game.Player2.Row[4].CanBePlacedOn = false;

                CheckPairs(game);
                
                var player1Stack1 = game.Player1.Row[1].Cards.Last().Value + game.Player1.Row[1].Cards.Last().Suit;
                var player1Stack2 = game.Player1.Row[2].Cards.Last().Value + game.Player1.Row[2].Cards.Last().Suit;
                var player1Stack3 = game.Player1.Row[3].Cards.Last().Value + game.Player1.Row[3].Cards.Last().Suit;
                var player1Stack4 = game.Player1.Row[4].Cards.Last().Value + game.Player1.Row[4].Cards.Last().Suit;
                var player2Stack1 = game.Player2.Row[1].Cards.Last().Value + game.Player2.Row[1].Cards.Last().Suit;
                var player2Stack2 = game.Player2.Row[2].Cards.Last().Value + game.Player2.Row[2].Cards.Last().Suit;
                var player2Stack3 = game.Player2.Row[3].Cards.Last().Value + game.Player2.Row[3].Cards.Last().Suit;
                var player2Stack4 = game.Player2.Row[4].Cards.Last().Value + game.Player2.Row[4].Cards.Last().Suit;
                await Clients.Client(game.Player1.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
                await Clients.Client(game.Player2.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
                await Clients.Client(game.Player1.ConnectionId).SendAsync("resetCounterToZero");
                await Clients.Client(game.Player2.ConnectionId).SendAsync("resetCounterToZero");
                await Clients.Client(game.Player1.ConnectionId).SendAsync("cardCount", game.Player1.Deck.Cards.Count);
                await Clients.Client(game.Player2.ConnectionId).SendAsync("cardCount", game.Player2.Deck.Cards.Count);
            }
        }
        
        public async Task NewGame()
        {
            var game = games?.Values.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            game.Player1.WantsReset = false;
            game.Player2.WantsReset = false;
            var newGame = new Game(game.Player1, game.Player2);
            games[game.Player1.ConnectionId] = newGame;

            CheckPairs(newGame);
            var player1Stack1 = newGame.Player1.Row[1].Cards.Last().Value + newGame.Player1.Row[1].Cards.Last().Suit;
            var player1Stack2 = newGame.Player1.Row[2].Cards.Last().Value + newGame.Player1.Row[2].Cards.Last().Suit;
            var player1Stack3 = newGame.Player1.Row[3].Cards.Last().Value + newGame.Player1.Row[3].Cards.Last().Suit;
            var player1Stack4 = newGame.Player1.Row[4].Cards.Last().Value + newGame.Player1.Row[4].Cards.Last().Suit;
            var player2Stack1 = newGame.Player2.Row[1].Cards.Last().Value + newGame.Player2.Row[1].Cards.Last().Suit;
            var player2Stack2 = newGame.Player2.Row[2].Cards.Last().Value + newGame.Player2.Row[2].Cards.Last().Suit;
            var player2Stack3 = newGame.Player2.Row[3].Cards.Last().Value + newGame.Player2.Row[3].Cards.Last().Suit;
            var player2Stack4 = newGame.Player2.Row[4].Cards.Last().Value + newGame.Player2.Row[4].Cards.Last().Suit;
            await Clients.Client(newGame.Player1.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
            await Clients.Client(newGame.Player2.ConnectionId).SendAsync("drawGame", player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4);
            await Clients.Client(newGame.Player1.ConnectionId).SendAsync("resetCounterToZero");
            await Clients.Client(newGame.Player2.ConnectionId).SendAsync("resetCounterToZero");
            await Clients.Client(newGame.Player1.ConnectionId).SendAsync("cardCount", newGame.Player1.Deck.Cards.Count);
            await Clients.Client(newGame.Player2.ConnectionId).SendAsync("cardCount", newGame.Player2.Deck.Cards.Count);
            await Clients.Client(newGame.Player1.ConnectionId).SendAsync("newGame");
            await Clients.Client(newGame.Player2.ConnectionId).SendAsync("newGame");
        }

        #endregion
    }
}