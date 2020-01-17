using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class RemoveActivityHandler : ActivityHandler
    {
        public RemoveActivityHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {}

        public override void Execute()
        {  
            RemoveActivityFromUserFeed(activity.RawActivity,true);
            //remove from DB
            RemoveActivityAction(activity.RawActivity);
        }

        protected override void RemoveActivityAction(ActivityAction action)
        {
            base.RemoveActivityAction(action);
            Likes.RemoveForResource(action.action_id);
            Comments.RemoveForResource(action.action_id);
            RemoveRefActions(action);
        }
        

        private void RemoveRefActions(ActivityAction action)
        {
            var refActions = ActivityActions.GetByObjectId((uint)action.action_id,"activity_action");
            //ActivityActions.RemoveRefActivities(activity.ActivityId);
            if(refActions != null)
            {
                foreach(var refAction in refActions)
                {
                    RemoveActivityAction(refAction);
                }
            }
        }

        protected override bool IsAuthorized
        { 
            get
            {
                return (this.authorization & (ushort)AuthorizationFlags.Delete) > 0;
            }
        }
    
    }
}
