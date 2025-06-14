using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomObjectPool
{
    public class ObjectPooler<T>
    {
        private Queue<T> _pool;
        private Transform _poolHolder;
        private GameObject _prefab;

        public ObjectPooler(GameObject prefab, Transform poolHolder, int initNum)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (poolHolder == null) throw new ArgumentNullException(nameof(_poolHolder));
            if (initNum < 0) throw new ArgumentException("Initial number cannot be negative", nameof(initNum));

            _prefab = prefab;
            _poolHolder = poolHolder;
            _pool = new Queue<T>();

            for (int i = 0; i < initNum; i++)
            {
                GameObject elem = GameObject.Instantiate(_prefab, _poolHolder.position, Quaternion.identity);
                elem.transform.SetParent(poolHolder.transform);
                T elemT = elem.GetComponent<T>();
                if (elemT is IPoolable poolable) elemT.Init();
                elem.SetActive(false);
                _pool.Enqueue(elemT);
            }
        }

        // @isCreated : when the pool count = 0 and then GetElem -> create new object if @isCreated is true else return default.
        public T GetElem(bool isCreated = false)
        {
            if (_pool.Count == 0)
            {
                if (!isCreated) return default;

                GameObject elem = GameObject.Instantiate(_prefab, _poolHolder.position, Quaternion.identity);
                return elem.GetComponent<T>();
            }
            else
            {
                T elem = _pool.Dequeue();
                return elem;
            }

        }

        public void ReturnPool(T elem)
        {
            if (elem is IPoolable poolable) poolable.Reset();
            _pool.Enqueue(elem); // Add to UnusedPool
        }

    }
}