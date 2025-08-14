#if ARMONY_NETCODE
using System;
using System.Collections.Generic;
using System.Linq;
using Armony.Utilities.Libraries;
using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPool : NetworkBehaviour
    {
        private INetworkPoolUser NetworkPoolUser { get; set; }
        private Stack<NetworkPoolable> PooledObjects { get; set; }
        private Transform Holder { get; set; }
        private NetworkPoolable poolableType;

        public void Construct(INetworkPoolUser _networkPoolUser, NetworkPoolable _poolableType)
        {
            poolableType = _poolableType;
            if (PooledObjects != null)
            {
                foreach (NetworkPoolable poolable in PooledObjects.Where(_poolable => _poolable != null))
                {
                    poolable.Deinitialize();
                }
            }
            else
            {
                Holder = new GameObject("NetworkPool").transform;
                PooledObjects = new Stack<NetworkPoolable>();
            }

            NetworkPoolUser = _networkPoolUser;
            Holder.parent = transform;
        }

        public NetworkPoolable GetPoolable(Vector3 _position, Quaternion _rotation, bool _replicate = true)
        {
            if (_replicate)
                GetPoolableServerRPC(_position, _rotation);

            NetworkPoolable poolable;
            if (PooledObjects.Count == 0)
            {
                poolable = poolableType.Build(Holder, NetworkPoolUser);
                poolable.Initialize(this);
                PooledObjects.Push(poolable);
            }

            poolable = PooledObjects.Pop();
            poolable.Get(_position, _rotation);
            return poolable;
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetPoolableServerRPC(Vector3 _position, Quaternion _rotation, ServerRpcParams _params = default) =>
            GetPoolableClientRPC(_position, _rotation, LibServer.SendExceptCaller(_params));

        [ClientRpc]
        private void GetPoolableClientRPC(Vector3 _position, Quaternion _rotation, ClientRpcParams _sendCaller) =>
            GetPoolable(_position, _rotation, false);


        public void ReleasePoolable(NetworkPoolable _pooledObject)
        {
            PooledObjects.Push(_pooledObject);
        }
    }
}
#endif