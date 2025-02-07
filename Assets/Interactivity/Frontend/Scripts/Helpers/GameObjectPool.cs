using System;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    internal class GameObjectPool
    {
        private ObjectPool<GameObject> _autoAttackPool;
        private GameObject _prefab;

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
            _autoAttackPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        }

        public GameObject Get()
        {
            return _autoAttackPool.Get();
        }

        public void Release(GameObject obj)
        {
           _autoAttackPool.Release(obj);
        }

        private void OnDestroyPoolObject(GameObject obj)
        {
            GameObject.Destroy(obj);
        }

        private void OnReturnedToPool(GameObject obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(GameObject obj)
        {
            // Handled in the manager to allow for transform changes to occur before setting active.
        }

        private GameObject CreatePooledItem()
        {
            return GameObject.Instantiate(_prefab);
        }
    }
}
