using KoobecaFeedController.DAL.Adapters;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL
{
    public class FollowedSources
    {
        private ulong userId;
        private List<NetworkMember> _sourceList;
        private object _lock = new object();

        public FollowedSources(ulong userId)
        {
            this.userId = userId;
        }

        public List<NetworkMember> List
        {
            get
            {
                lock (_lock)
                {
                    if (_sourceList == null)
                    {
                        _sourceList = FollowedItems.Get((uint)userId).Select(s => new NetworkMember() { MemberId = s.resource_id, Type = s.resource_type }).ToList();
                    }
                }
                return _sourceList;
            }
        }

        public void AddSource(ulong sourceId,string sourceType)
        { 
            lock (_lock)
            {
                RemoveSource(sourceId, sourceType);//remove it if exists just for making sure
                List.Add(new NetworkMember() { MemberId = sourceId, Type = sourceType });
            }
        }

        public void RemoveSource(ulong sourceId, string sourceType)
        {
            lock (_lock)
            {
                var item = List.FirstOrDefault(m => m.MemberId == sourceId && m.Type == sourceType);
                if(item != null)
                {
                    Logger.Instance.Debug($"removing {sourceType}:{sourceId} from list");
                    List.Remove(item);
                }
            }
        }

        public bool SourceExists(ulong sourceId, string sourceType)
        {
            lock (_lock)
            {
                var item = List.FirstOrDefault(m => m.MemberId == sourceId && m.Type == sourceType);
                return (item != null);
            }
        }
    }
}