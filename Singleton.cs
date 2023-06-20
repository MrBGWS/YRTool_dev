using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YRTool
{
    public class Singleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		public static bool HasInstance => _instance != null;
		public static T TryGetInstance() => HasInstance ? _instance : null;
		public static T Current => _instance;

		/// <summary>
		/// 单例实现，如果没有就自动创建一个物体
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// 唤醒时激活该单例的引用，重写时记得使用base.Awake
		/// </summary>
		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		/// <summary>
		/// 生成单例
		/// </summary>
		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			_instance = this as T;
		}
	}
}
