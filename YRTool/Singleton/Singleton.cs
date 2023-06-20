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
		/// ����ʵ�֣����û�о��Զ�����һ������
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
		/// ����ʱ����õ��������ã���дʱ�ǵ�ʹ��base.Awake
		/// </summary>
		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		/// <summary>
		/// ���ɵ���
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
