#if ARMONY_NETCODE
using Codice.CM.SEIDInfo;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public abstract class NetworkPoolable : MonoBehaviour
    {
        private NetworkPool pool;

        protected void Release()
        {
            pool.ReleasePoolable(this);
            gameObject.SetActive(false);
        }
        
        public abstract void Get(Vector3 _position, Quaternion _rotation);

        public abstract NetworkPoolable Build(Transform _holder, INetworkPoolUser _user);
        public void Initialize(NetworkPool _pool) => pool = _pool;

        public abstract void Deinitialize();
    }
}
#endif