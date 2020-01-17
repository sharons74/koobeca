using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using KoobecaFeedController.BL.Handlers;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using DAL = KoobecaFeedController.DAL;

namespace KoobecaFeedController.BL {
    public class Activity {
        private Activity _innerActivity;
        private ulong _innerActivityId;
        private ulong? _innerActivitySourceId;
        private string _innerActivitySourceType;
        private ulong _ownerId;
        private string _ownerType;
        private List<uint> _commenters;
        private object _lock = new object();
        private Dictionary<string, ushort> _AuthorizationCache = new Dictionary<string, ushort>();
        private NotifiedUsers _notifiedUsers;
        private Privacy _privacy = Privacy.Undefined;
        private SourceData _Source;

        public byte Category; // 64 categories
        public ulong Languages = 1; // 64bit -> 64 languages , default english
        public byte Subcategory; // 64 Subcategories

        public uint ActivityTime; // the second relative to reference time that the activity was created;

        public Activity() { }

        public Activity(ActivityAction rawactivity) {
            RawActivity = rawactivity;
            ActivityId = rawactivity.action_id;
            Type = GetType(rawactivity.type);
            ActivityTime = LocalTime.ParseDate(rawactivity.date);
            //RealObjectId = GetObjectId(rawactivity);
        }

       

        [JsonIgnore]
        public List<uint> CommentersList {
            get {
                lock (this) {
                    if (_commenters == null) {
                        Console.WriteLine($"getting comments for {ActivityId} from the DB");

                        _commenters = ActivityActions
                            .GetByObjectId((uint) ActivityId, "activity_action", "comment_activity_action")
                            .Select(a => a.subject_id)
                            .ToList(); //TODO need to do distinct because user can comment more than once
                        Logger.Instance.Debug($"activity {ActivityId} has {_commenters.Count} commenters");
                    }
                }

                return _commenters;
            }
        }
        

        public ulong RealObjectId { get; set; }

        public ulong ActivityId { get; set; }

        [JsonIgnore]
        public Activity InnerActivity {
            get {
                if (_innerActivity == null && InnerActivityId != 0){
                        _innerActivity = ActivityCache.Instance.Get(InnerActivityId);
                }
                return _innerActivity;
            }
            set {
                _innerActivity = value;
                _innerActivityId = value.ActivityId;
            }
        }

        private readonly List<string> _selfDescribedActivities = new List<string> {"friends"};

        public bool ActivityIsDescribedInBody => _selfDescribedActivities.Contains(RawActivity.type);

        public string ObjectType {
            get {
                if (ObjectInAttachment)
                    return RawActivity.Attachments[0].type;
                return RawActivity.object_type;
            }
        }

        public string TemplateObjectType {
            get {
                var rawObjType = ObjectType;
                if (rawObjType.StartsWith("alb"))
                    rawObjType = "photo";
                else if (rawObjType.StartsWith("act")) rawObjType = "post";
                return rawObjType;
            }
        }

        private bool ObjectInAttachment =>
            RawActivity.Attachments.Count > 0 && RawActivity.object_type == RawActivity.subject_type &&
            RawActivity.type != "status" && RawActivity.type != "signup";

        public uint ObjectId {
            get {
                if (ObjectInAttachment)
                    return RawActivity.Attachments[0].id;
                return RawActivity.object_id;
            }
        }


        public bool IsSubjectTheOwner => RawActivity.subject_id == OwnerId && RawActivity.subject_type == OwnerType;

        public bool IsSubjectEqualObject => RawActivity.subject_id == RawActivity.object_id &&
                                            RawActivity.subject_type == RawActivity.object_type;

        public ulong SubjectId => RawActivity.subject_id;

        public string SubjectType => RawActivity.subject_type;

        public string EffectiveSubjetType {
            get {
                //if (ObjectType == "sitepage_page" && Pages.GetByID(ObjectId).owner_id == OwnerId)
                if (IsSubjectTheOwner && RawActivity.object_type != "activity_action") {
                    return RawActivity.object_type;
                    ;
                }

                return RawActivity.subject_type;
            }
        }

        public uint EffectiveSubjectId {
            get {
                //if (EffectiveSubjetType == ObjectType)
                if (IsSubjectTheOwner && RawActivity.object_type != "activity_action")
                    return RawActivity.object_id;
                return RawActivity.subject_id;
            }
        }

        [JsonIgnore]
        public ulong InnerActivityId {
            get {
                if (ObjectType == "activity_action") _innerActivityId = ObjectId;
                return _innerActivityId;
            }
            set => _innerActivityId = value;
        }

        

        public bool AddComment(ulong userId) {
            throw new NotImplementedException();
        }

        public ulong OwnerId {
            get {
                if (_ownerType == null) _ownerType = OwnerType; //initiate
                return _ownerId;
            }
        }

