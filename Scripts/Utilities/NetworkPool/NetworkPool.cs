using System;
using System.Collections.Generic;
using Armony.Utilities.Singleton;
using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPool : NetworkBehaviour
    {
        private INetworkPoolable[] PooledObjects { get; set; }
        private int m_lastIndex;
        private int LastIndex
        {
            get => m_lastIndex;
            set => m_lastIndex = value % PooledObjects.Length;
        }

        public void Construct(int poolCapacity)
        {
            PooledObjects = new INetworkPoolable[poolCapacity];
            LastIndex = 0;
        }

        public int IncrementIndex()
        {
            int index = LastIndex;
            LastIndex++;
            return index;
        }

        public T GetPooledObject<T>(T poolableType, Vector3 position, Quaternion rotation, int objectIndex)
            where T : INetworkPoolable
        {
            if (PooledObjects[objectIndex] == null)
            {
                PooledObjects[objectIndex] = poolableType.Initialize(transform).GetComponent<T>();
                PooledObjects[objectIndex].Index = objectIndex;
            }

            PooledObjects[objectIndex].Get(position, rotation);
            return (T)PooledObjects[objectIndex];
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReleasePooledObjectServerRpc(int index)
        {
            ReleasePooledObjectClientRpc(index);
        }

        [ClientRpc]
        private void ReleasePooledObjectClientRpc(int index)
        {
            if (PooledObjects[index] != null)
            {
                PooledObjects[index].Release();
            }
        }
    }
}