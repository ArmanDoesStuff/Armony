using System.Collections.Generic;
using Unity.Netcode;

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
                    TargetClientIds = new[]{rpcParams.Receive.SenderClientId}
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
    }
}