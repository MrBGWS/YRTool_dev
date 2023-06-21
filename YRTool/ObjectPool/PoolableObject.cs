using UnityEngine;
using UnityEngine.Events;

namespace YRTool
{
	/// <summary>
	/// 对象池对象
	/// Destroy()时只是设成了未激活
	/// 本身和对象池系统没有过多的耦合
	/// 对象池基础对象仍然是GameObject
	/// </summary>
	[AddComponentMenu("YRTool/Tools/Object Pool/YRPoolableObject")]
	public class PoolableObject : ObjectBounds
	{
		[Header("Events")]
		public UnityEvent ExecuteOnEnable;
		public UnityEvent ExecuteOnDisable;
		
		public delegate void Events();
		public event Events OnSpawnComplete;

		[Header("Poolable Object")]
		/// The life time, in seconds, of the object. If set to 0 it'll live forever, if set to any positive value it'll be set inactive after that time.
		/// 生命周期，单位秒，如果设为0就是永远不自动销毁。
		public float LifeTime = 0f;

		/// <summary>
		/// 设为未激活，等待复用
		/// </summary>
		public virtual void Destroy()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// 每帧动作
		/// </summary>
		protected virtual void Update()
		{

		}

		/// <summary>
		/// 激活时初始化
		/// </summary>
		protected virtual void OnEnable()
		{
			Size = GetBounds().extents * 2;
			if (LifeTime > 0f)
			{
				Invoke(nameof(Destroy), LifeTime);	
			}
			ExecuteOnEnable?.Invoke();
		}

		/// <summary>
		/// 取消激活时
		/// </summary>
		protected virtual void OnDisable()
		{
			ExecuteOnDisable?.Invoke();
			CancelInvoke();
		}

		/// <summary>
		/// 生成成功事件
		/// </summary>
		public virtual void TriggerOnSpawnComplete()
		{
			OnSpawnComplete?.Invoke();
		}
	}
}