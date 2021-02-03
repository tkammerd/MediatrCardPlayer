using CardPlayer.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CardPlayer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardGameController : ControllerBase
    {
        private readonly IMediator _mediator;
        private static int _currentGame = -1;
        private static readonly List<Game> _foyer;

        public static Game CurrentGame
        {
            get => (_currentGame == -1) ? null : _foyer[_currentGame];
        }

        public CardGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        static CardGameController()
        {
            _foyer = new List<Game>();
        }

        #region Nested Classes

        public class NewGameRequest : IRequest<int>
        {
            public StandardDecks TypeOfDeck { get; set; }
            public int MaxPlayers { get; set; }
            public int HandSize { get; set; }
        }

        public class NewGameRequestHandler : IRequestHandler<NewGameRequest, int>
        {
            public Task<int> Handle(NewGameRequest requestValues, CancellationToken cancellation)
            {
                return Task.Run(() =>
                {
                    int NewId = _foyer.Any() ? _foyer.Select(g => g.Id).Max() + 1 : 1;
                    Game NewGame = new Game(requestValues.TypeOfDeck)
                    {
                        Id = NewId,
                        MaximumPlayers = requestValues.MaxPlayers,
                        HandSize = requestValues.HandSize
                    };
                    _foyer.Add(NewGame);
                    return NewId;
                });
            }
        }

        public class SetCurrentGameRequest : IRequest<int>
        {
            public int GameId { get; set; }
        }

        public class SetCurrentGameRequestHandler : IRequestHandler<SetCurrentGameRequest, int>
        {
            public Task<int> Handle(SetCurrentGameRequest requestValues, CancellationToken cancellation)
            {
                return Task.Run(() =>
                {
                    _currentGame = _foyer.FindIndex(g => g.Id == requestValues.GameId);
                    return _currentGame;
                });
            }
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return await Task.Run(() =>
            {
                MethodInfo[] MethodInfos = typeof(CardGameController).GetMethods(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var Interfaces = "\n";
                foreach (var method in MethodInfos)
                {
                    Interfaces += "                          <li>" + method.Name + "</li>\n";
                }
                var HttpBody = @"
                    <html>
                      <head>
                        <title>CardGame API</title>
                      </head>
                      <body>
                        <h1>The CardGame RESTful API is Running.</h1>
                        <h2>It provides the following interfaces:</h2>
                        <ul>" +
                          Interfaces + 
                        @"                        </ul>
                      </body>
                    </html>";
                return base.Content(HttpBody, "text/html");
            });
        }

        // GET: api/CardGame/NewGame/2/4/5
        [HttpGet("NewGame/{deckTypeRequested}/{maxPlayersRequested}/{handSizeRequested}")]
        public async Task<int> NewGame(
            [FromRoute] StandardDecks deckTypeRequested,
            [FromRoute] int maxPlayersRequested,
            [FromRoute] int handSizeRequested)
        {
            int GameId = await _mediator.Send(new NewGameRequest
            {
                TypeOfDeck = deckTypeRequested,
                MaxPlayers = maxPlayersRequested,
                HandSize = handSizeRequested
            });
            _currentGame = _foyer.FindIndex(g => g.Id == GameId);
            return GameId;
        }

        [HttpGet("Foyer")]
        public List<Game> Foyer()
        {
            return _foyer;
        }

        // GET: api/CardGame/SetCurrentGameTo/1
        [HttpGet("SetCurrentGameTo/{gameId}")]
        public async Task<Game> SetCurrentGameTo([FromRoute] int gameId)
        {
            int Index = await _mediator.Send(new SetCurrentGameRequest { GameId = gameId });
            return _foyer[Index];
        }

        // GET: api/CardGame/GetGame/1
        [HttpGet("GetGame/{gameId}")]
        public async Task<Game> GetGame([FromRoute] int gameId)
        {
            int Index = await _mediator.Send(new SetCurrentGameRequest { GameId = gameId });
            return _foyer[Index];
        }

        //// GET: api/CardGame/DealCard
        //[HttpGet("DealCard")]
        //public async Task<ActionResult<Card>> DealCard()
        //{
        //    return await Task.Run(() => DealCardFrom(CurrentGame));
        //}

        // GET: api/CardGame/DealCard/1
        [HttpGet("DealCard/{gameId}")]
        public async Task<ActionResult<Card>> DealCard(int gameId)
        {
            return await Task.Run(() => DealCardFrom(_foyer.Find(g => g.Id == gameId)));
        }

        private static Card DealCardFrom(Game game)
        {
            Card CardDealt = game.ShuffledDeck.Cards.FirstOrDefault();
            game.ShuffledDeck.Cards.RemoveRange(0, 1);
            return CardDealt;
        }

        //// GET: api/CardGame/GetDeckType
        //[HttpGet("GetDeckType")]
        //public async Task<ActionResult<DeckType>> GetDeckType()
        //{
        //    return await Task.Run(() => CurrentGame.StartDeck.TypeOfDeck);
        //}

        // GET: api/CardGame/GetDeckType/1
        [HttpGet("GetDeckType/{gameId}")]
        public async Task<ActionResult<DeckType>> GetDeckType(int gameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == gameId).StartDeck.TypeOfDeck);
        }

        //// GET: api/CardGame/GetStartDeck
        //[HttpGet("GetStartDeck")]
        //public async Task<ActionResult<Deck>> GetStartDeck()
        //{
        //    return await Task.Run(() => CurrentGame.StartDeck);
        //}

        // GET: api/CardGame/GetStartDeck/1
        [HttpGet("GetStartDeck/{gameId}")]
        public async Task<ActionResult<Deck>> GetStartDeck(int gameId)
        {
            Deck result;
            result = await Task.Run(() =>
                _foyer.Find(g => g.Id == gameId).StartDeck);
            return result;
        }

        //// GET: api/CardGame/GetShuffledDeck
        //[HttpGet("GetShuffledDeck")]
        //public async Task<ActionResult<Deck>> GetShuffledDeck()
        //{
        //    return await Task.Run(() => CurrentGame.ShuffledDeck);
        //}

        // GET: api/CardGame/GetShuffledDeck/1
        [HttpGet("GetShuffledDeck/{gameId}")]
        public async Task<ActionResult<Deck>> GetShuffledDeck(int gameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == gameId).ShuffledDeck);
        }

        // GET: api/CardGame/ShuffleDeck/1
        [HttpGet("ShuffleDeck/{gameId}")]
        public async Task<ActionResult<Deck>> ShuffleDeck(int gameId)
        {
            return await Task.Run(() =>
            {
                var ChosenGame = _foyer.Find(g => g.Id == gameId);
                ChosenGame.ShuffledDeck = ChosenGame.StartDeck.Shuffle();
                ChosenGame.Players.ForEach(p => p.PlayerHand = new Hand());
                return ChosenGame.ShuffledDeck;
            });
        }

        //// GET: api/CardGame/ShuffleDeck
        //[HttpGet("ShuffleDeck")]
        //public async Task<ActionResult<Deck>> ShuffleDeck()
        //{
        //    return await Task.Run(() =>
        //    {
        //        CurrentGame.ShuffledDeck = CurrentGame.StartDeck.Shuffle();
        //        CurrentGame.Players.ForEach(p => p.PlayerHand = new Hand());
        //        return CurrentGame.ShuffledDeck;
        //    });
        //}

        //[HttpGet("IncrementMaxPlayers/{gameId}")]
        //public async Task<ActionResult<int>> IncrementMaxPlayers(int gameId)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return ++_foyer.Find(g => g.Id == gameId).MaximumPlayers;
        //    });
        //}
        
        // GET: api/CardGame/AddToMaxPlayers/1/2
        [HttpGet("AddToMaxPlayers/{gameId}/{numberAdded}")]
        public async Task<ActionResult<int>> AddToMaxPlayers(int gameId, [FromBody] int numberAdded)
        {
            return await Task.Run(() =>
            {
                return _foyer.Find(g => g.Id == gameId).MaximumPlayers += numberAdded;
            });
        }

        //[HttpGet("CreatePlayer/{playerName}")]
        //public async Task<ActionResult<Player>> CreatePlayer(string PlayerName)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return new Player
        //        {
        //            Name = PlayerName,
        //            PlayerHand = new Hand { Cards = CurrentGame.ShuffledDeck.Cards.GetRange(10, 3) }
        //        };
        //    });
        //}

        // PUT: api/CardGame/AddPlayer/1
        [HttpPut("AddPlayer/{gameId}")]
        public async Task<ActionResult> AddPlayer([FromRoute] int gameId, [FromBody] Player player)
        {
            Game DesiredGame = _foyer.Find(g => g.Id == gameId);
            if (DesiredGame.Players.Count < DesiredGame.MaximumPlayers)
            {
                await Task.Run(() => DesiredGame.Players.Add(player));
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT: api/CardGame/AddToHand/1/2498731
        [HttpPut("AddToHand/{gameId}/{playerId}")]
        public async Task<ActionResult<Hand>> AddToHand([FromRoute] int gameId, [FromRoute] int playerId, [FromBody] Card card)
        {
            Game DesiredGame = _foyer.Find(g => g.Id == gameId);
            if (DesiredGame == null)
                return BadRequest();
            Player DesiredPlayer = DesiredGame.Players.Find(p => p.Id == playerId);
            if (DesiredPlayer == null)
                return BadRequest();
            await Task.Run(() =>
            {
                if (!DesiredPlayer.PlayerHand.Cards.Any())
                {
                    DesiredPlayer.PlayerHand = new Hand();
                }
                DesiredPlayer.PlayerHand.Cards.Add(card);
            });
            return DesiredPlayer.PlayerHand;
        }
    }
}
