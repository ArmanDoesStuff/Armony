using System;
using System.Collections.Generic;
using Armony.Utilities.Singleton;
using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPoolManager : NetworkBehaviour
    {
        private static NetworkPoolManager Instance => SingletonManager.GetInstance<NetworkPoolManager>();
        private static Dictionary<string, (NetworkPoolable[] networkPool, int lastIndex)> NetworkPools { get; } = new();
        private const int PoolSize = 5;
        
        public static int GetPooledObjectIndex<T>(T poolableType)
            where T : NetworkPoolable
        {
            string guid = GetPrefabId(poolableType);
            if (!NetworkPools.ContainsKey(guid))
            {
                NetworkPools.Add(guid, new ValueTuple<NetworkPoolable[], int>(new NetworkPoolable[PoolSize], 0));
            }

            (NetworkPoolable[] networkPool, int lastIndex) networkPool = NetworkPools[guid];
            int index = networkPool.lastIndex;
            networkPool.lastIndex = (networkPool.lastIndex + 1) % PoolSize;
            NetworkPools[guid] = networkPool;
            return index;
        }

        public static T GetPooledObject<T>(T poolableType, Vector3 position, Quaternion rotation, int objectIndex)
            where T : NetworkPoolable
        {
            string guid = GetPrefabId(poolableType);
            if (!NetworkPools.ContainsKey(guid))
            {
                NetworkPools.Add(guid, new ValueTuple<NetworkPoolable[], int>(new NetworkPoolable[PoolSize], 0));
            }

            (NetworkPoolable[] networkPool, int lastIndex) pool = NetworkPools[guid];
            if (pool.networkPool[objectIndex] == null)
            {
                pool.networkPool[objectIndex] = Instantiate(poolableType, parent: Instance.transform);
                pool.networkPool[objectIndex].Guid = guid;
                pool.networkPool[objectIndex].Index = objectIndex;
            }

            Transform poolableTransform = pool.networkPool[objectIndex].transform;
            poolableTransform.position = position;
            poolableTransform.rotation = rotation;

            pool.networkPool[objectIndex].Get();
            return (T)pool.networkPool[objectIndex];
        }

        public static void ReleasePooledObject(string guid, int index) => Instance.ReleasePooledObjectServerRpc(guid, index);

        [ServerRpc(RequireOwnership = false)]
        private void ReleasePooledObjectServerRpc(string guid, int index)
        {
            ReleasePooledObjectClientRpc(guid, index);
        }

        [ClientRpc]
        private void ReleasePooledObjectClientRpc(string guid, int index)
        {
            if (!NetworkPools.TryGetValue(guid, out (NetworkPoolable[] networkPool, int lastIndex) pool)) return;
            if (pool.networkPool[index] != null)
            {
                pool.networkPool[index].Release();
            }
        }

        private static string GetPrefabId(NetworkPoolable poolable)
        {
            return poolable.GetType().Name;
        }
    }
}