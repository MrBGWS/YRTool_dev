//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YRTool
{
	/// <summary>
	/// ���׵��¼�ʾ��
	/// </summary>
	public struct SimpleGameEvent
	{
		public string EventName;
		public SimpleGameEvent(string newName)
		{
			EventName = newName;
		}
		//static SimpleGameEvent e;
		//public static void Trigger(string newName)
		//{
		//	e.EventName = newName;
		//	EventManager.TriggerEvent(e);
		//}
	}

	/// <summary>
	/// �¼�ϵͳ �۲���ģʽ
	/// ���ȣ��۲�����̳� EventListener<MMGameEvent> ��ʵ�ֽӿ�
	/// �����ڹ۲������еļ������ȡ��������еǼ�/ע��
	/// void OnEnable()
	/// {
	/// 	this.MMEventStartListening<MMGameEvent>();
	/// }
	/// void OnDisable()
	/// {
	/// 	this.MMEventStopListening<MMGameEvent>();
	/// }
	/// ���Ҫ����ĳһ���¼�ʱ��ʹ��EventManager.TriggerEvent(new MMGameEvent())����ʽ�������¼�

	/// </summary>
	[ExecuteAlways]
	public static class EventManager
	{
		private static Dictionary<Type, List<EventListenerBase>> _subscribersList;

		static EventManager()
		{
			_subscribersList = new Dictionary<Type, List<EventListenerBase>>();
		}

		/// <summary>
		/// ���ض��¼����һ��������
		/// </summary>
		/// <param name="listener">listener.</param>
		/// <typeparam name="MMEvent">The event type.</typeparam>
		public static void AddListener<MMEvent>(EventListener<MMEvent> listener) where MMEvent : struct
		{
			Type eventType = typeof(MMEvent);

			if (!_subscribersList.ContainsKey(eventType))
			{
				_subscribersList[eventType] = new List<EventListenerBase>();
			}

			if (!SubscriptionExists(eventType, listener))
			{
				_subscribersList[eventType].Add(listener);
			}
		}

		/// <summary>
		/// ���ض��¼����Ƴ�һ��������
		/// </summary>
		/// <param name="listener">listener.</param>
		/// <typeparam name="MMEvent">The event type.</typeparam>
		public static void RemoveListener<MMEvent>(EventListener<MMEvent> listener) where MMEvent : struct
		{
			Type eventType = typeof(MMEvent);

			if (!_subscribersList.ContainsKey(eventType))
			{
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
				return;
#endif
			}

			List<EventListenerBase> subscriberList = _subscribersList[eventType];

#if EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false;
#endif

			for (int i = subscriberList.Count - 1; i >= 0; i--)
			{
				if (subscriberList[i] == listener)
				{
					subscriberList.Remove(subscriberList[i]);
#if EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
#endif

					if (subscriberList.Count == 0)
					{
						_subscribersList.Remove(eventType);
					}

					return;
				}
			}

#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
		}

		/// <summary>
		/// ����һ���ض��¼��������еĶ����߶�����
		/// </summary>
		/// <param name="newEvent">�����¼�����</param>
		/// <typeparam name="MMEvent">�¼��ṹ��</typeparam>
		public static void TriggerEvent<MMEvent>(MMEvent newEvent) where MMEvent : struct
		{
			List<EventListenerBase> list;
			if (!_subscribersList.TryGetValue(typeof(MMEvent), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
				return;
#endif

			for (int i = list.Count - 1; i >= 0; i--)
			{
				(list[i] as EventListener<MMEvent>).OnYREvent(newEvent);
			}
		}

		/// <summary>
		/// ����Ƿ��Ѿ�ע����һ��������
		/// </summary>
		/// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
		/// <param name="type">Type.</param>
		/// <param name="receiver">Receiver.</param>
		private static bool SubscriptionExists(Type type, EventListenerBase receiver)
		{
			List<EventListenerBase> receivers;

			if (!_subscribersList.TryGetValue(type, out receivers)) return false;

			bool exists = false;

			for (int i = receivers.Count - 1; i >= 0; i--)
			{
				if (receivers[i] == receiver)
				{
					exists = true;
					break;
				}
			}

			return exists;
		}
	}

	/// <summary>
	/// ע���ע���¼�����
	/// �����õ��������չ������ʹ���ײ���Ϊthis+������д��������Ϊ��������չ�º���
	/// Ϊ����EventListener��չ��Start��Stop����
	/// </summary>
	public static class EventRegister
	{
		//public delegate void Delegate<T>(T eventType);

		public static void MMEventStartListening<EventType>(this EventListener<EventType> caller) where EventType : struct
		{
			EventManager.AddListener<EventType>(caller);
		}

		public static void MMEventStopListening<EventType>(this EventListener<EventType> caller) where EventType : struct
		{
			EventManager.RemoveListener<EventType>(caller);
		}
	}

	/// <summary>
	/// �¼����������ӿ�
	/// </summary>
	public interface EventListenerBase { };

	/// <summary>
	/// ���͵��¼������ߣ���������߶�Ҫʵ������ӿ�
	/// </summary>
	public interface EventListener<T> : EventListenerBase
	{
		void OnYREvent(T eventType);
	}

	/// <summary>
	/// ��װ�ļ������࣬MM�����ķ�װʾ��
	/// </summary>
	/// <typeparam name="TOwner"></typeparam>
	/// <typeparam name="TTarget"></typeparam>
	/// <typeparam name="TEvent"></typeparam>
	public class MMEventListenerWrapper<TOwner, TTarget, TEvent> : EventListener<TEvent>, IDisposable
		where TEvent : struct
	{
		private Action<TTarget> _callback;

		private TOwner _owner;
		public MMEventListenerWrapper(TOwner owner, Action<TTarget> callback)
		{
			_owner = owner;
			_callback = callback;
			RegisterCallbacks(true);
		}

		public void Dispose()
		{
			RegisterCallbacks(false);
			_callback = null;
		}

		protected virtual TTarget OnEvent(TEvent eventType) => default;
		public void OnYREvent(TEvent eventType)
		{
			var item = OnEvent(eventType);
			_callback?.Invoke(item);
		}

		private void RegisterCallbacks(bool b)
		{
			if (b)
			{
				this.MMEventStartListening<TEvent>();
			}
			else
			{
				this.MMEventStopListening<TEvent>();
			}
		}
	}

}