        internal bool HiddenFor(SourceData source)
        {
            if (source.HidenItems.IsHidden(ActivityId)) return true;
            //else
            if (InnerActivity != null && source.HidenItems.IsHidden(InnerActivity.ActivityId)) return true;

            return false;
        }

        public string OwnerType {
            get {
                if (_ownerType == null) {
                    if (RawActivity.ParamMap == null || !RawActivity.ParamMap.ContainsKey("owner")) {
                        if (RawActivity.object_type == "sitepage_page") {
                            //get it from the page
                            var page = Pages.GetById(RawActivity.object_id);
                            if (page == null) Logger.Instance.Error($"didn't find page {RawActivity.object_id}");
                            _ownerId = page.owner_id;
                            _ownerType = "user";
                        }
                        else if (RawActivity.object_type == "sitegroup_group") {
                            //get it from the page
                            var group = Groups.GetById(RawActivity.object_id);
                            if (group == null) Logger.Instance.Error($"didn't find group {RawActivity.object_id}");
                            _ownerId = group.owner_id;
                            _ownerType = "user";
                        }
                        else {
                            _ownerType = RawActivity.subject_type;
                            _ownerId = RawActivity.subject_id;
                        }
                    }
                    else {
                        //get both owner id and type
                        var pair = RawActivity.ParamMap["owner"].ToString().Split('_');
                        _ownerType = pair[0];
                        _ownerId = ulong.Parse(pair[1]);
                    }
                }

                return _ownerType;
            }
        }

        public string Reaction(ulong forUser) {
            if (IsPrimaryOrShare)
            {
                var source = SourcesMap.Instance.GetSource(forUser, "user");
                return source.LikedItems.Reaction("activity_action", (uint)ActivityId);
            }
            if (InnerActivity != null) return InnerActivity.Reaction(forUser);

            return null;
            
        }

        public ActivityType Type { get; set; }

        public bool IsPrimary => Type == ActivityType.Primary;

        public bool IsShare => Type == ActivityType.Share;

        public bool IsPrimaryOrShare => Type == ActivityType.Primary || Type == ActivityType.Share;

        public bool IsLikeOrReact => Type == ActivityType.Like || Type == ActivityType.React;

        public bool IsLikeCommentOrReact => IsLikeOrReact || Type == ActivityType.Comment;

        public NotifiedUsers NotifiedUsers
        {
            get
            {
                if(_notifiedUsers == null)
                {
                    _notifiedUsers = new NotifiedUsers(ActivityId);
                }
                return _notifiedUsers;
            }
        }

        public string GetSpecificActivityDescription() {
            if (RawActivity.type.Contains("post"))
                return RawActivity.BodyStr;
            return "";
        }


        [JsonIgnore]
        public ActivityAction RawActivity { get; set; }

        public uint AdditionalId { get; set; }

        public bool IsDefectedActivity { get; set; }

        public ushort GetAuthorization(ulong sourceId,string sourceType)
        {
            string key = $"{sourceType}|{sourceId}";
            if (_AuthorizationCache.ContainsKey(key))
                return _AuthorizationCache[key];
            //else
            ushort authorizationFlags = (ushort)AuthorizationFlags.Nothing;
            
            if ((sourceId == OwnerId && sourceType == OwnerType) || 
                (sourceId == RawActivity.subject_id && sourceType == RawActivity.subject_type) ||
                (sourceId == RawActivity.object_id && sourceType == RawActivity.object_type))
            {
                //if it is me I can do everything
                authorizationFlags = (ushort)AuthorizationFlags.All;
            }
            else if(Privacy == Privacy.OnlyMe)
            {
                //and it is not me
                //stay nothing
                //authorizationFlags = (ushort)AuthorizationFlags.Nothing;
            }
            else if (Privacy == Privacy.Friends)
            {
                //if it is a freind
                if(UsersNetworks.Instance.GetAllMembers(ConcreteActivity.OwnerId).Count(m => (m.MemberId == sourceId && m.MemberIs(ConcreteActivity.OwnerType))) > 0)
                {
                    authorizationFlags |= (ushort)(AuthorizationFlags.View | AuthorizationFlags.Like | AuthorizationFlags.Comment);
                }
            }
            else if (Privacy == Privacy.Everyone)
            {
                authorizationFlags |= (ushort)(AuthorizationFlags.View | AuthorizationFlags.Like | AuthorizationFlags.Comment | AuthorizationFlags.Share);
            }

            _AuthorizationCache[key] = authorizationFlags;
            return authorizationFlags;
        }

        public Activity Clone() {
            return (Activity) MemberwiseClone();
        }


