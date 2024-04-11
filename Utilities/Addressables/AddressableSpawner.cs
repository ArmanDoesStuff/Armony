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
        private static readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> AsyncOperationHandles = new();
        private static Dictionary<AssetReference, List<GameObject>> SpawnedObjects { get; } = new();

        #region Spawn
        public static async Task<T> LoadAndSpawnAddressable<T>(
            AssetReference aRef,
            Vector3 pos,
            Quaternion rot,
            Transform par,
            bool single = false,
            bool autoRelease = true,
            Action action = null
        )
            where T : MonoBehaviour
        {
            return (await LoadAndSpawnAddressable(aRef, pos, rot, par, single, autoRelease, action)).GetComponent<T>();
        }

        public static async Task<GameObject> LoadAndSpawnAddressable(
            AssetReference aRef,
            Vector3 pos,
            Quaternion rot,
            Transform par,
            bool single = false,
            bool autoRelease = true,
            Action action = null
        )
        {
            if (!aRef.RuntimeKeyIsValid())
            {
                Debug.LogError($"Invalid key: {aRef.RuntimeKey}");
                return null;
            }

            if (!AsyncOperationHandles.ContainsKey(aRef))
            {
                AsyncOperationHandles[aRef] = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(aRef);
                return await LoadRefAndSpawn(aRef, pos, rot, par, autoRelease, action);
            }
            else if (!single)
            {
                if (AsyncOperationHandles[aRef].IsDone)
                {
                    return await SpawnFromLoadedRef(aRef, pos, rot, par, autoRelease, action);
                }
                else
                {
                    return await LoadRefAndSpawn(aRef, pos, rot, par, autoRelease, action);
                }
            }
            return GetLastSpawnedAddressable(aRef);
        }

        private static async Task<GameObject> LoadRefAndSpawn(
            AssetReference aRef,
            Vector3 pos,
            Quaternion rot,
            Transform par,
            bool autoRelease,
            Action action
        )
        {
            while (AsyncOperationHandles[aRef].Status != AsyncOperationStatus.Succeeded)
                await Task.Yield();
            return await SpawnFromLoadedRef(aRef, pos, rot, par, autoRelease, action);
        }

        private static async Task<GameObject> SpawnFromLoadedRef(
            AssetReference aRef,
            Vector3 pos,
            Quaternion rot,
            Transform par,
            bool autoRelease,
            Action action
        )
        {
            var instantiateAsync = aRef.InstantiateAsync(pos, rot, par);
            GameObject go = instantiateAsync.Result;
            while (instantiateAsync.Status != AsyncOperationStatus.Succeeded)
                await Task.Yield();

            if (!SpawnedObjects.ContainsKey(aRef))
            {
                SpawnedObjects[aRef] = new List<GameObject>();
            }
            SpawnedObjects[aRef].Add(go);

            if (autoRelease)
            {
                AddressableGameObject goAdd = go.AddComponent<AddressableGameObject>();
                goAdd.assetRef = aRef;
                goAdd.Destroyed += ReleaseSingle;
            }

            action?.Invoke();
            return go;
        }
        #endregion

        #region Release
        public static void ReleaseAll(AssetReference aRef)
        {
            List<GameObject> spawnedGOs = GetSpawnedAddressables(aRef);
            for (int i = 0; i < spawnedGOs.Count; i++)
            {
                Release(aRef, spawnedGOs[i]);
            }
            CheckHandle(aRef);
        }

        public static void ReleaseSingle(AssetReference aRef, GameObject go)
        {
            Release(aRef, go);
            CheckHandle(aRef);
        }

        private static void Release(AssetReference aRef, GameObject go)
        {
            UnityEngine.AddressableAssets.Addressables.ReleaseInstance(go);
            SpawnedObjects[aRef].Remove(go);
        }

        private static void CheckHandle(AssetReference aRef)
        {
            if (SpawnedObjects[aRef].Count == 0)
            {
                if (AsyncOperationHandles[aRef].IsValid())
                {
                    UnityEngine.AddressableAssets.Addressables.Release(AsyncOperationHandles[aRef]);
                }
                AsyncOperationHandles.Remove(aRef);
                SpawnedObjects.Remove(aRef);
            }
        }
        #endregion

        #region Other
        public static List<GameObject> GetSpawnedAddressables(AssetReference aRef) => AddressableHasBeenSpawned(aRef) ? SpawnedObjects[aRef] : null;

        public static List<T> GetSpawnedAddressables<T>(AssetReference aRef)
            where T : MonoBehaviour => AddressableHasBeenSpawned(aRef) ? SpawnedObjects[aRef].Select((o) => o.GetComponent<T>()).ToList<T>() : null;

        public static GameObject GetLastSpawnedAddressable(AssetReference aRef) =>
            AddressableHasBeenSpawned(aRef) ? SpawnedObjects[aRef][SpawnedObjects[aRef].Count - 1] : null;

        public static T GetLastSpawnedAddressable<T>(AssetReference aRef)
            where T : MonoBehaviour =>
            AddressableHasBeenSpawned(aRef) ? SpawnedObjects[aRef][SpawnedObjects[aRef].Count - 1].GetComponent<T>() : null;

        public static bool AddressableHasBeenSpawned(AssetReference aRef)
        {
            return SpawnedObjects.ContainsKey(aRef);
        }
    }
        #endregion
}
