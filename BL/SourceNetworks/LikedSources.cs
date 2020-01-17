using KoobecaFeedController.DAL.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.SourceNetworks
{
    public class LikedSources
    {
        private ulong userId;
        private List<NetworkMember> _sourceList;
        private object _lock = new object();

        public LikedSources(ulong userId)
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
                        _sourceList = SourceAffinities.GetForUser(userId).Select(s => new NetworkMember() { MemberId = s.source_id, Type = s.source_type,AffinityLevel = s.affinity }).ToList();
                    }
                }
                return _sourceList;
            }
        }

        public NetworkMember GetSource(ulong sourceId, string sourceType)
        {
            return List.FirstOrDefault(m => m.MemberId == sourceId && m.Type == sourceType);
        }

        public void AddSource(ulong sourceId, string sourceType,byte affinty)
        {
            lock (_lock)
            {
                RemoveSource(sourceId, sourceType);//remove it if exists just for making sure
                List.Add(new NetworkMember() { MemberId = sourceId, Type = sourceType ,AffinityLevel = affinty});
            }
        }

        public void RemoveSource(ulong sourceId, string sourceType)
        {
            lock (_lock)
            {
                var item = List.FirstOrDefault(m => m.MemberId == sourceId && m.Type == sourceType);
                if (item != null)
                {
                    Logger.Instance.Debug($"removing {sourceType}:{sourceId} from list");
                    List.Remove(item);
                }
            }
        }

        public  bool SourceExists(uint sourceId, string sourceType)
        {
            return GetSource(sourceId, sourceType) != null;
        }

        
    }
}
