//AWAN SOFTWORKS LTD 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Armony.Utilities.Addressables
{
    public static class AddressableSpawner
    {
        private static  Dictionary<AssetReference, AsyncOperationHandle<GameObject>> AsyncOperationHandles { get; } = new();
        private static Dictionary<AssetReference, List<GameObject>> SpawnedObjects { get; } = new();

        public static async Task<T> LoadAndSpawnAddressable<T>(
            AssetReference assetReference,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool single = false,
            bool autoRelease = true,
            Action action = null
        )
            where T : MonoBehaviour
        {
            return (await LoadAndSpawnAddressable(assetReference, position, rotation, parent, single, autoRelease, action)).GetComponent<T>();
        }

        public static async Task<GameObject> LoadAndSpawnAddressable(
            AssetReference assetReference,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool single = false,
            bool autoRelease = true,
            Action action = null
        )
        {
            if (!assetReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"Invalid key: {assetReference.RuntimeKey}");
                return null;
            }

            if (!AsyncOperationHandles.ContainsKey(assetReference))
            {
                AsyncOperationHandles[assetReference] = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(assetReference);
                return await LoadRefAndSpawn(assetReference, position, rotation, parent, autoRelease, action);
            }
            else if (!single)
            {
                if (AsyncOperationHandles[assetReference].IsDone)
                {
                    return await SpawnFromLoadedRef(assetReference, position, rotation, parent, autoRelease, action);
                }
                else
                {
                    return await LoadRefAndSpawn(assetReference, position, rotation, parent, autoRelease, action);
                }
            }
            return GetLastSpawnedAddressable(assetReference);
        }

        private static async Task<GameObject> LoadRefAndSpawn(
            AssetReference assetReference,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool autoRelease,
            Action action
        )
        {
            while (AsyncOperationHandles[assetReference].Status != AsyncOperationStatus.Succeeded)
                await Task.Yield();
            return await SpawnFromLoadedRef(assetReference, position, rotation, parent, autoRelease, action);
        }

        private static async Task<GameObject> SpawnFromLoadedRef(
            AssetReference assetReference,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool autoRelease,
            Action action
        )
        {
            AsyncOperationHandle<GameObject> instantiateAsync = assetReference.InstantiateAsync(position, rotation, parent);
            GameObject go = instantiateAsync.Result;
            while (instantiateAsync.Status != AsyncOperationStatus.Succeeded)
                await Task.Yield();

            if (!SpawnedObjects.ContainsKey(assetReference))
            {
                SpawnedObjects[assetReference] = new List<GameObject>();
            }
            SpawnedObjects[assetReference].Add(go);

            if (autoRelease)
            {
                AddressableGameObject goAdd = go.AddComponent<AddressableGameObject>();
                goAdd.AssetRef = assetReference;
                goAdd.Destroyed += ReleaseSingle;
            }

            action?.Invoke();
            return go;
        }

        public static void ReleaseAll(AssetReference assetReference)
        {
            List<GameObject> spawnedAddressables = GetSpawnedAddressables(assetReference);
            foreach (GameObject go in spawnedAddressables)
            {
                Release(assetReference, go);
            }
            CheckHandle(assetReference);
        }

        public static void ReleaseSingle(AssetReference assetReference, GameObject go)
        {
            Release(assetReference, go);
            CheckHandle(assetReference);
        }

        private static void Release(AssetReference assetReference, GameObject go)
        {
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(go);
            SpawnedObjects[assetReference].Remove(go);
        }

        private static void CheckHandle(AssetReference assetReference)
        {
            if (SpawnedObjects[assetReference].Count == 0)
            {
                if (AsyncOperationHandles[assetReference].IsValid())
                {
                    UnityEngine.AddressableAssets.Addressables.Release(AsyncOperationHandles[assetReference]);
                }
                AsyncOperationHandles.Remove(assetReference);
                SpawnedObjects.Remove(assetReference);
            }
        }

        public static List<GameObject> GetSpawnedAddressables(AssetReference assetReference) => AddressableHasBeenSpawned(assetReference) ? SpawnedObjects[assetReference] : null;

        public static List<T> GetSpawnedAddressables<T>(AssetReference assetReference)
            where T : MonoBehaviour => AddressableHasBeenSpawned(assetReference) ? SpawnedObjects[assetReference].Select((o) => o.GetComponent<T>()).ToList<T>() : null;

        public static GameObject GetLastSpawnedAddressable(AssetReference assetReference) =>
            AddressableHasBeenSpawned(assetReference) ? SpawnedObjects[assetReference][SpawnedObjects[assetReference].Count - 1] : null;

        public static T GetLastSpawnedAddressable<T>(AssetReference assetReference)
            where T : MonoBehaviour =>
            AddressableHasBeenSpawned(assetReference) ? SpawnedObjects[assetReference][SpawnedObjects[assetReference].Count - 1].GetComponent<T>() : null;

        public static bool AddressableHasBeenSpawned(AssetReference assetReference)
        {
            return SpawnedObjects.ContainsKey(assetReference);
        }
    }
}
