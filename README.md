WHAT DOES IT HAVE:
1. Generic and Type-Safe.
2. Having a manager to handler all pools.
3. Using static methods instead of singleton.
4. Each pool uses Queue to manage objects.



HOW TO USE:
1. Declare ObjectPooler type variable in your class. Ex:
   private ObjectPooler<GameObject> _pooler; // This means you have a pool of GameObject.
   
2. Initialize the pool. Ex:
   _pooler = new ObjectPooler<GameObject>(_gameObjectPrefabs, transformPos, initNum);

3. Register that pool to ObjectPoolManager, Ex:
   ObjectPoolManager.RegisterPool(this, _pooler); 

4. Then you can use this pool for everwhere=)), Ex:
   ObjectPoolManager.ReturnToPool<TestObjectPool, GameObject>(gameObject);

NOTE: If the script having pool can not register to ObjectPoolManager, make sure that the order of 
      ObjectPoolManager is placed over that script in Script Execution Order.
