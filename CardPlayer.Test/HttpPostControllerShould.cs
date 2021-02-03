using CardPlayer.Data.Models;
using CardPlayer.Web.Controllers;
using CardPlayer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CardPlayer.Test
{
    public class HttpPostControllerShould
    {
        private readonly ITestOutputHelper _output;

        HomeController DebugController { get; set; } = new HomeController();
        int SelectGameDeserializedBody { get; set; }
        Player JoinGameDeserializedBody { get; set; }

        public HttpPostControllerShould(ITestOutputHelper output)
        {
            _output = output;

            var SelectGameSerializedIntTask =
                CreateDebugGame(StandardDecks.Traditional, 4, 5);
            SelectGameSerializedIntTask.Wait();
            var SelectGameSerializedInt = SelectGameSerializedIntTask.Result.Value.ToString();
            SelectGameDeserializedBody = JsonSerializer
                .Deserialize<int>(SelectGameSerializedInt);
            string JoinGameSerializedPlayer = @"
                {
                    ""Id"": 2020390862,
                    ""Name"": ""Debugger"",
                    ""PlayerHand"": 
                    {
                        ""Cards"": []
                    }
                }";
            JoinGameDeserializedBody = JsonSerializer
                .Deserialize<Player>(JoinGameSerializedPlayer);
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            // This is just a stub until we can figure out how to do this in .Net Core 3.1
            return $"<html><body>{viewName} {model}</body></html>";
        }

        private bool HtmlContainsErrorOutput(string viewName, string htmlView)
        {
            // This is just a stub right now, but will find various error outputs in the HTML.
            // The following line produces no errors:
            var ErrorExists = htmlView.Contains("HtmlContainsErrorOutput=Error");
            // The following line produces one error for the action in the parameter:
            //var ErrorExists = htmlView.Contains("AddedToGame");
            if (ErrorExists)
                _output.WriteLine($"HTML output for the {viewName} view contains an error.");
            return ErrorExists;
        }

        private async static Task<ActionResult<int>> CreateDebugGame(
            StandardDecks deckType, int maxPlayers, int handSize)
        {
            var DebugGameId = -1;
            using (var Client = new HttpClient())
            {
                using var Response = await Client.GetAsync(
                    $"{HomeController.ApiLocation}NewGame/{deckType}/{maxPlayers}/{handSize}");
                var apiResponse = await Response.Content.ReadAsStringAsync();
                DebugGameId = JsonSerializer.Deserialize<int>(apiResponse);
            }
            return DebugGameId;
        }

        [Fact]
        [Trait("Category", "CoreCandidate")]
        public async void NotThrowExceptions()
        {
            _ = await DebugController.SelectGame(SelectGameDeserializedBody);
            _ = await DebugController.JoinGame(JoinGameDeserializedBody);

            Assert.True(true, "An exception occurred");
        }

        [Fact]
        [Trait("Category", "CoreCandidate")]
        public async void ReturnCorrectActionResults()
        {
            RedirectToActionResult SelectGameResult = (RedirectToActionResult)
                (await DebugController.SelectGame(SelectGameDeserializedBody));
            ViewResult JoinGameResult = (ViewResult)
                (await DebugController.JoinGame(JoinGameDeserializedBody));

            Assert.IsType<RedirectToActionResult>(SelectGameResult);

            Assert.IsType<ViewResult>(JoinGameResult);
            Assert.IsType<AddedPlayerViewModel>(JoinGameResult.Model);
            var ActualViewName = JoinGameResult.ViewName ?? nameof(DebugController.JoinGame);
            Assert.False(HtmlContainsErrorOutput(
                ActualViewName, RenderRazorViewToString(ActualViewName, JoinGameResult.Model)));
        }
    }
}
