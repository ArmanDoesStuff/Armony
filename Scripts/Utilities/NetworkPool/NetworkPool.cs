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
        private INetworkPoolUser networkPoolUser;
        private readonly Stack<NetworkPoolable> pooledObjects = new();
        private Transform holder;
        private NetworkPoolable poolableType;

        public override void OnNetworkSpawn()
        {
            holder = new GameObject("NetworkPool") { transform = { parent = transform } }.transform;
            base.OnNetworkSpawn();
        }

        public void Construct(INetworkPoolUser _networkPoolUser, NetworkPoolable _poolableType)
        {
            poolableType = _poolableType;
            networkPoolUser = _networkPoolUser;
            while (pooledObjects.Count > 0)
            {
                NetworkPoolable poolable = pooledObjects.Pop();
                if (poolable != null)
                {
                    poolable.Deinitialize();
                }
            }
        }

        public NetworkPoolable GetPoolable(Vector3 _position, Quaternion _rotation, bool _replicate = true)
        {
            if (_replicate)
                GetPoolableServerRPC(_position, _rotation);

            NetworkPoolable poolable;
            if (pooledObjects.Count == 0)
            {
                poolable = poolableType.Build(holder, networkPoolUser);
                poolable.Initialize(this);
                pooledObjects.Push(poolable);
            }

            poolable = pooledObjects.Pop();
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
            pooledObjects.Push(_pooledObject);
        }
    }
}
#endif