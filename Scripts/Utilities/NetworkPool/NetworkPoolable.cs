using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public abstract class NetworkPoolable : MonoBehaviour
    {
        public int Index { get; set; }

        protected abstract void RequestRelease();

        public virtual void Release()
        {
            gameObject.SetActive(false);
        }

        public virtual void Get()
        {
            gameObject.SetActive(true);
        }
    }
}