using System;
using System.Collections.Generic;
using UnityEngine;
namespace CustomObjectPool
{
    public static class ObjectPoolManager
    {
        #region Declaration
        /*
        - The key is the type of script having pools.
        - The value is hashmap of different pools that the key has.
        */
        private static Dictionary<Type, Dictionary<Type, object>> _poolingObjects = new Dictionary<Type, Dictionary<Type, object>>();

        /*
        - The key is the type of script having pools.
        - The value is that script.
        - Used to access the script having pools -> no need to make that script become singleton.
        - It means using all scripts having pools through this ObjectPoolManager class.
        */
        private static Dictionary<Type, object> _poolObjContainer = new Dictionary<Type, object>();
        #endregion

        #region Register

        // Used to assign @_poolingObjects[T][N] = pooler.
        public static void RegisterPool<T, N>(T poolObj, ObjectPooler<N> pooler, bool isOverride = false)
        where T : class
        {
            // If @_poolingObjects has already have T type (of @poolObj).
            if (_poolingObjects.TryGetValue(typeof(T), out var poolerMap))
            {
                // @poolerMap = _poolingObjects[T] -> check whether it has N type or not.
                if (poolerMap.TryGetValue(typeof(N), out var objectPooler))
                {
                    if (isOverride)
                    {
                        poolerMap[typeof(N)] = pooler;
                    }
                    else Debug.LogWarning("The type " + typeof(T) + " has already have" + typeof(N) + "pool.");
                }
                else
                {
                    poolerMap[typeof(N)] = pooler;
                }
            }
            else
            {
                _poolObjContainer[typeof(T)] = poolObj;

                _poolingObjects[typeof(T)] = new Dictionary<Type, object>();
                _poolingObjects[typeof(T)][typeof(N)] = pooler;
            }
        }

        // Simple register method.
        public static ObjectPooler<T> RegisterPool<T>(GameObject prefab, Transform poolHolder, int initNum, bool isOverride = false)
        {
            var pooler = new ObjectPooler<T>(prefab, poolHolder, initNum);
            RegisterPool<T, T>(pooler, pooler, isOverride);
            return pooler;
        }
        #endregion

        #region Unregister
        // Unregister a pool from a poolObject.
        public static bool UnRegisterPool<T, N>(T poolObj, ObjectPooler<N> pooler)
        where T : class
        {
            if (_poolingObjects.TryGetValue(typeof(T), out var poolMap))
            {
                bool result = poolMap.Remove(typeof(N));
                if (poolMap.Count == 0)
                {
                    return _poolObjContainer.Remove(typeof(T)) && result;
                }
                return result;
            }
            return false;
        }

        // Clear poolObject.
        public static bool RemovePoolObject<T>(T poolObj)
        where T : class
        {
            return _poolingObjects.Remove(typeof(T)) && _poolObjContainer.Remove(typeof(T));
        }
        #endregion

        #region Return to pool
        public static N ReturnToPool<T, N>(N obj)
        where T : class
        {
            if (_poolingObjects.TryGetValue(typeof(T), out var poolerMap))
            {
                if (poolerMap.TryGetValue(typeof(N), out var objectPooler))
                {
                    (objectPooler as ObjectPooler<N>).ReturnPool(obj);
                    return obj;
                }
                else
                {
                    Debug.LogWarning("The type " + typeof(T) + " has not register " + typeof(N) + " pool yet.");
                }
            }
            else
            {
                Debug.LogWarning("The poolingObject has no-t have type " + typeof(T) + "yet.");
            }
            return default;
        }
        #endregion

        #region Get Object
        // Used to get an element of pool "N" from script "T".
        public static N GetElem<T, N>()
        where T : class
        {
            if (_poolingObjects.TryGetValue(typeof(T), out var poolerMap))
            {
                if (poolerMap.TryGetValue(typeof(N), out var objectPooler))
                {
                    return (objectPooler as ObjectPooler<N>).GetElem();
                }
            }
            return default;
        }

        // Used to get the script having pools.
        public static T GetPoolingObject<T>() where T : class
        {
            if (_poolObjContainer.TryGetValue(typeof(T), out var poolObj))
            {
                return poolObj as T;
            }
            else return default;
        }
        #endregion

        #region Clear
        public static void ClearPools<T>() where T : class
        {
            if (_poolingObjects.ContainsKey(typeof(T)))
            {
                _poolingObjects.Remove(typeof(T));
                _poolObjContainer.Remove(typeof(T));
            }
        }
        #endregion

        #region Debug
        public static (int active, int pooled) GetPoolStats<T, N>()
        {
            if (_poolingObjects.TryGetValue(typeof(T), out var poolerMap) &&
                poolerMap.TryGetValue(typeof(N), out var objectPooler))
            {
                var pooler = objectPooler as ObjectPooler<N>;
                return (pooler.ActiveCount, pooler.PooledCount);
            }
            return (0, 0);
        }
        #endregion
    }
}