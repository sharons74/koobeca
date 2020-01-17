using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class RemoveRefHandler : ActivityHandler
    {
        protected ActivityType type;

        public RemoveRefHandler(uint userId,RequestParams reqParams, ActivityType type) : base(userId, reqParams)
        {
            this.type = type;
        }

        public override void Execute()
        {
            CheckIfRefNotExists(out ActivityAction refAction, out object refRecord);

            DecreaseCount();
            if (refAction != null)
            {
                RemoveActivityAction(refAction);
                RemoveActivityFromUserFeed(refAction);
            }
            else
            {
                Logger.Instance.Warn("no reference action to remove");
            }
            RemoveRefItemsFromSource();
            RemoveRecordFromTable(refRecord);
        }

       

        protected virtual void RemoveRecordFromTable(object refRecord)
        {}

        protected virtual void DecreaseCount()
        {}

        protected virtual void RemoveRefItemsFromSource()
        {}


        protected virtual void CheckIfRefNotExists(out ActivityAction refActivityAction, out object actionRecord)
        {
            throw new NotImplementedException();
        }

    }
}
