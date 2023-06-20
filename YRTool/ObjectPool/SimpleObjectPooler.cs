using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace YRTool.Tools
{
	/// <summary>
	/// 一个输出简单对象的对象池操作器
	/// </summary>
	[AddComponentMenu("YRTool/Tools/Object Pool/SimpleObjectPooler")]
	public class SimpleObjectPooler : ObjectPooler 
	{
		/// the game object we'll instantiate 
		public GameObject GameObjectToPool;
		/// 对象池大小
		public int PoolSize = 20;
		/// 为真则对象池会自动填充
		public bool PoolCanExpand = true;
	    
		public List<SimpleObjectPooler> Owner { get; set; }
		private void OnDestroy() { Owner?.Remove(this); }

		/// <summary>
		/// Fills the object pool with the gameobject type you've specified in the inspector
		/// </summary>
		public override void FillObjectPool()
		{
			if (GameObjectToPool == null)
			{
				return;
			}

			// if we've already created a pool, we exit
			if ((_objectPool != null) && (_objectPool.PooledGameObjects.Count > PoolSize))
			{
				return;
			}

			CreateWaitingPool ();

			int objectsToSpawn = PoolSize;

			if (_objectPool != null)
			{
				objectsToSpawn -= _objectPool.PooledGameObjects.Count;
			}

			// we add to the pool the specified number of objects
			for (int i = 0; i < objectsToSpawn; i++)
			{
				AddOneObjectToThePool ();
			}
		}

		/// <summary>
		/// Determines the name of the object pool.
		/// </summary>
		/// <returns>The object pool name.</returns>
		protected override string DetermineObjectPoolName()
		{
			return ("[SimpleObjectPooler] " + GameObjectToPool.name);	
		}
	    	
		/// <summary>
		/// 返回一个未激活的对象
		/// </summary>
		/// <returns>The pooled game object.</returns>
		public override GameObject GetPooledGameObject()
		{
			// we go through the pool looking for an inactive object
			for (int i=0; i< _objectPool.PooledGameObjects.Count; i++)
			{
				if (!_objectPool.PooledGameObjects[i].gameObject.activeInHierarchy)
				{
					// if we find one, we return it
					return _objectPool.PooledGameObjects[i];
				}
			}
			// if we haven't found an inactive object (the pool is empty), and if we can extend it, we add one new object to the pool, and return it		
			if (PoolCanExpand)
			{
				return AddOneObjectToThePool();
			}
			// if the pool is empty and can't grow, we return nothing.
			return null;
		}

		/// <summary>
		/// 将指定类型的一个对象添加到池中
		/// </summary>
		/// <returns>返回添加的对象</returns>
		protected virtual GameObject AddOneObjectToThePool()
		{
			if (GameObjectToPool == null)
			{
				Debug.LogWarning("The "+gameObject.name+" ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
				return null;
			}

			bool initialStatus = GameObjectToPool.activeSelf;
			GameObjectToPool.SetActive(false);
			GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
			GameObjectToPool.SetActive(initialStatus);
			SceneManager.MoveGameObjectToScene(newGameObject, this.gameObject.scene);
			if (NestWaitingPool)
			{
				newGameObject.transform.SetParent(_waitingPool.transform);	
			}
			newGameObject.name = GameObjectToPool.name + "-" + _objectPool.PooledGameObjects.Count;

			_objectPool.PooledGameObjects.Add(newGameObject);

			return newGameObject;
		}
	}
}