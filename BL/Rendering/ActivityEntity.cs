using System;
using System.Web;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL {
    public class ActivityEntity {
        protected Activity _referenceActivity;
        protected Activity _object;
        private IActivityObject _DalObject;
        private Storage _Storage;
        private readonly EntityType _entityType;
        private ulong _viewerId;

        public ActivityEntity() { }

        public ActivityEntity(Activity activity, EntityType entityType, ulong viewerId) {
            _referenceActivity = activity;
            _entityType = entityType;
            _viewerId = viewerId;

            if (entityType == EntityType.Subject) {
                if (SubjectIsOwner(activity) && activity.IsPrimary && (activity.ObjectType == "sitepage_page" || activity.ObjectType == "sitegroup_group"))
                {
                    Type = activity.ObjectType;
                    Id = activity.ObjectId;
                }
                else
                {
                    Type = activity.SubjectType;
                    Id = activity.SubjectId;
                }
            }

            if (entityType == EntityType.Owner) {
                Type = activity.OwnerType;
                Id = activity.OwnerId;
            }
            else if (entityType == EntityType.Object) {
                if (activity.ObjectType == "activity_action") {
                    //that mean that we need to get the subject of the referred action
                    var refActivity = activity.InnerActivity;
                    if (SubjectIsOwner(refActivity)) {
                        //in case the subject is owner 
                        Type = refActivity.ObjectType;
                        Id = refActivity.ObjectId;
                    }
                    else {
                        Type = refActivity.SubjectType;
                        Id = refActivity.SubjectId;
                    }
                }
                else {
                    Type = activity.ObjectType;
                    Id = activity.ObjectId;
                }
            }
        }

        private bool SubjectIsOwner(Activity activity) {
            return activity.SubjectId == activity.OwnerId && activity.SubjectType == activity.OwnerType;
        }

        

        protected IActivityObject GetDalObject(Activity activity) {
                //Logger.Instance.Debug($"getting dal object for type {Type} with Id {Id}");
                switch (Type) {
                    case "user":
                        return Users.GetById((uint) Id);
                    case "sitegroup_group":
                        return Groups.GetById((uint) Id);
                    case "sitepage_page":
                        return Pages.GetById((uint) Id);
                    case "album_photo":
                        return AlbumPhotos.GetById((uint) Id);
                    case "core_link":
                        return CoreLinks.GetById((uint) Id);
                    default:
                    //throw new Exception($"Unknown type {Type} for Dal Object for entity type {_entityType}");
                    return new DummeyActivityObject();
                }
            
        }

        public string Type { get; set; }

        public ulong Id { get; set; }

        public string Label {
            get {
                if(Type == "user" && Id == _viewerId)
                {
                    return "You";
                }
                return HttpUtility.HtmlDecode(DalObject.GetObjectTitle());
            }
        }

        

        protected IActivityObject DalObject {
            get {
                if (_DalObject == null) _DalObject = GetDalObject(_referenceActivity);
                return _DalObject;
            }
        }

        public string Url => Storage != null ? ServiceUrlPrefixes.CouldFront + Storage.storage_path : null;

        protected Storage Storage {
            get {
                if (_Storage == null && DalObject.GetStorageFileId() != 0)
                    _Storage = Storages.GetByFileId(DalObject.GetStorageFileId());
                return _Storage;
            }
        }

        public bool IsViewer {
            get {
                return Label == "You";
            }
        }
    }

    public enum EntityType {
        Subject,
        Object,
        Owner
    }
}