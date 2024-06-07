using Unity.Netcode;
using UnityEngine;

namespace Armony.Scripts.Utilities.NetworkPool
{
    public class NetworkPoolable : MonoBehaviour
    {
        public string Guid { get; set; }
        public int Index { get; set; }
        
        protected void RequestRelease()
        {
            NetworkPoolManager.ReleasePooledObject(Guid, Index);
        }
        
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