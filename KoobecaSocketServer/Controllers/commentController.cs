using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommentController {
        [HttpGet]
        public Comment Get(uint commentId) {
            return Comments.GetById(commentId);
        }
    }
}