using System;
using System.Collections.Generic;

namespace KoobecaFeedController.BL
{
    public class FailedActivities
    {
        private static Dictionary<ulong, bool> _failList = new Dictionary<ulong, bool>();

        public static void Add(ulong actionId)
        {
            _failList[actionId] = true;
        }

        public static bool Contains(ulong actionId)
        {
            return _failList.ContainsKey(actionId);
        }
    }
}