using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaSocketServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class activityController
    {
        [HttpGet]
        public ActivityAction Get(uint action_id)
        {
            ///
            return ActivityActions.GetById(action_id,true);
        }
    }
}
