using Armony.Utilities.Libraries;
using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPool : NetworkBehaviour
    {
        private INetworkPoolUser NetworkPoolUser { get; set; }
        private INetworkPoolable[] PooledObjects { get; set; }
        private int CurrentIndex { get; set; }

        private Transform Holder { get; set; }

        public void Construct(int poolCapacity, INetworkPoolUser networkPoolUser)
        {
            if (PooledObjects != null)
            {
                foreach (INetworkPoolable poolable in PooledObjects)
                {
                    poolable?.Deinitialize();
                }
            }
            else
            {
                Holder = new GameObject("NetworkPool").transform;
            }

            NetworkPoolUser = networkPoolUser;
            PooledObjects = new INetworkPoolable[poolCapacity];
            Holder.parent = transform;
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
                PooledObjects[objectIndex] = poolableType.Initialize(Holder, NetworkPoolUser).GetComponent<T>();
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