using KoobecaFeedController;
using KoobecaFeedController.BL;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaSocketServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class feedController
    {
        private static string _Cache = null;

        [HttpGet]
        public FeedResponse indexActoin(uint user_id,int minutes_back = 24*60,bool serialize = true,bool cache = false)
        {
            throw new NotImplementedException();
            //RequestParams request = new RequestParams() { viewerId = user_id };
            //if (cache)
            //{
            //    if(_Cache == null)
            //    {
            //        _Cache = JsonConvert.SerializeObject(FeedController.Instance.GetClientFeed(user_id, minutes_back));
            //    }
            //    return new FeedResponse() { cached_data = _Cache };
            //}
            //var ret = FeedController.Instance.GetClientFeed(user_id, minutes_back);
            //if (serialize)
            //{
            //    return new FeedResponse() { data = ret };
            //}
            //else
            //{
            //    return new FeedResponse();
            //}
        }
    }

    public class FeedResponse
    {
        public FeedWrapper[] data;
        public string cached_data;
    }
}
