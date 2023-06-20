using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace YRTool
{
	/// <summary>
	/// 对象池操作器基类
	/// 抽象类，不能直接使用，使用SimpleObjectPooler 或 MultipleObjectPooler类替代
	/// </summary>
	public abstract class ObjectPooler : MonoBehaviour
	{
		/// 单例
		public static ObjectPooler Instance;
		/// if this is true, the pool will try not to create a new waiting pool if it finds one with the same name.
		public bool MutualizeWaitingPools = false;
		/// if this is true, all waiting and active objects will be regrouped under an empty game object. Otherwise they'll just be at top level in the hierarchy
		public bool NestWaitingPool = true;
		/// if this is true, the waiting pool will be nested under this object
		public bool NestUnderThis = false;

		/// this object is just used to group the pooled objects
		protected GameObject _waitingPool = null;
		protected ObjectPool _objectPool;
		protected const int _initialPoolsListCapacity = 5;
		protected bool _onSceneLoadedRegistered = false;
        
		public static List<ObjectPool> _pools = new List<ObjectPool>(_initialPoolsListCapacity);

		/// <summary>
		/// Adds a pooler to the static list if needed
		/// </summary>
		/// <param name="pool"></param>
		public static void AddPool(ObjectPool pool)
		{
			if (_pools == null)
			{
				_pools = new List<ObjectPool>(_initialPoolsListCapacity);    
			}
			if (!_pools.Contains(pool))
			{
				_pools.Add(pool);
			}
		}

		/// <summary>
		/// Removes a pooler from the static list
		/// </summary>
		/// <param name="pool"></param>
		public static void RemovePool(ObjectPool pool)
		{
			_pools?.Remove(pool);
		}

		/// <summary>
		/// On awake we fill our object pool
		/// </summary>
		protected virtual void Awake()
		{
			Instance = this;
			FillObjectPool();
			
		}

		/// <summary>
		/// Creates the waiting pool or tries to reuse one if there's already one available
		/// </summary>
		protected virtual bool CreateWaitingPool()
		{
			if (!MutualizeWaitingPools)
			{
				// we create a container that will hold all the instances we create
				_waitingPool = new GameObject(DetermineObjectPoolName());
				SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
				_objectPool = _waitingPool.AddComponent<ObjectPool>();
				_objectPool.PooledGameObjects = new List<GameObject>();
				ApplyNesting();
				return true;
			}
			else
			{
				ObjectPool objectPool = ExistingPool(DetermineObjectPoolName());
				if (objectPool != null)
				{
					_objectPool = objectPool;
					_waitingPool = objectPool.gameObject;
					return false;
				}
				else
				{
					_waitingPool = new GameObject(DetermineObjectPoolName());
					SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
					_objectPool = _waitingPool.AddComponent<ObjectPool>();
					_objectPool.PooledGameObjects = new List<GameObject>();
					ApplyNesting();
					AddPool(_objectPool);
					return true;
				}
			}
		}
        
		/// <summary>
		/// Looks for an existing pooler for the same object, returns it if found, returns null otherwise
		/// </summary>
		/// <param name="objectToPool"></param>
		/// <returns></returns>
		public virtual ObjectPool ExistingPool(string poolName)
		{
			if (_pools == null)
			{
				_pools = new List<ObjectPool>(_initialPoolsListCapacity);    
			}
			if (_pools.Count == 0)
			{
				var pools = FindObjectsOfType<ObjectPool>();
				if (pools.Length > 0)
				{
					_pools.AddRange(pools);
				}
			}
			foreach (ObjectPool pool in _pools)
			{
				if ((pool != null) && (pool.name == poolName)/* && (pool.gameObject.scene == this.gameObject.scene)*/)
				{
					return pool;
				}
			}
			return null;
		}

		/// <summary>
		/// If needed, nests the waiting pool under this object
		/// </summary>
		protected virtual void ApplyNesting()
		{
			if (NestWaitingPool && NestUnderThis && (_waitingPool != null))
			{
				_waitingPool.transform.SetParent(this.transform);
			}
		}

		/// <summary>
		/// Determines the name of the object pool.
		/// </summary>
		/// <returns>The object pool name.</returns>
		protected virtual string DetermineObjectPoolName()
		{
			return ("[ObjectPooler] " + this.name);	
		}

		/// <summary>
		/// Implement this method to fill the pool with objects
		/// </summary>
		public virtual void FillObjectPool()
		{
			return ;
		}

		/// <summary>
		/// Implement this method to return a gameobject
		/// </summary>
		/// <returns>The pooled game object.</returns>
		public virtual GameObject GetPooledGameObject()
		{
			return null;
		}

		/// <summary>
		/// Destroys the object pool
		/// </summary>
		public virtual void DestroyObjectPool()
		{
			if (_waitingPool != null)
			{
				Destroy(_waitingPool.gameObject);
			}
		}

		/// <summary>
		/// On enable we register to the scene loaded hook
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!_onSceneLoadedRegistered)
			{
				SceneManager.sceneLoaded += OnSceneLoaded;    
			}
		}

		/// <summary>
		/// OnSceneLoaded we recreate 
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="loadSceneMode"></param>
		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (this == null)
			{
				return;
			}
			if ((_objectPool == null) || (_waitingPool == null))
			{
				if (this != null)
				{
					FillObjectPool();    
				}
			}
		}
        
		/// <summary>
		/// On Destroy we remove ourselves from the list of poolers 
		/// </summary>
		private void OnDestroy()
		{
			if ((_objectPool != null) && NestUnderThis)
			{
				RemovePool(_objectPool);    
			}

			if (_onSceneLoadedRegistered)
			{
				SceneManager.sceneLoaded -= OnSceneLoaded;
				_onSceneLoadedRegistered = false;
			}
		}
	}
}