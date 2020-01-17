using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CoreLInkController {
        [HttpGet]
        public CoreLink Get(uint linkId) {
            ///
            return CoreLinks.GetById(linkId);
        }
    }
}