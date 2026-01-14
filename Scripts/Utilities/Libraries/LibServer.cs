//Copyright AWAN SOFTWORKS LTD 2025

#if ARMONY_NETCODE
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Armony.Utilities.Libraries
{
    public static class LibServer
    {
        public static ClientRpcParams SendExceptCaller(ServerRpcParams rpcParams)
        {
            List<ulong> clientIds = new(NetworkManager.Singleton.ConnectedClientsIds);
            clientIds.Remove(rpcParams.Receive.SenderClientId);
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = clientIds
                }
            };
            return clientRpcParams;
        }

        public static ClientRpcParams SendCaller(ServerRpcParams rpcParams)
        {
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            };
            return clientRpcParams;
        }

        public static ClientRpcParams TargetClients(params ulong[] targetClientIds)
        {
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = targetClientIds
                }
            };
            return clientRpcParams;
        }

        public static IEnumerator DespawnAfterDelay(this NetworkObject _networkObject, float _delaySeconds)
        {
            if (!_networkObject.IsOwnedByServer) yield break;
            
            yield return new WaitForSeconds(_delaySeconds);
            if (_networkObject != null && _networkObject.IsSpawned)
            {
                _networkObject.Despawn();
            }
        }
    }
}
#endif