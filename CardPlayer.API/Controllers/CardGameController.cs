using CardPlayer.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        MaximumPlayers = requestValues.MaxPlayers
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

        // GET: api/CardGame/NewGame/2/4
        [HttpGet("NewGame/{deckTypeRequested}/{maxPlayersRequested}")]
        public async Task<int> NewGame(
            [FromRoute] StandardDecks deckTypeRequested,
            [FromRoute] int maxPlayersRequested)
        {
            int GameId = await _mediator.Send(new NewGameRequest
            {
                TypeOfDeck = deckTypeRequested,
                MaxPlayers = maxPlayersRequested
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
        [HttpGet("SetCurrentGameTo/{id}")]
        public async Task<Game> SetCurrentGameTo([FromRoute] int id)
        {
            int Index = await _mediator.Send(new SetCurrentGameRequest { GameId = id });
            return _foyer[Index];
        }

        // GET: api/CardGame
        [HttpGet]
        public IActionResult Get()
        {
            return NoContent();
        }

        // GET: api/CardGame/DealCard
        [HttpGet("DealCard")]
        public async Task<ActionResult<Card>> DealCard()
        {
            return await Task.Run(() => DealCardFrom(CurrentGame));
        }

        // GET: api/CardGame/DealCard/1
        [HttpGet("DealCard/{GameId}")]
        public async Task<ActionResult<Card>> DealCard(int GameId)
        {
            return await Task.Run(() => DealCardFrom(_foyer.Find(g => g.Id == GameId)));
        }

        private Card DealCardFrom(Game game)
        {
            Card CardDealt = game.ShuffledDeck.Cards.FirstOrDefault();
            game.ShuffledDeck.Cards.RemoveRange(0, 1);
            return CardDealt;
        }

        // GET: api/CardGame/GetDeckType
        [HttpGet("GetDeckType")]
        public async Task<ActionResult<DeckType>> GetDeckType()
        {
            return await Task.Run(() => CurrentGame.StartDeck.TypeOfDeck);
        }

        // GET: api/CardGame/GetDeckType/1
        [HttpGet("GetDeckType/{GameId}")]
        public async Task<ActionResult<DeckType>> GetDeckType(int GameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == GameId).StartDeck.TypeOfDeck);
        }

        // GET: api/CardGame/GetStartDeck
        [HttpGet("GetStartDeck")]
        public async Task<ActionResult<Deck>> GetStartDeck()
        {
            return await Task.Run(() => CurrentGame.StartDeck);
        }

        // GET: api/CardGame/GetStartDeck/1
        [HttpGet("GetStartDeck/{GameId}")]
        public async Task<ActionResult<Deck>> GetStartDeck(int GameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == GameId).StartDeck);
        }

        // GET: api/CardGame/GetShuffledDeck
        [HttpGet("GetShuffledDeck")]
        public async Task<ActionResult<Deck>> GetShuffledDeck()
        {
            return await Task.Run(() => CurrentGame.ShuffledDeck);
        }

        // GET: api/CardGame/GetShuffledDeck/1
        [HttpGet("GetShuffledDeck/{GameId}")]
        public async Task<ActionResult<Deck>> GetShuffledDeck(int GameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == GameId).ShuffledDeck);
        }

        // GET: api/CardGame/GetRemainingCards
        [HttpGet("GetRemainingCards")]
        public async Task<ActionResult<List<Card>>> GetRemainingCards()
        {
            return await Task.Run(() => CurrentGame.ShuffledDeck.Cards);
        }

        // GET: api/CardGame/GetRemainingCards/1
        [HttpGet("GetRemainingCards/{GameId}")]
        public async Task<ActionResult<List<Card>>> GetRemainingCards(int GameId)
        {
            return await Task.Run(() =>
                _foyer.Find(g => g.Id == GameId).ShuffledDeck.Cards);
        }

        [HttpGet("IncrementMaxPlayers/{GameId}")]
        public async Task<ActionResult<int>> IncrementMaxPlayers(int GameId)
        {
            return await Task.Run(() =>
            {
                return ++_foyer.Find(g => g.Id == GameId).MaximumPlayers;
            });
        }

        [HttpGet("AddToMaxPlayers/{GameId}")]
        public async Task<ActionResult<int>> AddToMaxPlayers(int GameId, [FromBody] int NumberAdded)
        {
            return await Task.Run(() =>
            {
                return _foyer.Find(g => g.Id == GameId).MaximumPlayers += NumberAdded;
            });
        }

        [HttpGet("CreatePlayer/{PlayerName}")]
        public async Task<ActionResult<Player>> CreatePlayer(string PlayerName)
        {
            return await Task.Run(() =>
            {
                return new Player
                {
                    Name = PlayerName,
                    PlayerHand = new Hand { Cards = CurrentGame.ShuffledDeck.Cards.GetRange(10, 3) }
                };
            });
        }

        [HttpPost("AddPlayer")]
        public async Task<ActionResult> AddPlayer([FromBody] Player player)
        {
            if (CurrentGame.Players.Count < CurrentGame.MaximumPlayers)
            {
                await Task.Run(() => CurrentGame.Players.Add(player));
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        //// GET: api/<CardGameController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<CardGameController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<CardGameController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<CardGameController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<CardGameController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
