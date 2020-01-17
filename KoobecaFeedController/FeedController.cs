using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.BL;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController {
    public class FeedController {
        private bool _inited;
        private Exception _lastException;
        private readonly OpreationBank _operationBank = new OpreationBank();
        private DateTime _lastExceptionTime;
        private readonly object _Lock = new object();

        public static FeedController Instance { get; } = new FeedController();

        public int ErrorCount { get; private set; }


        //public void InitIfNeeded() {
        //    lock (_Lock) {
        //        if (!_inited) {
        //            DbAccess.Instance.InitialDBRead();
        //            ItemFeed.Instance.Init();
        //            _inited = true;
        //        }
        //    }
        //}


        private CombinedActivity[] GetCombinedActivityFeed(ulong userId, int minutesBack = 0, int maxCount = -1) {
            try {

                var source = SourcesMap.Instance.GetSource(userId, "user");
                //update the activity
                source.LastActivity = LocalTime.CurrentIntSecond;

                return UsersNetworks.Instance.GetActivities(userId, 0, 100, minutesBack, maxCount, true).Take(50).ToArray();
                //.Where(a => a.Activities[a.FirstReactor].ActivityId == 10757).Take(50).ToArray();
            }
            catch (Exception e) {
                OnException(e);
                return null;
            }
        }

        public void SaveActionInBank(ulong actionId, ulong commentId = 0) {
            var activity = FindActivity(actionId, out var source);
            _operationBank.StoreActivityAction(activity.RawActivity, commentId);

        }

        public ActivityAction FetchActionFromBank(ulong actionId) {
            return _operationBank.FetchActivityAction(actionId);
        }

        public ActivityAction FetchCommentActionFromBank(ulong commentId) {
            return _operationBank.FetchActivityCommentAction(commentId);
        }


        public void AddReferenceActivity(uint userId, ulong actionId, ActivityType type) {
            var activity = FindActivity(actionId, out var activitySource);
            activity.AddReferenceActivity(userId, type);
            activitySource.ActivityFeed.AdvanceActivity(actionId);
            var refActivity = CreateReferenceActivity(activity, userId, type);
            var source = SourcesMap.Instance.GetSource(userId, "user");
            source.ActivityFeed.AddActivity(refActivity);
            Likes.Add(new Like() {poster_id = userId,poster_type = "user",reaction = "like",resource_id = (uint)actionId });
        }

        private Activity CreateReferenceActivity(Activity activity, ulong userId, ActivityType type)
        {
            throw new NotImplementedException();
        }

        public void RemoveLike(ulong userId, ulong actionId) {
            var activity = FindActivity(actionId, out var activitySource);

            if (activity.RemoveLike(userId)) {
                var removeRes = Likes.Remove((uint)userId, actionId);
                Logger.Instance.Debug($"remove like of user {userId} to activity {actionId} resulted in {removeRes}");
                //also remove the like activity from the user's feed
                var source = SourcesMap.Instance.GetSource(userId, "user");
                source.ActivityFeed.RemoveReactionTo(actionId);
            }
        }

        private Activity GetReferenceActivityOnCreation(Activity activity, ulong userId, string type) {
            Logger.Instance.Debug($"trying to find {type} to type {activity.RawActivity.type} object type {activity.ObjectType}:{activity.ObjectId}");

            var objectId = activity.ActivityId;
            var objectType = "activity_action";
            //fixing SEAO missing activities
            if (activity.ObjectType == "album_photo") {
                objectId = activity.ObjectId;
                objectType = activity.ObjectType;

                //if (type == "like")
                //{
                //    //add like to the table
                //    Like newLike = new Like() { poster_id = (uint)userId,poster_type = "user",reaction = "like",resource_id = (uint)activity.ActivityId};
                //    Likes.Add(newLike);
                //}
            }
            ActivityAction rawRefAction = null;
            if (type == "like") {
                rawRefAction = ActivityActions.GetLikeActivity(objectId, objectType, userId);
            }
            if (rawRefAction == null) {
                Logger.Instance.Warn($"Didn't find in db the {type} activity of {userId} to action {activity.ActivityId}");
            }
            else {

                //if (rawRefAction.object_type != "activity_action")
                //{
                //    Logger.Instance.Debug($"need to update the inner activity of the {type} action {rawRefAction.action_id} to {activity.ActivityId}, currently it is pointing to {rawRefAction.object_type}");
                //    ActivityActions.SetInnerActivity(rawRefAction.action_id, activity.ActivityId, "like");
                //    //fetch it again from DB
                //    rawRefAction = ActivityActions.GetByID(rawRefAction.action_id);

                //}
            }
            var refActivity = new Activity(rawRefAction);
            return refActivity;
        }

        public void UpdateAction(ulong actionId) {
            var rawActivity = ActivityActions.GetById((uint)actionId);
            var activity = FindActivity(actionId, out var source);
            activity.RawActivity = rawActivity;
            source.ActivityFeed.AdvanceActivity(actionId);
        }

        public void DeleteComment(ulong userId, ulong actionId, ulong commentId) {
            var activity = FindActivity(actionId, out var source);
            Console.WriteLine($"reducing comment count on {actionId}");
            activity.RawActivity.comment_count--;

            if (activity.IsShare) {
                Logger.Instance.Info("for comments on shares we stop here");
                return;
            }
            if (activity.CommentersList.Contains((uint)userId)) activity.CommentersList.Remove((uint)userId);
            var user = SourcesMap.Instance.GetSource(userId, "user");
            var commentAction = FetchCommentActionFromBank(commentId);
            if (commentAction == null) {
                Logger.Instance.Warn($"comment activity {commentId} was not found");
            }
            user.ActivityFeed.RemoveActivity(commentAction.action_id, true);
        }

        public Activity FindActivity(ulong actionId, out SourceData source, bool useBank = false, uint subjectId = 0, string subjectType = null) {
            if (subjectType == null) {
                source = GetSource(actionId, useBank);
            }
            else {
                source = SourcesMap.Instance.GetSource(subjectId, subjectType);
            }
            return source.ActivityFeed.FindActivity(actionId, true);
        }

        private SourceData GetSource(ulong actionId, bool useBank = false) {
            Logger.Instance.Debug($"Getting source for action {actionId} useBank={useBank}");
            ActivityAction rawAction = null;
            if (useBank) {
                rawAction = FetchActionFromBank(actionId);
            }
            else {
                Logger.Instance.Debug("Geiint from db");
                rawAction = ActivityActions.GetById((uint)actionId, false);
            }

            if (rawAction == null) {
                Logger.Instance.Info($"no ActivityAction {actionId} in DB or bank");
                return null;
            }
            ulong subjectId = rawAction.subject_id;
            var subjectType = rawAction.subject_type;
            return SourcesMap.Instance.GetSource(subjectId, subjectType);

        }

        public void AddComment(ulong userId, ulong actionId, ulong commentId, ActivityAction commentAction) {
            var activity = FindActivity(actionId, out var source);


            var user = SourcesMap.Instance.GetSource(userId, "user");

            Console.WriteLine($"Adding comment {commentId} to {actionId} by user {userId}");
            activity.RawActivity.comment_count++;
            source.ActivityFeed.AdvanceActivity(actionId);
            if (activity.IsShare) {
                Logger.Instance.Info("for comments on shares we do not create activity in user's feed");
                return;
            }
            if (!activity.CommentersList.Contains((uint)userId)) activity.CommentersList.Add((uint)userId);

            //add "comment" activity to the user's feed
            var commentActivity = user.ActivityFeed.AddActivity(commentAction.action_id, commentAction);
            if (commentAction.object_type != "activity_action") {
                Logger.Instance.Debug($"need to update the inner activity of the comment action {commentAction.action_id} to {actionId}, currently it is pointing to {commentAction.object_type}");
                ActivityActions.SetInnerActivity(commentAction.action_id, activity.ActivityId, "comment");
                if (commentActivity != null) {
                    //fetch it again from DB
                    commentAction = ActivityActions.GetById(commentAction.action_id);

                    commentActivity.RawActivity = commentAction;//put it in the activity
                    commentActivity.InnerActivity = activity;
                    Logger.Instance.Debug($"inner activity updated");
                }
            }
        }

        public void AddComment(ulong userId, ulong actionId) {
            var activity = FindActivity(actionId, out var activitySource);
            //we need to see that the user didn't like this item already
            if (activity.AddComment(userId)) {
                activitySource.ActivityFeed.AdvanceActivity(actionId);
                //need to find the comment activity and add it to the user's feed
                var commentActivity = GetReferenceActivityOnCreation(activity, userId, "comment");
                //add the comment activity to the feed
                var source = SourcesMap.Instance.GetSource(userId, "user");
                source.ActivityFeed.AddActivity(commentActivity);
            }
        }



        public string GetFeedForActionAsJson(ulong actionId, ulong forUser, string sourceType) {
            var activity = FindActivity(actionId, out var source);
            if (activity == null) {
                Logger.Instance.Warn($"didn't find item {actionId}");
                return null;
            }
            return JsonConvert.SerializeObject(new ResponseBody(new FeedWrapper[] { new FeedWrapper(activity, forUser, sourceType) }), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings {
                                NullValueHandling = NullValueHandling.Ignore
                            });


        }

        public string GetSubjectFeedAsJson(ulong subjectId, string subjectType, int count) {
            var source = SourcesMap.Instance.GetSource(subjectId, subjectType);
            if (source == null) {
                Logger.Instance.Error($"{subjectType}:{subjectId} not found in sources");
                return "";
            }
            var totalFeedCount = source.ActivityFeed.Feed.Count;
            var skip = 0;
            if (totalFeedCount > count) {
                skip = totalFeedCount - count;
            }
            var userId = subjectType == "user" ? subjectId : 0;
            return Serialize(source.ActivityFeed.Feed.Skip(skip).Reverse(), userId, subjectType);
        }

        private string Serialize(IEnumerable<Activity> activities, ulong forSource, string sourceType) {
            var feed = activities.Select(a => FeedWrapper.Create(a, forSource, sourceType)).Where(w => w != null).ToArray();
            return JsonConvert.SerializeObject(new ResponseBody(feed), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }



        public void CheckAndUpdateAction(ulong actionId) {
            //get the raw action
            var rawActivity = ActivityActions.GetById((uint)actionId);
            if (rawActivity == null) {
                Console.WriteLine($"action {actionId} was not found in DB");
                return;
            }

            var activity = FindActivity(actionId, out var source);
            activity.RawActivity = rawActivity;
            source.ActivityFeed.AdvanceActivity(actionId);
        }

        public ActivityAction[] GetActivityFeed(ulong userId, int minutesBack = 0, int maxCount = -1) {
            var combined = GetCombinedActivityFeed(userId, minutesBack, maxCount);
            return combined.Select(a => a.Activities[a.FirstReactor].RawActivity).ToArray();
        }

        public string GetClientFeedAsJson(ulong userId, int minutesBack = 0, int maxCount = -1) {
            var feed = GetCombinedActivityFeed(userId, minutesBack, maxCount)
                .Select(a => FeedWrapper.Create(a.Activities[a.FirstReactor], userId, "user")).Where(f => f != null);
            return JsonConvert.SerializeObject(new ResponseBody(feed.ToArray()), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }

        public FeedWrapper[] GetClientFeed(ulong userId, int minutesBack = 0, int maxCount = -1) {
            var feed = GetCombinedActivityFeed(userId, minutesBack, maxCount)
                .Select(a => new FeedWrapper(a.Activities[a.FirstReactor], userId, "user"));
            return feed.ToArray();
        }

        public IEnumerable<NetworkMember> GetUserSources(ulong userId) {
            try {
                return UsersNetworks.Instance.GetAllMembers(userId);
            }
            catch (Exception e) {
                OnException(e);
                return null;
            }
        }



        public void RemoveActivity(Activity activity) {
            try {
                if (activity.IsLikeOrReact) {
                    //handle like remove like
                    RemoveLike(activity.SubjectId, activity.InnerActivityId);
                }
                else {
                    var source = SourcesMap.Instance.GetSource(activity.SubjectId, activity.SubjectType);
                    source.ActivityFeed.RemoveActivity(activity.ActivityId, true);
                    var referencingUsers = activity.GetReferencingUsers();
                    foreach (var user in referencingUsers) {
                        var refSource = SourcesMap.Instance.GetSource(user, "user");
                        refSource.ActivityFeed.RemoveRecationActivitiesTo(activity.ActivityId);
                    }
                }
            }
            catch (Exception e) {
                OnException(e);
            }
        }



        public void RemoveReferencingActivities(Activity primaryActivity) {
            try {
                // add the activity to the user's activity feed
                var source = SourcesMap.Instance.GetSource(primaryActivity.EffectiveSubjectId, primaryActivity.EffectiveSubjetType);
                source.ActivityFeed.RemoveActivities(primaryActivity);

            }
            catch (Exception e) {
                OnException(e);
            }
        }



        public void AddRelationShip(ulong sourceId, ulong target, int affinity = -1) {
            try {
                UsersNetworks.Instance.AddRelationShip(sourceId, target, affinity);
            }
            catch (Exception e) {
                OnException(e);
            }
        }

        public void RemoveRelationShip(ulong sourceId, ulong targetId) {
            try {
                UsersNetworks.Instance.RemoveRelationShip(sourceId, targetId);
            }
            catch (Exception e) {
                OnException(e);
            }
        }

        private void OnException(Exception e) {
            ErrorCount++;
            _lastException = e;
            _lastExceptionTime = DateTime.Now;
        }

        public string GetLastException() {
            return _lastException != null ? $"on:{_lastExceptionTime} : {_lastException}" : string.Empty;
        }

    }

    internal class OpreationBank {
        private readonly Dictionary<ulong, ActivityActionContainer> _bankMap = new Dictionary<ulong, ActivityActionContainer>();
        private readonly Dictionary<ulong, ulong> _commentToActionMap = new Dictionary<ulong, ulong>();

        public void StoreActivityAction(ActivityAction rawActivity, ulong commentId = 0) {
            lock (this) {
                Logger.Instance.Debug($"Storing {rawActivity.action_id} in bank. commandId is {commentId}");
                _bankMap[rawActivity.action_id] = new ActivityActionContainer() { Activity = rawActivity };
                if (commentId != 0) _commentToActionMap[commentId] = rawActivity.action_id;
                if (_bankMap.Count > 100000) {
                    CleanOldStores();
                }
            }
        }

        public ActivityAction FetchActivityAction(ulong actionId) {
            lock (this) {
                Logger.Instance.Debug($"Looking for action {actionId} in the bank");

                var activity = _bankMap[actionId];
                _bankMap.Remove(actionId);
                if (activity != null) Logger.Instance.Debug($"Activity {activity.Activity.action_id} found and returned");
                return activity.Activity;
            }
        }

        public ActivityAction FetchActivityCommentAction(ulong commentId) {
            if (_commentToActionMap.ContainsKey(commentId)) {
                var actionId = _commentToActionMap[commentId];
                return FetchActivityAction(actionId);
            }
            else {
                Logger.Instance.Info($"comment {commentId} does not exist in the bank");
                return null;
            }
        }

        private void CleanOldStores() {
            //no reason that an item should be more than an hour in the bank
            var olds = _bankMap.Where(i => (DateTime.Now - i.Value.StoreTime).TotalMinutes > 60).Select(i => new { i.Key, i.Value.CommentId }).ToList();
            olds.ForEach(pair => { _bankMap.Remove(pair.Key); _commentToActionMap.Remove(pair.CommentId); });
        }

        internal class ActivityActionContainer {
            public ActivityAction Activity;
            public ulong CommentId;
            public DateTime StoreTime = DateTime.Now;
        }
    }
}