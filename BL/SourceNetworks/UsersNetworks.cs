using KoobecaFeedController.BL.SourceNetworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL {
    public class UsersNetworks {
        private readonly object _lock = new object();
        private readonly Dictionary<ulong, NetworkMembers> _map = new Dictionary<ulong, NetworkMembers>();
        private readonly Dictionary<ulong, FollowedSources> _followMap = new Dictionary<ulong, FollowedSources>();
        private readonly Dictionary<ulong, LikedSources> _likedMap = new Dictionary<ulong, LikedSources>();

        private UsersNetworks() { }
        public static UsersNetworks Instance { get; } = new UsersNetworks();

        public NetworkMembers GetUserNetwork(ulong userId) {
            NetworkMembers network;

            lock (_lock) {
                if (!_map.ContainsKey(userId)) {
                    network = new NetworkMembers(userId);
                    _map[userId] = network;
                }
                else {
                    network = _map[userId];
                }
            }

            return network;
        }

        public IEnumerable<NetworkMember> GetMembers(ulong userId, byte affinityStart, byte affinityEnd) {
            return GetUserNetwork(userId).FilterMembers(affinityStart, affinityEnd);
        }

        

        public IEnumerable<NetworkMember> GetAllMembers(ulong userId) {
            return GetMembers(userId, 0, 100);
        }

        public FollowedSources GetFollowedSources(ulong userId)
        {
            FollowedSources sources;

            lock (_lock)
            {
                if (!_followMap.ContainsKey(userId))
                {
                    sources = new FollowedSources(userId);
                    _followMap[userId] = sources;
                }
                else
                {
                    sources = _followMap[userId];
                }
            }
            return sources;
        }

        public LikedSources GetLikedSources(uint userId)
        {
            LikedSources sources;

            lock (_lock)
            {
                if (!_likedMap.ContainsKey(userId))
                {
                    sources = new LikedSources(userId);
                    _likedMap[userId] = sources;
                }
                else
                {
                    sources = _likedMap[userId];
                }
            }
            return sources;
        }

        public void RemoveRelationShip(ulong memberA, ulong memberB) {
            GetUserNetwork(memberA).RemoveMember(memberB);
            GetUserNetwork(memberB).RemoveMember(memberA);
        }

        public void AddRelationShip(ulong memberA, ulong memberB, int affinity) {
            if (affinity == -1) affinity = GlobalSettings.InitialMemberAffinity;
            GetUserNetwork(memberA).AddOrSetMember(memberB, (byte)affinity);
            GetUserNetwork(memberB).AddOrSetMember(memberA, (byte)affinity);
        }

        public Dictionary<ulong, List<NetworkMember>> GetUpdatedNetworkMembers(bool clearUpdate = false) {
            var result = new Dictionary<ulong, List<NetworkMember>>();

            lock (_lock) {
                foreach (var pair in _map) {
                    var updates = pair.Value.GetUpdatedMembers();
                    if (updates != null && updates.Count > 0) {
                        result[pair.Key] = updates;
                        if (clearUpdate) pair.Value.ClearUpdatedMembers();
                    }
                }
            }

            return result;
        }
    }
}