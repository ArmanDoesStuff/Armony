using System;
using System.Collections.Generic;
using Armony.Utilities.Singleton;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPool : NetworkBehaviour
    {
        private List<INetworkPoolable> PooledObjects { get; } = new();
        private Queue<int> AvailableIndexes { get; } = new();

        public void Construct(int poolCapacity)
        {
        }

        public int IncrementIndex()
        {
            if (AvailableIndexes.Count == 0)
            {
                AvailableIndexes.Enqueue(PooledObjects.Count);
                Debug.LogWarning("Adding to projectile queue", this);
            }
            return AvailableIndexes.Dequeue();
        }

        public T GetPooledObject<T>(T poolableType, Vector3 position, Quaternion rotation, int objectIndex)
            where T : INetworkPoolable
        {
            if (objectIndex >= PooledObjects.Count)
            {
                for (int i = PooledObjects.Count; i <= objectIndex; i++)
                {
                    PooledObjects.Add(poolableType.Initialize(transform).GetComponent<T>());
                    PooledObjects[i].Index = i;
                }
            }

            PooledObjects[objectIndex].Get(position, rotation);
            return (T)PooledObjects[objectIndex];
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReleasePooledObjectServerRpc(int index)
        {
            if (AvailableIndexes.Contains(index)) return;
            AvailableIndexes.Enqueue(index);
            ReleasePooledObjectClientRpc(index);
        }

        [ClientRpc]
        private void ReleasePooledObjectClientRpc(int index)
        {
            if (PooledObjects.Count >= index && PooledObjects[index] != null)
            {
                PooledObjects[index].Release();
            }
        }
    }
}