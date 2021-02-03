using CardPlayer.Data.Models;
using CardPlayer.Web.Models;
using CardPlayer.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CardPlayer.Web.Controllers
{
    public class HomeController : Controller
    {
        internal static string ApiLocation { get; set; } = "https://localhost:44331/api/CardGame/";
        internal static Game SelectedGame { get; set; } = null;
        internal static int SelectedGameId => (SelectedGame != null) ? SelectedGame.Id : 0;
        internal static Player SelectedPlayer { get; set; } = null;

        private void SetMenuToggles()
        {
            ViewData["PlayerIsSelected"] = SelectedPlayer != null;
            ViewData["GameIsSelected"] = SelectedGame != null;
            ViewData["PlayerIsInGame"] = SelectedPlayer != null && SelectedGame != null &&
                SelectedGame.Players.Contains(SelectedPlayer);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FoyerViewModel FoyerView = new FoyerViewModel
            {
                GameIdSelected = SelectedGameId,
                Games = new List<Game> { SelectedGame }
            };
            using (var Client = new HttpClient())
            {
                using var Response = await Client.GetAsync($"{ApiLocation}Foyer");
                var apiResponse = await Response.Content.ReadAsStringAsync();
                FoyerView.Games = JsonSerializer.Deserialize<List<Game>>(apiResponse);
            }

            SetMenuToggles();
            var DebugView = View(FoyerView);
            return DebugView;
            //return View("Index", FoyerView);
        }

        [HttpGet]
        public async Task<IActionResult> SelectGame()
        {
            FoyerViewModel FoyerView = new FoyerViewModel() 
                { GameIdSelected = SelectedGameId };
            using (var Client = new HttpClient())
            {
                using var Response = await Client.GetAsync($"{ApiLocation}Foyer");
                var apiResponse = await Response.Content.ReadAsStringAsync();
                FoyerView.Games = JsonSerializer.Deserialize<List<Game>>(apiResponse);
            }

            SetMenuToggles();
            return View(FoyerView);
        }

        [HttpPost]
        public async Task<IActionResult> SelectGame([FromForm] int GameIdSelected)
        {
            using (var Client = new HttpClient())
            {
                using var Response = await Client.GetAsync(
                    $"{ApiLocation}SetCurrentGameTo/{GameIdSelected}");
                var apiResponse = await Response.Content.ReadAsStringAsync();
                SelectedGame = JsonSerializer.Deserialize<Game>(apiResponse);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Original()
        {

            CardTableViewModel CardTable = new CardTableViewModel
            {
                Title = "Cards in original order:",
                UndealtCards = (SelectedGame != null) ? new Deck(SelectedGame.StartDeck) :
                    new Deck(DeckType.MakeDeckType(StandardDecks.Traditional), new List<Card>())
            };

            SetMenuToggles();
            return View("DeckView", CardTable);
        }

        [HttpGet]
        public IActionResult CreateGame()
        {
            SetMenuToggles();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame(Game game)
        {
            CardTableViewModel CardTable = new CardTableViewModel();
            if (ModelState.IsValid)
            {
                using var Client = new HttpClient();
                int GameId;
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}NewGame/{game.StandardDeckType}/{game.MaximumPlayers}/{game.HandSize}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    GameId = JsonSerializer.Deserialize<int>(apiResponse);
                    CardTable.Title = $"New Game (ID: {GameId}) Created";
                }
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}GetStartDeck/{GameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    CardTable.UndealtCards = JsonSerializer.Deserialize<Deck>(apiResponse);
                }
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}SetCurrentGameTo/{GameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    SelectedGame = JsonSerializer.Deserialize<Game>(apiResponse);
                }

                SetMenuToggles();
                return View("CardTableView", CardTable);
            }
            else
                return InvalidModelView();    
        }

        [HttpGet]
        public ActionResult JoinGame()
        {
            SetMenuToggles();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JoinGame([FromBody] Player player)
        {
            Player Homonym = SelectedGame.Players.FirstOrDefault(p => p.Name == player.Name);
            AddedPlayerViewModel AddedPlayer = new AddedPlayerViewModel
            {
                MaxPlayers = SelectedGame.MaximumPlayers,
                CurrentPlayers = SelectedGame.Players.Count,
                PlayerIsNew = Homonym == null,
                PlayerName = player.Name
            };
            if (AddedPlayer.PlayerIsNew)
            {
                if (ModelState.IsValid)
                {
                    using var Client = new HttpClient();

                    using var Response = await Client.PutAsync(
                        $"{ApiLocation}AddPlayer/{SelectedGameId}",
                        new StringContent(JsonSerializer.Serialize(player),
                            Encoding.UTF8, "application/json"));
                    if (Response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        AddedPlayer.PlayerAdded = false;
                    }
                    else
                    {
                        AddedPlayer.PlayerAdded = true;
                        AddedPlayer.CurrentPlayers++;
                        SelectedPlayer = player;
                        SelectedGame.Players.Add(player);
                    }
                }
                else
                    return InvalidModelView();
            }
            else
                SelectedPlayer = Homonym;

            SetMenuToggles();
            return View("AddedToGame", AddedPlayer);
        }

        [HttpGet]
        public async Task<IActionResult> Reshuffle()
        {
            if (SelectedGame == null)
                return NoGameView();
            using (var Client = new HttpClient())
            {
                using var Response = await Client.GetAsync(
                    $"{ApiLocation}ShuffleDeck/{SelectedGameId}");
                var apiResponse = await Response.Content.ReadAsStringAsync();
                SelectedGame.ShuffledDeck = JsonSerializer.Deserialize<Deck>(apiResponse);
            }
            CardTableViewModel CardTable = new CardTableViewModel
            {
                Title = "Cards Reshuffled:",
                UndealtCards = new Deck(SelectedGame.ShuffledDeck)
            };

            SetMenuToggles();
            return View("DeckView", CardTable);
        }

        [HttpGet]
        public IActionResult Shuffled()
        {
            if (SelectedGame == null)
                return NoGameView();
            CardTableViewModel CardTable = new CardTableViewModel
            {
                Title = "Cards Shuffled:",
                UndealtCards = new Deck(SelectedGame.ShuffledDeck)
            };

            SetMenuToggles();
            return View("DeckView", CardTable);
        }

        [HttpGet]
        public IActionResult ShuffledAndSuited()
        {
            if (SelectedGame == null)
                return NoGameView();
            CardTableViewModel CardTable = new CardTableViewModel
            (
                title: "Cards shuffled and separated into suits:",
                undealtCards: new Deck(SelectedGame.StandardDeckType)
            );
            CardTable.UndealtCards.Cards.Clear();
            var DealtCards =
                from card in SelectedGame.ShuffledDeck.Cards
                group card by card.CardSuit into suits
                select suits;
            foreach (var suit in DealtCards)
                CardTable.UndealtCards.Cards.AddRange(suit);

            SetMenuToggles();
            return View("DeckView", CardTable);
        }

        [HttpGet]
        public async Task<IActionResult> DrawCard()
        {
            if (SelectedGame == null)
                return NoGameView();
            if (!SelectedGame.Players.Contains(SelectedPlayer))
                return NotPlayingView();
            if (!SelectedPlayer.PlayerHand.Cards.Any())
            {
                SelectedPlayer.PlayerHand = new Hand();
            }
            using (var Client = new HttpClient())
            {
                Card CardDrawn;
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}DealCard/{SelectedGameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    CardDrawn = JsonSerializer.Deserialize<Card>(apiResponse);
                }
                using (var Response = await Client.PutAsync(
                    $"{ApiLocation}AddToHand/{SelectedGameId}/{SelectedPlayer.Id}",
                    new StringContent(JsonSerializer.Serialize(CardDrawn),
                        Encoding.UTF8, "application/json")))
                {
                    if (Response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return UnexpectedErrorView
                            ($"API Problem adding card to Player " +
                            $"{SelectedPlayer.Id} in Game {SelectedGameId}");
                    }
                    else
                    {
                        SelectedPlayer.PlayerHand.Cards.Add(CardDrawn);
                        SelectedGame.ShuffledDeck.Cards.Remove(CardDrawn);
                    }
                }
            }

            return ShowHands();
        }

        [HttpGet]
        public async Task<IActionResult> DealHand()
        {
            if (SelectedGame == null)
                return NoGameView();
            if (!SelectedGame.Players.Contains(SelectedPlayer))
                return NotPlayingView();
            if (!SelectedPlayer.PlayerHand.Cards.Any())
            {
                SelectedPlayer.PlayerHand = new Hand();
            }
            using (var Client = new HttpClient())
            {
                for (int drawNumber = 0; drawNumber < SelectedGame.HandSize; drawNumber++)
                {
                    Card CardDrawn;
                    using (var Response = await Client.GetAsync(
                        $"{ApiLocation}DealCard/{SelectedGameId}"))
                    {
                        var apiResponse = await Response.Content.ReadAsStringAsync();
                        CardDrawn = JsonSerializer.Deserialize<Card>(apiResponse);
                    }
                    using (var Response = await Client.PutAsync(
                        $"{ApiLocation}AddToHand/{SelectedGameId}/{SelectedPlayer.Id}",
                        new StringContent(JsonSerializer.Serialize(CardDrawn),
                            Encoding.UTF8, "application/json")))
                    {
                        if (Response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            return UnexpectedErrorView
                                ($"API Problem adding card to Player " +
                                $"{SelectedPlayer.Id} in Game {SelectedGameId}");
                        }
                        else
                        {
                            SelectedPlayer.PlayerHand.Cards.Add(CardDrawn);
                            SelectedGame.ShuffledDeck.Cards.Remove(CardDrawn);
                        }
                    }
                }
            }

            return ShowHands();
        }

        [HttpGet]
        public IActionResult ShowHands()
        {

            if (SelectedGame == null)
                return NoGameView();
            if (!SelectedGame.Players.Any())
                return NoPlayersView();
            CardTableViewModel CardTable = new CardTableViewModel
            (
                title: "Cards dealt into hands:",
                undealtCards: new Deck(SelectedGame.ShuffledDeck)
            );
            foreach (var player in SelectedGame.Players)
            {
                CardTable.Hands.Add(player.PlayerHand);
                CardTable.PlayerNames.Add(player.Name);
            }

            SetMenuToggles();
            return View("CardTableView", CardTable);
        }

        private ViewResult NoGameView()
        {
            SetMenuToggles();
            return View("DisplayText",
                        new TextViewModel
                        {
                            Title = "NoSelectedGame",
                            Text = "No game has been selected.",
                            CssStyle = "font-size: 3vw;"
                        });
        }
        private ViewResult NotPlayingView()
        {
            SetMenuToggles();
            return View("DisplayText",
                        new TextViewModel
                        {
                            Title = "NotPlaying",
                            Text = "The currently selected player is not in this game.",
                            CssStyle = "font-size: 2vw;"
                        });
        }
        private ViewResult NoPlayersView()
        {
            SetMenuToggles();
            return View("DisplayText",
                        new TextViewModel
                        {
                            Title = "NoPlayers",
                            Text = "There are no players in this game.",
                            CssStyle = "font-size: 3vw; font-color: blue;"
                        });
        }
        private ViewResult InvalidModelView()
        {
            SetMenuToggles();
            return View("DisplayText",
                        new TextViewModel
                        {
                            Title = "InvalidModel",
                            Text = "The model state was invalid.",
                            CssStyle = "font-size: 5vw; font-color: red;"
                        });
        }
        private ViewResult UnexpectedErrorView(string Message)
        {
            SetMenuToggles();
            return View("DisplayText",
                        new TextViewModel
                        {
                            Title = "UnexpectedError",
                            Text = Message,
                            CssStyle = "font-size: 5vw; font-color: red;"
                        });
        }
    }
}