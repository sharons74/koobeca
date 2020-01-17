using KoobecaFeedController;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TestController {
        [HttpGet]
        public string TestAction() {
            //var res = KoobecaFeedController.FeedController.Instance.GetClientFeedAsJson(1, 60000);
            return "not implemented";// res.Length.ToString();
        }
    }
}