using System;
using Xunit;
using CardPlayer.Web.Controllers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using CardPlayer.Data.Models;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace CardPlayer.Test
{
    public class CardsInAHandShould
    {
        [Fact]
        public async void BeMissingFromTheShuffledDeck()
        {
            //Arrange
            var ApiLocation = HomeController.ApiLocation;
            var DeckType = StandardDecks.Traditional;
            var MaxPlayers = 2;
            var HandSize = 5;
            int DebugGameId;
            var DebugPlayer = new Player() { Name = "Debugger" };
            Hand DebugHand;
            Card CardDrawn;
            Deck StartDeck;
            Deck ShuffledDeck;
            //Act
            using (var Client = new HttpClient())
            {
                // Create a game
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}NewGame/{DeckType}/{MaxPlayers}/{HandSize}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    DebugGameId = JsonSerializer.Deserialize<int>(apiResponse);
                }
                // Add a player to the game
                using (var Response = await Client.PutAsync(
                    $"{ApiLocation}AddPlayer/{DebugGameId}",
                    new StringContent(JsonSerializer.Serialize(DebugPlayer),
                        Encoding.UTF8, "application/json")))
                {
                    Assert.NotEqual(HttpStatusCode.BadRequest, Response.StatusCode);
                }
                // Deal a card from the deck
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}DealCard/{DebugGameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    CardDrawn = JsonSerializer.Deserialize<Card>(apiResponse);
                }
                // And give it to the player
                using (var Response = await Client.PutAsync(
                    $"{ApiLocation}AddToHand/{DebugGameId}/{DebugPlayer.Id}",
                    new StringContent(JsonSerializer.Serialize(CardDrawn),
                        Encoding.UTF8, "application/json")))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    DebugHand = JsonSerializer.Deserialize<Hand>(apiResponse);
                }
                // Get the starting deck
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}GetStartDeck/{DebugGameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    StartDeck = JsonSerializer.Deserialize<Deck>(apiResponse);
                }
                // Get the shuffled deck (the remaining cards)
                using (var Response = await Client.GetAsync(
                    $"{ApiLocation}GetShuffledDeck/{DebugGameId}"))
                {
                    var apiResponse = await Response.Content.ReadAsStringAsync();
                    ShuffledDeck = JsonSerializer.Deserialize<Deck>(apiResponse);
                }
            }
            Deck DeckDifference = new Deck(StartDeck);
            foreach (var card in StartDeck.Cards)
            {
                Card ShuffleMatch = ShuffledDeck.Cards.FirstOrDefault(c => c.Name == card.Name);
                Card DifferenceMatch = DeckDifference.Cards.FirstOrDefault(c => c.Name == card.Name);
                if (ShuffleMatch != null)
                {
                    DeckDifference.Cards.Remove(DifferenceMatch);
                    ShuffledDeck.Cards.Remove(ShuffleMatch);
                }
            }
            Assert.Single<Card>(DeckDifference.Cards);
            Assert.Single<Card>(DebugHand.Cards);
            Assert.Equal(CardDrawn.Name, DeckDifference.Cards.Single().Name);
            Assert.Equal(DeckDifference.Cards.Single().Name, DebugHand.Cards.Single().Name);
        }
    }
}
