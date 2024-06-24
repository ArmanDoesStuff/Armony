using System;
using System.Collections.Generic;
using Armony.Utilities.Libraries;
using Armony.Utilities.Singleton;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPool : NetworkBehaviour
    {
        private INetworkPoolable[] PooledObjects { get; set; }
        private int CurrentIndex { get; set; }

        public void Construct(int poolCapacity)
        {
            PooledObjects = new INetworkPoolable[poolCapacity];
        }

        public int IncrementIndex()
        {
            CurrentIndex = (CurrentIndex + 1) % PooledObjects.Length;
            return CurrentIndex;
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

        public void ReleasePooledObject(int index)
        {
            ReleaseLocal(index);
            ReleasePooledObjectServerRpc(index);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReleasePooledObjectServerRpc(int index, ServerRpcParams rpcParams = default) =>
            ReleasePooledObjectClientRpc(index, LibServer.SendExceptCaller(rpcParams));

        [ClientRpc]
        private void ReleasePooledObjectClientRpc(int index, ClientRpcParams rpcParams)
        {
            if (index >= PooledObjects.Length || PooledObjects[index] == null) return;
            ReleaseLocal(index);
        }

        private void ReleaseLocal(int index) => PooledObjects[index].Release();
    }
}