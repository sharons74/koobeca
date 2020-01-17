using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LikeController {
        [HttpGet]
        public List<uint> Get(uint objectId) {
            //return Likes.GetByResourceId(resourceId).ToArray();

           return ActivityActions
                                .GetByObjectId(objectId, "activity_action", "like_activity_action")
                                .Select(a => a.subject_id).ToList();
        }
    }
}