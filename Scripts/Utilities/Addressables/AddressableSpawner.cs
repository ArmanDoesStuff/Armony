//AWAN SOFTWORKS LTD 2022

#if ARMONY_ADDRESSABLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Armony.Utilities.Addressables
{
    public static class AddressableSpawner
    {
        private static Dictionary<AssetReference, AsyncOperationHandle<GameObject>> AsyncOperationHandles { get; } = new();
        private static Dictionary<AssetReference, List<GameObject>> SpawnedObjects { get; } = new();

        private static CancellationTokenSource tokenSource;

        private static CancellationToken Token
        {
            get
            {
                if (tokenSource == null || tokenSource.IsCancellationRequested)
                    tokenSource = new CancellationTokenSource();
                return tokenSource.Token;
            }
        }

        public static async Task<T> InstantiateAddressable<T>(
            this GameObject _owner,
            AssetReference _assetReference,
            bool _single = false,
            bool _autoRelease = true,
            Action _action = null
        )
            where T : MonoBehaviour
        {
            Transform parent = _owner.transform;
            return await InstantiateAddressable<T>(_assetReference, parent.position, parent.rotation, parent, _single, _autoRelease, _action);
        }

        public static async Task<T> InstantiateAddressable<T>(
            AssetReference _assetReference,
            Vector3 _position,
            Quaternion _rotation,
            Transform _parent,
            bool _single = false,
            bool _autoRelease = true,
            Action _action = null
        )
            where T : MonoBehaviour
        {
            return (await InstantiateAddressable(_assetReference, _position, _rotation, _parent, _single, _autoRelease, _action)).GetComponent<T>();
        }

        public static async Task<GameObject> InstantiateAddressable(
            AssetReference _assetReference,
            Vector3 _position,
            Quaternion _rotation,
            Transform _parent,
            bool _single = false,
            bool _autoRelease = true,
            Action _action = null
        )
        {
            if (!_assetReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"Invalid key: {_assetReference.RuntimeKey}");
                return null;
            }

            if (!AsyncOperationHandles.ContainsKey(_assetReference))
            {
                AsyncOperationHandles[_assetReference] = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(_assetReference);
                return await LoadRefAndSpawn(_assetReference, _position, _rotation, _parent, _autoRelease, _action);
            }

            if (_single) return GetLastSpawnedAddressable(_assetReference);
            if (AsyncOperationHandles[_assetReference].IsDone)
            {
                return await SpawnFromLoadedRef(_assetReference, _position, _rotation, _parent, _autoRelease, _action);
            }

            return await LoadRefAndSpawn(_assetReference, _position, _rotation, _parent, _autoRelease, _action);
        }

        private static async Task<GameObject> LoadRefAndSpawn(
            AssetReference _assetReference,
            Vector3 _position,
            Quaternion _rotation,
            Transform _parent,
            bool _autoRelease,
            Action _action
        )
        {
            while (AsyncOperationHandles[_assetReference].Status == AsyncOperationStatus.None)
            {
                Token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            return await SpawnFromLoadedRef(_assetReference, _position, _rotation, _parent, _autoRelease, _action);
        }

        private static async Task<GameObject> SpawnFromLoadedRef(
            AssetReference _assetReference,
            Vector3 _position,
            Quaternion _rotation,
            Transform _parent,
            bool _autoRelease,
            Action _action
        )
        {
            AsyncOperationHandle<GameObject> instantiateAsync = _assetReference.InstantiateAsync(_position, _rotation, _parent);
            while (instantiateAsync.Status == AsyncOperationStatus.None)
            {
                Token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            GameObject go = instantiateAsync.Result;

            if (!SpawnedObjects.ContainsKey(_assetReference))
            {
                SpawnedObjects[_assetReference] = new List<GameObject>();
            }

            SpawnedObjects[_assetReference].Add(go);

            if (_autoRelease)
            {
                AddressableGameObject goAdd = go.AddComponent<AddressableGameObject>();
                goAdd.AssetRef = _assetReference;
                goAdd.Destroyed += ReleaseSingle;
            }

            _action?.Invoke();
            return go;
        }

        public static void ReleaseAll(AssetReference _assetReference)
        {
            List<GameObject> spawnedAddressables = GetSpawnedAddressables(_assetReference);
            foreach (GameObject go in spawnedAddressables)
            {
                Release(_assetReference, go);
            }

            CheckHandle(_assetReference);
        }

        private static void ReleaseSingle(AssetReference _assetReference, GameObject _go)
        {
            Release(_assetReference, _go);
            CheckHandle(_assetReference);
        }

        private static void Release(AssetReference _assetReference, GameObject _go)
        {
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(_go);
            SpawnedObjects[_assetReference].Remove(_go);
        }

        private static void CheckHandle(AssetReference _assetReference)
        {
            if (SpawnedObjects[_assetReference].Count != 0) return;
            if (AsyncOperationHandles[_assetReference].IsValid())
            {
                UnityEngine.AddressableAssets.Addressables.Release(AsyncOperationHandles[_assetReference]);
            }

            AsyncOperationHandles.Remove(_assetReference);
            SpawnedObjects.Remove(_assetReference);
        }

        public static List<GameObject> GetSpawnedAddressables(AssetReference _assetReference) => AddressableHasBeenSpawned(_assetReference) ? SpawnedObjects[_assetReference] : null;

        public static List<T> GetSpawnedAddressables<T>(AssetReference _assetReference)
            where T : MonoBehaviour => AddressableHasBeenSpawned(_assetReference) ? SpawnedObjects[_assetReference].Select((_o) => _o.GetComponent<T>()).ToList<T>() : null;

        public static GameObject GetLastSpawnedAddressable(AssetReference _assetReference) =>
            AddressableHasBeenSpawned(_assetReference) ? SpawnedObjects[_assetReference][SpawnedObjects[_assetReference].Count - 1] : null;

        public static T GetLastSpawnedAddressable<T>(AssetReference _assetReference)
            where T : MonoBehaviour =>
            AddressableHasBeenSpawned(_assetReference) ? SpawnedObjects[_assetReference][SpawnedObjects[_assetReference].Count - 1].GetComponent<T>() : null;

        public static bool AddressableHasBeenSpawned(AssetReference _assetReference)
        {
            return SpawnedObjects.ContainsKey(_assetReference);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            Debug.Log("AddressableSpawner Reset");
        }
    }
}
#endif