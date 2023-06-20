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
	/// 简易的事件示例
	/// </summary>
	public struct SimpleGameEvent
	{
		public string EventName;
		public SimpleGameEvent(string newName)
		{
			EventName = newName;
		}
		static SimpleGameEvent e;
		public static void Trigger(string newName)
		{
			e.EventName = newName;
			EventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
	/// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
	/// basically just made of a string, but you can work with more complex ones if you want.
	/// 
	/// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
	/// So MMGameEvent.Trigger("Save"); for example will trigger a Save MMGameEvent
	/// 
	/// you can also call MMEventManager.TriggerEvent(YOUR_EVENT);
	/// For example : MMEventManager.TriggerEvent(new MMGameEvent("GameStart")); will broadcast an MMGameEvent named GameStart to all listeners.
	///
	/// To start listening to an event from any class, there are 3 things you must do : 
	///
	/// 1 - tell that your class implements the MMEventListener interface for that kind of event.
	/// For example: public class GUIManager : Singleton<GUIManager>, MMEventListener<MMGameEvent>
	/// You can have more than one of these (one per event type).
	///
	/// 2 - On Enable and Disable, respectively start and stop listening to the event :
	/// void OnEnable()
	/// {
	/// 	this.MMEventStartListening<MMGameEvent>();
	/// }
	/// void OnDisable()
	/// {
	/// 	this.MMEventStopListening<MMGameEvent>();
	/// }
	/// 
	/// 3 - Implement the MMEventListener interface for that event. For example :
	/// public void OnMMEvent(MMGameEvent gameEvent)
	/// {
	/// 	if (gameEvent.EventName == "GameOver")
	///		{
	///			// DO SOMETHING
	///		}
	/// } 
	/// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
	/// 事件系统 观察者模式
	/// 首先，观察者类继承 EventListener<MMGameEvent> 并实现接口
	/// 接着在观察者类中的激活函数和取消激活函数中登记/注销
	/// void OnEnable()
	/// {
	/// 	this.MMEventStartListening<MMGameEvent>();
	/// }
	/// void OnDisable()
	/// {
	/// 	this.MMEventStopListening<MMGameEvent>();
	/// }
	/// 最后要触发某一类事件时，使用EventManager.TriggerEvent(new MMGameEvent())的形式来触发事件

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
		/// 给特定事件添加一个监听者
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
		/// 从特定事件中移除一个监听者
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
		/// 触发一个特定事件，让所有的订阅者都触发
		/// </summary>
		/// <param name="newEvent">触发事件数据</param>
		/// <typeparam name="MMEvent">事件结构体</typeparam>
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
				(list[i] as EventListener<MMEvent>).OnMMEvent(newEvent);
			}
		}

		/// <summary>
		/// 检查是否已经注册了一个监听者
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
	/// 注册和注销事件监听
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
	/// 事件监听基础接口
	/// </summary>
	public interface EventListenerBase { };

	/// <summary>
	/// 泛型的事件监听者，任意监听者都要实现这个接口
	/// </summary>
	public interface EventListener<T> : EventListenerBase
	{
		void OnMMEvent(T eventType);
	}

	/// <summary>
	/// 封装的监听者类，MM给出的封装示例
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
		public void OnMMEvent(TEvent eventType)
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
