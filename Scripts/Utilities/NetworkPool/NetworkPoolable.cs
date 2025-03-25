#if ARMONY_NETCODE
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public interface INetworkPoolable
    {
        public int Index { get; set; }

        void RequestRelease();
        
        void Release();
        
        void Get(Vector3 position, Quaternion rotation);

        GameObject Initialize(Transform holder, INetworkPoolUser parent);

        void Deinitialize();
    }
}
#endif