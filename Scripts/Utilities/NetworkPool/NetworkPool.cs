#if ARMONY_NETCODE
using System;
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

        public void Construct(int _poolCapacity, INetworkPoolUser _networkPoolUser)
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
                PooledObjects = new INetworkPoolable[_poolCapacity];
            }

            NetworkPoolUser = _networkPoolUser;
            Holder.parent = transform;
        }

        public int IncrementIndex()
        {
            CurrentIndex = (CurrentIndex + 1) % PooledObjects.Length;
            return CurrentIndex;
        }

        public T GetPooledObject<T>(T _poolableType, Vector3 _position, Quaternion _rotation, int _objectIndex)
            where T : INetworkPoolable
        {
            if (PooledObjects[_objectIndex] == null)
            {
                PooledObjects[_objectIndex] = _poolableType.Initialize(Holder, NetworkPoolUser).GetComponent<T>();
                PooledObjects[_objectIndex].Index = _objectIndex;
            }

            PooledObjects[_objectIndex].Get(_position, _rotation);
            return (T)PooledObjects[_objectIndex];
        }

        public void ClearPoolable(int _index)
        {
            PooledObjects[_index] = null;
        }

        public void ReleasePooledObject(int _index, bool _replicate = true)
        {
            if (_replicate)
                ReleasePooledObjectServerRpc(_index);
            PooledObjects[_index].Release();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReleasePooledObjectServerRpc(int _index) =>
            ReleasePooledObjectClientRpc(_index);

        [ClientRpc]
        private void ReleasePooledObjectClientRpc(int _index)
        {
            if (_index >= PooledObjects.Length || PooledObjects[_index] == null) return;
            ReleasePooledObject(_index, false);
        }
    }
}
#endif