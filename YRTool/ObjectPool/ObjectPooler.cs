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
		/// 如果为真则在找到相同名字的对象池时不会创建一个新的池，默认false为会创建
		public bool MutualizeWaitingPools = false;
		/// 池化对象会生成在一个空对象池物体下，否则就不设置父物体
		public bool NestWaitingPool = true;
		/// 对象池物体会以pooler作为父物体
		public bool NestUnderThis = false;

		/// 用来作父物体的对象
		protected GameObject _waitingPool = null;
		protected ObjectPool _objectPool;
		protected const int _initialPoolsListCapacity = 5;
		protected bool _onSceneLoadedRegistered = false;
        
		public static List<ObjectPool> _pools = new List<ObjectPool>(_initialPoolsListCapacity);

		/// <summary>
		/// 将该池加入静态池列表
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
		/// 将对象池从静态池列表中取出
		/// </summary>
		/// <param name="pool"></param>
		public static void RemovePool(ObjectPool pool)
		{
			_pools?.Remove(pool);
		}

		/// <summary>
		/// 当唤醒物体时，先填充物体池
		/// </summary>
		protected virtual void Awake()
		{
			Instance = this;
			FillObjectPool();		
		}

		/// <summary>
		/// 创造或重用一个对象池
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
		/// 寻找一个相同名字的对象池
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
		/// 如果需要，则设置池的父对象为自己
		/// </summary>
		protected virtual void ApplyNesting()
		{
			if (NestWaitingPool && NestUnderThis && (_waitingPool != null))
			{
				_waitingPool.transform.SetParent(this.transform);
			}
		}

		/// <summary>
		/// 生成物体的名字
		/// </summary>
		/// <returns>The object pool name.</returns>
		protected virtual string DetermineObjectPoolName()
		{
			return ("[ObjectPooler] " + this.name);	
		}

		/// <summary>
		/// 填充对象池
		/// </summary>
		public virtual void FillObjectPool()
		{
			return ;
		}

		/// <summary>
		/// 实现这个方法来返回一个池化对象
		/// </summary>
		/// <returns>The pooled game object.</returns>
		public virtual GameObject GetPooledGameObject()
		{
			return null;
		}

		/// <summary>
		/// 释放对象池
		/// </summary>
		public virtual void DestroyObjectPool()
		{
			if (_waitingPool != null)
			{
				Destroy(_waitingPool.gameObject);
			}
		}

		/// <summary>
		/// 场景加载时事件添加
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!_onSceneLoadedRegistered)
			{
				SceneManager.sceneLoaded += OnSceneLoaded;    
			}
		}

		/// <summary>
		/// 场景加载时填充对象池
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
		/// 销毁时将自己移除对象池列表
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