        private ActivityType GetType(string type) {
            //TODO maybe use map to figure /primary/secondary activity types
            if (type.StartsWith("like")) return ActivityType.Like;
            if (type.StartsWith("comment")) return ActivityType.Comment;
            if (type.StartsWith("react")) return ActivityType.React;
            if (type.StartsWith("share")) return ActivityType.Share;
            //if (type.StartsWith("friends")) return ActivityType.Friends;

            return ActivityType.Primary;
        }

        private ulong GetObjectId(ActivityAction rawactivity) {
            if (IsPrimary && rawactivity.Attachments.Count > 0) return rawactivity.Attachments[0].id;

            return rawactivity.object_id;
        }

        Dictionary<string, int> _Reactions = null;

        public Dictionary<string, int> Reactions
        {
            get
            {
                if (_Reactions == null)
                {
                    lock (_lock)
                    {
                        if (_Reactions == null)
                            GetReactions();
                    }
                }
                return _Reactions;
            }
        }

        public Activity ConcreteActivity
        {
            get
            {
                if (!IsPrimaryOrShare && RawActivity.object_type == "activity_action")
                {
                    return InnerActivity;
                }
                //else
                return this;
            }
        }

        

        public Privacy Privacy {
            get {
                
                if (_privacy == Privacy.Undefined)
                {
                    lock (_lock)
                    {
                        if (_privacy == Privacy.Undefined)
                        {
                            if (IsLikeCommentOrReact && InnerActivity != null)
                            {
                                var innerPrevacy = InnerActivity.Privacy;
                                if (innerPrevacy == Privacy.Everyone) _privacy = Privacy.Friends;
                                else if (innerPrevacy == Privacy.Friends) _privacy = Privacy.OwnerFriends;
                                else if (innerPrevacy == Privacy.OnlyMe) _privacy = Privacy.NoOne;
                                else _privacy = Privacy.Unknown;
                            }
                            else if (IsShare && InnerActivity != null)
                            {
                                var innerPrevacy = InnerActivity.Privacy;
                                if (innerPrevacy != Privacy.Everyone) _privacy = Privacy.NoOne;
                                else _privacy = GetPrivacy(RawActivity);
                            }
                            else if (IsPrimary)
                            {
                                _privacy = GetPrivacy(RawActivity);
                            }
                        }
                    }
                }
                return _privacy;
            }
        }

        public DateTime ModifyDate
        {
            get {
                return RawActivity.modified_date;
            }
        }

        public DateTime CreationDate {
            get
            {
                return RawActivity.date;
            }
        }

        public SourceData Source {
            get {
                lock (_lock)
                {
                    if(_Source == null)
                    _Source = SourcesMap.Instance.GetSource(EffectiveSubjectId,EffectiveSubjetType);
                }
                return _Source;
            }
        }

        

        private Privacy GetPrivacy(ActivityAction rawActivity)
        {
            if (rawActivity.privacy == null && rawActivity.type.StartsWith("sitepage_")) return Privacy.Everyone;
            else if (string.IsNullOrEmpty(rawActivity.privacy)) return Privacy.Everyone; //TODO rethink this
            else if (rawActivity.privacy == "everyone") return Privacy.Everyone;
            else if (rawActivity.privacy == "friends") return Privacy.Friends;
            else if (rawActivity.privacy == "onlyme") return Privacy.OnlyMe;
            else return Privacy.NoOne;
        }

        private void GetReactions()
        {
            //TODO make this more efficient in DB
            _Reactions = new Dictionary<string, int>();
            DAL.Adapters.Reactions.GetAll().ForEach(r => _Reactions[r.type] = 0);

            var allReactions = Likes.GetByResourceId((uint)ActivityId);
            foreach(var group in allReactions.GroupBy(r => r.reaction)){
                _Reactions[group.Key] = group.Count();
            }
        }
    }

    public class NotifiedUsers
    {
        private Dictionary<ulong, bool> _items;
        private readonly ulong _actionId;
        private object _lock = new object();
        
        public NotifiedUsers(ulong actionId)
        {
            _actionId = actionId;
        }

        private Dictionary<ulong, bool> Items
        {
            get
            {
                lock (_lock)
                {
                    if (_items == null)
                    {
                        _items = new Dictionary<ulong, bool>();
                        foreach (var user in DAL.Adapters.NotifiedUsers.Get(_actionId))
                        {
                            Logger.Instance.Debug($"user {user} is nofified on {_actionId}");
                            _items[user] = true;
                        }
                    }
                }

                return _items;
            }
        }

        public bool IsNotified(ulong userId)
        {
            lock (_lock)
            {
                return Items.ContainsKey(userId);
            }
        }

        public bool Add(ulong userId)
        {
            lock (_lock)
            {
                return Items[userId] = true; ;
            }
        }

        public void Remove(ulong userId)
        {
            lock (_lock)
            {
                if (Items.ContainsKey(userId))
                    Items.Remove(userId);
            }
        }


    }
}