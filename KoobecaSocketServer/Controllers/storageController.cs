using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoobecaSocketServer.Controllers {
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StorageController {
        [HttpGet]
        public Storage Get(uint fileId) {
            ///
            return Storages.GetByFileId(fileId);
        }
    }
}