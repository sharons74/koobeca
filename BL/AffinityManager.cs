using KoobecaFeedController.DAL.Adapters;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KoobecaFeedController.BL {
    public class AffinityManager {
        private AffinityManager() { }
        public static AffinityManager Instance { get; } = new AffinityManager();

        private Task _PeriodicLikeDecreaseTask = null;

        public void Start()
        {
            lock (this)
            {
                if (_PeriodicLikeDecreaseTask != null)
                {
                    Logger.Instance.Debug("Affinity Manager is already started");
                    return;
                }
                _PeriodicLikeDecreaseTask = Task.Run(() => PeriodicLikeDecrease());
            }
        }

        private void PeriodicLikeDecrease()
        {
            Random r = new Random();
            while (true) //TODO use cancelation token
            {
                try
                {
                    //see if it is time to make a decrease
                    DateTime lastDecrease = CoreSettings.Get("LastLikeDecreaseIteration") == null?DateTime.MinValue:DateTime.Parse(CoreSettings.Get("ListLikeDecreaseIteration"));
                    Logger.Instance.Debug($"Lase decrease was at {lastDecrease}");
                    var span = DateTime.Now - lastDecrease;
                    Logger.Instance.Debug($"Time spent from last like decrease iteration is {span.TotalDays} days");
                    if (span.TotalDays < 100 && span.TotalDays > 1)
                    {
                        //need to do decrease
                        CoreSettings.Set("LastLikeDecreaseIteration", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);
                        //Logger.Instance.Debug($"doing like decrease iteration");
                        //var count  = SourceAffinities.DecreasAllLikes();
                        //Logger.Instance.Debug($"Decreased likes in {count} records");
                        Logger.Instance.Debug($"date is db set now to {CoreSettings.Get("LastLikeDecreaseIteration")} and now it is {DateTime.Now}");
                    }
                }
                catch (System.Exception e)
                {
                    Logger.Instance.Error($"Preriodic like decrease error ! {e.ToString()}");
                }

                int sleepTime = r.Next(1000,2000); //random number of secconds
                Thread.Sleep(sleepTime * 1000); 
            }
        }

    }
}