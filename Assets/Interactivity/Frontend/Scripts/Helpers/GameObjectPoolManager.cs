using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static class GameObjectPoolManager
    {
        private static readonly Dictionary<GameObject, GameObjectPool> _pools = new Dictionary<GameObject, GameObjectPool>();

        public static bool CreatePool(GameObject prefab)
        {
            if (_pools.ContainsKey(prefab))
                return false;

            _pools.Add(prefab, new GameObjectPool(prefab));

            return true;
        }

        public static bool TryGet(GameObject prefab, out GameObject obj)
        {
            obj = null;

            if (!_pools.TryGetValue(prefab, out var pool))
                return false;

            obj = pool.Get();
            obj.gameObject.SetActive(true);

            return true;
        }

        public static bool TryGet(GameObject prefab, Vector3 position, Quaternion rotation, out GameObject obj)
        {
            obj = null;

            if (!_pools.TryGetValue(prefab, out var pool))
                return false;

            obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);

            return true;
        }

        public static bool TryGet(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale, out GameObject obj)
        {
            obj = null;

            if (!_pools.TryGetValue(prefab, out var pool))
                return false;

            obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = localScale;
            obj.gameObject.SetActive(true);

            return true;
        }

        public static bool TryRelease(GameObject prefab, GameObject obj)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
                return false;

            pool.Release(obj);
            return true;
        }
    }
}
