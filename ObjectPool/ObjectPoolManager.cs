using System;
using System.Collections.Generic;
using UnityEngine;
public static class ObjectPoolManager
{
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

}