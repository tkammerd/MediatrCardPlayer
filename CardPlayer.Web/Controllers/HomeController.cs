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
using System.Text.Json;
using System.Threading.Tasks;

namespace CardPlayer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        private const StandardDecks START_DECK_TYPE = StandardDecks.Traditional;
        private const int START_HAND_SIZE = 5;
        private static readonly Random _rng;
        private static Game _currentGame;
        private static Player _currentPlayer;
        
        private static CardTableViewModel _cardTable;

        private bool PlayerCreated { get; set; }
        private bool GameCreated { get; set; }
        private bool PlayerJoinedGame { get; set; }

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;

            PlayerCreated = HomeController._currentPlayer != null;
            GameCreated = HomeController._currentGame.MaximumPlayers != 0;
            PlayerJoinedGame = PlayerCreated && GameCreated &&
                HomeController._currentGame.Players.Contains(HomeController._currentPlayer);
        }

        static HomeController()
        {
            HomeController._rng = new Random();
            _currentGame = new Game()
            {
                StandardDeckType = START_DECK_TYPE,
                HandSize = START_HAND_SIZE,
                StartDeck = new Deck(START_DECK_TYPE),
                ShuffledDeck = new Deck(START_DECK_TYPE)
            };
            HomeController.Shuffle();
           _cardTable = new CardTableViewModel("", HomeController._currentGame.StartDeck);
        }

        private static void Shuffle()
        {
            HomeController._currentGame.ShuffledDeck.Cards =
            (
                from card in HomeController._currentGame.StartDeck.Cards
                orderby HomeController._rng.Next()
                select card
            ).ToList();
            HomeController._currentGame.Players.ForEach(p => p.PlayerHand = new Hand());
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            _cardTable.Title = "Cards in original order:";
            _cardTable.UndealtCards = new Deck(HomeController._currentGame.StartDeck);

            return View("DeckView", _cardTable);
        }

        [HttpGet]
        public IActionResult CreateGame()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            return View();
        }

        [HttpPost]
        public ActionResult CreateGame(Game game)
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            if (ModelState.IsValid)
            {
                _currentGame = game;
                HomeController._currentGame.StartDeck = new Deck(_currentGame.StandardDeckType);
                HomeController._currentGame.ShuffledDeck = new Deck(_currentGame.StandardDeckType);
                HomeController.Shuffle();
                _cardTable = new CardTableViewModel("", HomeController._currentGame.StartDeck);
                GameCreated = true;
                ViewData["GameCreated"] = GameCreated;
                PlayerJoinedGame = false;
                ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
                //game.TaskId = StartGame(game);
            }
            
            return View();
        }

        //public async Task StartGame(Game game)
        //{
        //    string MessageFromUI = await Mediator.Send(new MessageRequest());
        //}

        [HttpGet]
        public ActionResult CreatePlayer()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            return View();
        }

        [HttpPost]
        public ActionResult CreatePlayer(Player player)
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            if (ModelState.IsValid)
            {
                HomeController._currentPlayer = player;
                PlayerCreated = true;
                ViewData["PlayerCreated"] = PlayerCreated;
                PlayerJoinedGame = PlayerCreated && GameCreated &&
                    HomeController._currentGame.Players.Contains(HomeController._currentPlayer);
            }

            return View();
        }

        [HttpGet]
        public ActionResult JoinGame()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            if (_currentGame.Players.Count < _currentGame.MaximumPlayers)
            {
                _currentGame.Players.Add(HomeController._currentPlayer);
                PlayerJoinedGame = true;
                ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
                return View("AddedToGame", _currentGame);
            }
            else
            {
                return View("GameFull", _currentGame);
            }
        }

        [HttpGet]
        public IActionResult ReShuffle()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            HomeController.Shuffle();
            _cardTable.Title = "Cards Reshuffled:";
            _cardTable.UndealtCards = new Deck(HomeController._currentGame.ShuffledDeck);

            return View("DeckView", _cardTable);
        }

        [HttpGet]
        public IActionResult Shuffled()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            _cardTable.Title = "Cards shuffled:";
            _cardTable.UndealtCards = new Deck(HomeController._currentGame.ShuffledDeck);

            return View("DeckView", _cardTable);
        }

        [HttpGet]
        public IActionResult ShuffledAndSuited()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            _cardTable.Title = "Cards shuffled and separated into suits:";
            _cardTable.UndealtCards.Cards = new List<Card>();
            var DealtCards =
                from card in HomeController._currentGame.ShuffledDeck.Cards
                group card by card.CardSuit into suits
                select suits;
            foreach (var suit in DealtCards)
                _cardTable.UndealtCards.Cards.AddRange(suit);

            return View("SuitedDeckView", _cardTable);
        }

        [HttpGet]
        public IActionResult DrawCard()
        {
            if (!HomeController._currentPlayer.PlayerHand.Cards.Any())
            {
                HomeController._currentPlayer.PlayerHand = new Hand();
            }
            HomeController._currentPlayer.PlayerHand.Cards.Add(
                HomeController._currentGame.ShuffledDeck.Cards.FirstOrDefault());
            HomeController._currentGame.ShuffledDeck.Cards.RemoveRange(0, 1);
            return ShowHands();
        }

        [HttpGet]
        public IActionResult DealHand()
        {
            if (!HomeController._currentPlayer.PlayerHand.Cards.Any())
            {
                HomeController._currentPlayer.PlayerHand = new Hand();
            }
            HomeController._currentPlayer.PlayerHand.Cards.AddRange(
                HomeController._currentGame.ShuffledDeck.Cards.Take(_currentGame.HandSize).ToList());
            HomeController._currentGame.ShuffledDeck.Cards.RemoveRange(0, _currentGame.HandSize);
            return ShowHands();
        }

        [HttpGet]
        public IActionResult ShowHands()
        {
            ViewData["PlayerCreated"] = PlayerCreated;
            ViewData["GameCreated"] = GameCreated;
            ViewData["PlayerJoinedGame"] = PlayerJoinedGame;
            _cardTable.Title = "Cards dealt into hands:";
            _cardTable.Hands = new List<Hand>();
            foreach (var player in _currentGame.Players)
                _cardTable.Hands.Add(player.PlayerHand);
            _cardTable.UndealtCards = new Deck(HomeController._currentGame.ShuffledDeck);

            return View("CardTableView", _cardTable);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
