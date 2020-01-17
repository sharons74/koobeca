using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PhotoController {
        [HttpGet]
        public AlbumPhoto Get(uint photoId) {
            ///
            return AlbumPhotos.GetById(photoId);
        }
    }
}