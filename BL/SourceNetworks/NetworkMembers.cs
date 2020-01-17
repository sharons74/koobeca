using System;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL {
    public class NetworkMembers {
        private readonly object _lock = new object();

        private readonly ulong _sourceId;
        private Dictionary<ulong, NetworkMember> _membersMap;

        private List<NetworkMember> _updatedMembers = new List<NetworkMember>();

        public NetworkMembers(ulong sourceId) {
            _sourceId = sourceId;
        }

       

        public IEnumerable<NetworkMember> Members => Map.Values.AsEnumerable();

        private Dictionary<ulong, NetworkMember> Map {
            get {
                lock (_lock) {
                    if (_membersMap == null) {
                        var members = DbAccess.Instance.GetNetworkMembers(_sourceId);
                        _membersMap = new Dictionary<ulong, NetworkMember>();
                        members.ForEach(m => _membersMap[m.MemberId] = m);
                    }
                    return _membersMap;
                }
            }
        }

        public NetworkMember GetMember(ulong memberId) {
            Map.TryGetValue(memberId, out var member);
            return member;
        }

        public void AddUpdatedMember(NetworkMember member) {
            _updatedMembers.Add(member);
        }

        public void ClearUpdatedMembers() {
            _updatedMembers = new List<NetworkMember>();
        }

        public void AddOrSetMember(ulong memberId, byte affinity) {
            lock (_lock) {
                if (!Map.ContainsKey(memberId)) {
                    var newMember = new NetworkMember {
                        AffinityLevel = affinity,
                        MemberId = memberId
                    };
                    Map[memberId] = newMember;
                }
                else {
                    Map[memberId].AffinityLevel = affinity;
                }
            }
        }

        /// <summary>
        ///     Return subset of members with range of affinity
        /// </summary>
        /// <param name="lowAffinity"></param>
        /// <param name="highAffinity"></param>
        /// <returns></returns>
        public IEnumerable<NetworkMember> FilterMembers(byte lowAffinity, byte highAffinity) {
            return Members.Where(m => m.AffinityLevel >= lowAffinity && m.AffinityLevel <= highAffinity);
        }

        /// <summary>
        ///     Return subset of members with range of affinity
        /// </summary>
        /// <param name="lowAffinity"></param>
        /// <param name="highAffinity"></param>
        /// <returns></returns>
        public IEnumerable<NetworkMember> GetMembers(NetworkMemberFilter filter)
        {
            return Members.Where(m=>m.MemberType == filter.MemberType);
            //TODO implement getting members by filter Members.Where(m => m.AffinityLevel >= lowAffinity && m.AffinityLevel <= highAffinity);
        }

        public List<NetworkMember> GetUpdatedMembers() {
            return _updatedMembers;
        }

        internal void RemoveMember(ulong memberId) {
            lock (_lock) {
                if (!Map.ContainsKey(memberId)) Map.Remove(memberId);
            }
        }
    }
}