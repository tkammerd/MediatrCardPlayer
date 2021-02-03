using CardPlayer.Web.Controllers;
using CardPlayer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace CardPlayer.Test
{
    public class HttpGetControllerShould
    {
        private readonly ITestOutputHelper _output;

        private HomeController DebugController { get; set; } = new HomeController();
        private string ActualViewName { get; set; }

        public HttpGetControllerShould(ITestOutputHelper output)
        {
            _output = output;
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
            //var ErrorExists = htmlView.Contains("SelectGame");
            if (ErrorExists)
                _output.WriteLine($"HTML output for the {viewName} view contains an error.");
            return ErrorExists;
        }

        [Fact]
        [Trait("Category", "CoreCandidate")]
        public async void NotThrowExceptions()
        {
            _ = await DebugController.Index();
            _ = await DebugController.SelectGame();
            _ = DebugController.JoinGame();

            Assert.True(true, "An exception occurred");
        }

        [Fact]
        [Trait("Category", "CoreCandidate")]
        public async void ReturnCorrectActionResults()
        {
            ViewResult IndexResult = (ViewResult)
                (await DebugController.Index());
            ViewResult SelectGameResult = (ViewResult)
                (await DebugController.SelectGame());
            ViewResult JoinGameResult = (ViewResult)
                (DebugController.JoinGame());

            Assert.IsType<ViewResult>(IndexResult);
            Assert.IsType<FoyerViewModel>(IndexResult.Model);
            ActualViewName = IndexResult.ViewName ?? nameof(DebugController.Index);
            Assert.False(HtmlContainsErrorOutput(
                ActualViewName, RenderRazorViewToString(ActualViewName, IndexResult.Model)));

            Assert.IsType<ViewResult>(SelectGameResult);
            Assert.IsType<FoyerViewModel>(SelectGameResult.Model);
            ActualViewName = SelectGameResult.ViewName ?? nameof(DebugController.SelectGame);
            Assert.False(HtmlContainsErrorOutput(
                ActualViewName, RenderRazorViewToString(ActualViewName, SelectGameResult.Model)));

            Assert.IsType<ViewResult>(JoinGameResult);
            Assert.IsType<FoyerViewModel>(JoinGameResult.Model);
            ActualViewName = JoinGameResult.ViewName ?? nameof(DebugController.JoinGame);
            Assert.False(HtmlContainsErrorOutput(
                ActualViewName, RenderRazorViewToString(ActualViewName, JoinGameResult.Model)));
        }
    }
}