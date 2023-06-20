using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YRTool
{
    /// <summary>
    /// 不会随场景卸载的单例
    /// </summary>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        [Header("Persistent Singleton")]
		//如果true，那么单例在唤醒时发现自己有父对象的话，将自动分离
		[Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
		public bool AutomaticallyUnparentOnAwake = true;

		public static bool HasInstance => _instance != null;
		public static T Current => _instance;

		protected static T _instance;
		protected bool _enabled;

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
		/// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
		/// </summary>
		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		/// <summary>
		/// Initializes the singleton.
		/// </summary>
		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (AutomaticallyUnparentOnAwake)
			{
				this.transform.SetParent(null);
			}

			if (_instance == null)
			{
				//If I am the first instance, make me the Singleton
				_instance = this as T;
				DontDestroyOnLoad(transform.gameObject);
				_enabled = true;
			}
			else
			{
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if (this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}

