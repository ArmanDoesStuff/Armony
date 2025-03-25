//AWAN SOFTWORKS LTD 2022
#if ARMONY_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armony.Utilities.Addressables
{
    public class AddressableGameObject : MonoBehaviour
    {
        public event Action<AssetReference, GameObject> Destroyed;

        [HideInInspector]
        public AssetReference AssetRef;

        private void OnDestroy()
        {
            //Calls ReleaseSingle on the AddressableSpawner
            Destroyed?.Invoke(AssetRef, gameObject);
        }
    }
}
#endif