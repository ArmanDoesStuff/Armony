//AWAN SOFTWORKS LTD 2022

using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armony.Utilities.Addressables
{
    public class AddressableGameObject : MonoBehaviour
    {
        public event Action<AssetReference, GameObject> Destroyed;

        [HideInInspector]
        public AssetReference assetRef;

        private void OnDestroy()
        {
            //Calls ReleaseSingle on the AddressableSpawner
            Destroyed?.Invoke(assetRef, gameObject);
        }
    }
}
