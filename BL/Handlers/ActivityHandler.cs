using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class ActivityHandler
    {
        protected uint userId;
        protected Activity activity;
        protected SourceData userSource;
        protected RequestParams reqParams;
        protected ushort authorization;

        public ActivityHandler(uint userId,RequestParams reqParams)
        {
            this.reqParams = reqParams;
            reqParams.viewerId = userId;
            this.userId = userId;
            if (reqParams.action_id != 0)
            {
                this.activity = ActivityCache.Instance.Get(reqParams.action_id);
                if (activity == null)
                    throw new Exception($"Didn't find activity {reqParams.action_id}");
            }
            userSource = SourcesMap.Instance.GetSource(userId, "user");

            authorization = activity?.GetAuthorization(userId, "user")??0;

            if (!IsAuthorized)
            {
                throw new UnauthorizedAccessException($"user {userId} is not authorized for this action");
            }
        }

        public string Response { get; set; } = string.Empty;
        

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        #region Activity Methods

        protected virtual bool IsAuthorized {
            get
            { 
                return true;
            }
        }
       
        protected Activity CreateRefActivity(ActivityType type)
        {
            string typeStr = type.ToString().ToLower();
            if (type != ActivityType.Share) typeStr += "_activity_action";
            ActivityAction refAction = ActivityAction.CreateReferenceAction(activity.RawActivity,typeStr, userId);
            refAction.body = GetRefBody();
            //add it to the DB & Cache
            ActivityActions.Add(refAction, true);
            var newActivity = new Activity(refAction);
            ActivityCache.Instance.Set(newActivity);
            return newActivity;
        }

        
        
        protected void RemoveActivityFromUserFeed(ActivityAction activity,bool deleteAll = false)
        {   
            userSource.ActivityFeed.RemoveActivity(activity.action_id, deleteAll);
        }

        protected virtual void RemoveActivityAction(ActivityAction action)
        {
            Activity activity = new Activity(action);
            activity.Source.ActivityFeed.RemoveActivity(activity,true); //remove it from the feeds
            ActivityCache.Instance.Remove(action.action_id); 
            ActivityActions.Delete(action.action_id, deleteAttachments: true);
            HiddenActivities.RemoveForActivity(action.action_id);
            ActivityNotifications.RemoveForActivity(action.action_id);
        }
        
        protected void SetStatistics()
        {
            //throw new NotImplementedException();
        }

        protected virtual void SendNotifications()
        {
            return; //currently do nothing because notifications are made outside

            var notification = CreateNotification();
            if(notification.subject_id == notification.user_id)
            {
                Logger.Instance.Debug($"user:{userId} will not be notified on his own actions");
                return;
            }
            //Logger.Instance.Debug($"sending notification:{JsonConvert.SerializeObject(notification)}");
            ActivityNotifications.Add(notification);
        }

        protected virtual ActivityNotification CreateNotification()
        {
            ActivityNotification notification = new ActivityNotification();
            notification.user_id = (uint)activity.SubjectId;
            notification.subject_type = "user";// refActivity.subject_type;
            notification.subject_id = userId;// refActivity.subject_id;
            notification.object_type = "activity_action";
            notification.object_id = (uint)activity.ActivityId;
            notification.type = "";
            notification.@params = "{\"label\":\"TYPE\"}"; ;
            notification.date = DateTime.Now;
            return notification;
        }

        protected void UpdateActionInDB(ActivityAction rawActivity)
        {
            ActivityActions.Update(rawActivity);
        }

        protected virtual byte[] GetRefBody()
        {
            return string.IsNullOrEmpty(reqParams.body)?new byte[0]:Encoding.UTF8.GetBytes(reqParams.body);
        }

        protected void LikeSourceIfPossible(ulong sourceId,string sourceType)
        {
            try
            {

                var likedSource = UsersNetworks.Instance.GetLikedSources(userId).GetSource(sourceId, sourceType);
                if (likedSource == null)// && (sourceType == "sitepage_page" || sourceType == "sitegroup_group"))
                {
                    //set affinity to this page/group/user
                    Logger.Instance.Debug($"start liking {sourceType} {sourceId}");
                    byte affinity = 50;
                    SourceAffinities.Add(new SourceAffinity()
                    {
                        affinity = affinity,//TODO manage affinity in the future
                        user_id = this.userId,
                        source_id = sourceId,
                        source_type = sourceType,
                        date = DateTime.Now
                    });
                    UsersNetworks.Instance.GetLikedSources(userId).AddSource(sourceId, sourceType, affinity);
                }
                else
                {
                    //increase existing affinity up to 100
                    if (likedSource.AffinityLevel < 50)
                    {
                        likedSource.AffinityLevel = 50; //give it a new chance
                    }
                    else if (likedSource.AffinityLevel > 55) // that mean we have a history of liking this
                    {
                        likedSource.AffinityLevel += 10;
                    }
                    else
                    {
                        likedSource.AffinityLevel++;
                    }

                    //fix it on max 100
                    if (likedSource.AffinityLevel > 100)
                    {
                        likedSource.AffinityLevel = 100;
                    }
                    //update the db
                    SourceAffinities.UpdateAffinity(userId, (uint)sourceId, sourceType, likedSource.AffinityLevel);
                }
            }
            catch(Exception e)
            {
                Logger.Instance.Error($"Failed to like source:{e.Message}");
                Logger.Instance.Debug(e.ToString());
            }
        }

       

        #endregion
    }
}
