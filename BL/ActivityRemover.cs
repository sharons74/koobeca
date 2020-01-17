using KoobecaFeedController.BL.Handlers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KoobecaFeedController.BL
{
    public class ActivityRemover
    {
        public static ActivityRemover Instance { get; } = new ActivityRemover();

        private ActivityRemover() {

            Task.Run(()=> Consumer());
        }


        BlockingCollection<ulong> _activities = new BlockingCollection<ulong>();
        private Dictionary<ulong, int> _forRemoval = new Dictionary<ulong, int>();

        public void AddActivity(ulong actionId)
        {
            lock (_forRemoval)
            {
                if (!_forRemoval.ContainsKey(actionId))
                {
                    Logger.Instance.Debug($"activity {actionId} was added for removal");
                    _activities.Add(actionId);
                    _forRemoval[actionId] = 1;
                }
                else
                {
                    Logger.Instance.Debug($"activity {actionId} all ready exists for removal");
                }
            }
        }

        private Dictionary<ulong, int> _removed = new Dictionary<ulong, int>();

        private void Consumer()
        {
            
            foreach (var actionId in _activities.GetConsumingEnumerable())
            {
                try
                {
                    Logger.Instance.Debug($"Removing activity {actionId}");
                    FailedActivities.Add(actionId);
                    //var handler = HandlerFactory.CreateHandler("delete", 3, new Request.RequestParams() { action_id = actionId }, null);
                    //handler.Execute();
                }
                catch(Exception e)
                {
                    Logger.Instance.Error(e.Message);
                }
                Thread.Sleep(25);
            }

            Logger.Instance.Debug("ActivityRemover Consumer is finished.");
        }
    }
}