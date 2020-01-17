using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using KoobecaFeedController;
using KoobecaFeedController.BL;
using KoobecaFeedController.BL.Handlers;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KoobecaSocketServer
{
    public class RequestHandler
    {
        public static string Handle(string cmd, uint userId,RequestParams requestParams,Dictionary<string,object> additionalParams )
        {
            if (cmd == "dummy") return string.Empty;

            Logger.Instance.Debug($"User Id :{userId}");
            Logger.Instance.Debug($"Parameters :{JsonConvert.SerializeObject(requestParams)}");

            ActivityHandler handler = HandlerFactory.CreateHandler(cmd,userId, requestParams, additionalParams);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            handler.Execute();
            sw.Stop();
            Logger.Instance.Info($"execution of {cmd} took {sw.ElapsedMilliseconds} ms");
            return handler.Response;
        }
    }
}
