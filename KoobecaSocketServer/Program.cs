using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading;
using KoobecaFeedController;
using KoobecaFeedController.BL;
using KoobecaFeedController.BL.Request;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KoobecaFeedController.DAL.Adapters;
using System.IO;

namespace KoobecaSocketServer {
    class Program {
        private static SocketServer Server;
        public static bool NonInteractive { get; set; }
        public static bool Development { get; set; }
        static void Main(string[] args) {
            if (args.Contains("q")) {
                NonInteractive = true;
            }

            if (args.Contains("d")) {
                Development = true;
            }

            Server = new SocketServer(Development ? 8766 : 8765);
            
            var env = ConfigurationManager.AppSettings["env"];
            Console.WriteLine("Starting server {0}", env);
            Server.Request += Server_Request;
            Server.Run();
            //also start listening to http clients
            StartWeb();
            try
            {
                CoreSettings.Set("ListLikeDecreaseIteration", DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss")); //that's just to make sure we dont decrease the likes
                AffinityManager.Instance.Start();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e.ToString());
            }
            if (NonInteractive) {
                Thread.Sleep(Timeout.Infinite);
            }
            else {
                do {
                    Console.WriteLine("Press q to stop listening.");
                } while (Console.ReadKey().KeyChar != 'q');
            }

            Server.Request -= Server_Request;
            Server.Dispose();
        }

        private static void Server_Request(object sender, RequestEventArgs e) {
            try {

              //  e.Stream.Write(File.ReadAllText(@"C:\Workspace\Koobeca\files\raw.txt").Substring(0,1024));
               // e.Stream.Write(File.ReadAllText(@"C:\Workspace\Koobeca\files\raw.txt").Substring(0, 1025));
                Logger.Instance.Debug(e.Request);
                var reqObj = (JObject)JsonConvert.DeserializeObject(e.Request);
                var userId = (uint)3;// = (uint)reqObj["user_id"];
                var cmd = (string)reqObj["cmd"];
                Logger.Instance.Debug($"command = {cmd}");
                var extraData = reqObj.ContainsKey("data") ? JsonConvert.DeserializeObject<Dictionary<string, object>>((string)reqObj["data"]) : new Dictionary<string, object>();
                RequestParams reqParams = new RequestParams();
                try
                {
                    reqParams = JsonConvert.DeserializeObject<AllData>((string)reqObj["all_data"]).getRequestAllParams;
                }
                catch(Exception ex) 
                {
                    Logger.Instance.Warn("faild to parse all_data");
                    Logger.Instance.Debug(ex.ToString());
                }
                //var allParams = JsonConvert.DeserializeObject<AllData>((string)reqObj["all_data"]);
                var result = RequestHandler.Handle(cmd, userId,reqParams, extraData);
                e.Stream.Write(result);

            }
            catch (Exception exc) {
                Logger.Instance.Error(exc.ToString());
                e.Stream.Write(exc.ToString());
            }
        }



        private static void StartWeb() {
            var port = Development ? 7777 : 7778;
            var host = WebHost
                      .CreateDefaultBuilder()
                      .UseKestrel()
                      .UseStartup<WebStartup>()
                      .UseUrls($"http://*:{port}")
                      .Build();
            host.Start();
        }
    }
}
