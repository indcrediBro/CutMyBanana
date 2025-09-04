using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Utilities
{
    [System.Serializable]
    public class ObjectPooler
    {
        public bool addToDontDetroyOnLoad = false;
        private GameObject poolsHolder;

        private static GameObject gameobjectsHolder;
        private static GameObject particlesHolder;
        private static GameObject sfxHolder;

        private static Dictionary<GameObject, ObjectPool<GameObject>> pools;
        private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

        public enum PoolType
        {
            GameObjects,
            ParticleSystems,
            SFX
        }

        public static PoolType PoolingType;

        public void InitializePool()
        {
            pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

            InitializeHolders();
        }

        private void InitializeHolders()
        {
            poolsHolder = new GameObject("Object Pools");

            gameobjectsHolder = new GameObject("Pooled GameObjects");
            gameobjectsHolder.transform.SetParent(poolsHolder.transform);

            particlesHolder = new GameObject("Pooled ParticleSystems");
            particlesHolder.transform.SetParent(poolsHolder.transform);

            sfxHolder = new GameObject("Pooled SFX");
            sfxHolder.transform.SetParent(poolsHolder.transform);

            if (addToDontDetroyOnLoad)
                Object.DontDestroyOnLoad(gameobjectsHolder.transform.root);
        }

        private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, pos, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject
            );
            pools.Add(prefab, pool);
        }
        private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, parent, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject
            );
            pools.Add(prefab, pool);
        }

        private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);
            GameObject obj = MonoBehaviour.Instantiate(prefab, pos, rot);
            prefab.SetActive(true);

            SetParentObject(obj, poolType);

            return obj;
        }

        private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);
            GameObject obj = MonoBehaviour.Instantiate(prefab, parent);
            
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = rot;
            obj.transform.localScale = Vector3.one;
            
            prefab.SetActive(true);

            return obj;
        }
        private static void OnGetObject(GameObject obj)
        {

        }

        private static void OnReleaseObject(GameObject obj)
        {
            obj.SetActive(false);
        }

        private static void OnDestroyObject(GameObject obj)
        {
            if (cloneToPrefabMap.ContainsKey(obj))
                cloneToPrefabMap.Remove(obj);
        }

        private static void SetParentObject(GameObject obj, PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.GameObjects:
                    obj.transform.SetParent(gameobjectsHolder.transform);
                    break;
                case PoolType.ParticleSystems:
                    obj.transform.SetParent(particlesHolder.transform);
                    break;
                case PoolType.SFX:
                    obj.transform.SetParent(sfxHolder.transform);
                    break;
            }
        }

        private static T SpawnObject<T>(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects) where T : Object
        {
            if (!pools.ContainsKey(prefab))
                CreatePool(prefab, pos, rot, poolType);

            var obj = pools[prefab].Get();

            if (!cloneToPrefabMap.ContainsKey(obj))
                cloneToPrefabMap.Add(obj, prefab);

            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
                return obj as T;

            T component = obj.GetComponent<T>();
            if (component != null)
            {
                Debug.LogError($"The object {prefab.name} does not have a component of type {typeof(T).Name}.");
            }

            return component;
        }
        
        private static T SpawnObject<T>(GameObject prefab, Transform parent, Quaternion rot,
            PoolType poolType = PoolType.GameObjects) where T : Object
        {
            if (!pools.ContainsKey(prefab))
                CreatePool(prefab, parent, rot, poolType);

            var obj = pools[prefab].Get();

            if (!cloneToPrefabMap.ContainsKey(obj))
                cloneToPrefabMap.Add(obj, prefab);

            obj.transform.SetParent(parent);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = rot;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
                return obj as T;

            T component = obj.GetComponent<T>();
            if (component != null)
            {
                Debug.LogError($"The object {prefab.name} does not have a component of type {typeof(T).Name}.");
            }

            return component;
        }

        public static T SpawnObject<T>(T prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(prefab.gameObject, pos, rot, poolType);
        }

        public static GameObject SpawnObject(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(prefab, pos, rot, poolType);
        }

        public static T SpawnObject<T>(T prefab, Transform parent, Quaternion rot,
            PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(prefab.gameObject, parent, rot, poolType);
        }

        public static GameObject SpawnObject(GameObject prefab, Transform parent, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(prefab, parent, rot, poolType);
        }

        public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
        {
            if (cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
            {
                SetParentObject(obj, poolType);
            }

            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }

            if (!obj.activeSelf) return;
        }
    }
}