using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YRTool
{
	#region SoundManagerTrackEvent
	public enum SoundManagerTrackEventTypes
	{
		MuteTrack,
		UnmuteTrack,
		SetVolumeTrack,
		PlayTrack,
		PauseTrack,
		StopTrack,
		FreeTrack
	}

	/// <summary>
	/// 此反馈可用于静音、取消静音、播放、暂停、停止、释放或设置选定曲目的音量
	/// 例子 :  MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PauseTrack,MMSoundManager.MMSoundManagerTracks.UI);
	/// 将暂停整个UI轨迹
	/// </summary>
	public struct SoundManagerTrackEvent
	{
		/// 给音轨传的命令
		public SoundManagerTrackEventTypes TrackEventType;
		/// 目标音轨
		public SoundManager.SoundManagerTracks Track;
		/// 如果在“设置音量”模式下，要传递到音轨的顺序，即要将音轨设置到的音量
		public float Volume;

		public SoundManagerTrackEvent(SoundManagerTrackEventTypes trackEventType, SoundManager.SoundManagerTracks track = SoundManager.SoundManagerTracks.Master, float volume = 1f)
		{
			TrackEventType = trackEventType;
			Track = track;
			Volume = volume;
		}

		static SoundManagerTrackEvent e;
		public static void Trigger(SoundManagerTrackEventTypes trackEventType, SoundManager.SoundManagerTracks track = SoundManager.SoundManagerTracks.Master, float volume = 1f)
		{
			e.TrackEventType = trackEventType;
			e.Track = track;
			e.Volume = volume;
			EventManager.TriggerEvent(e);
		}
	}
	#endregion

	#region SoundManagerEvent
	public enum SoundManagerEventTypes
	{
		SaveSettings,
		LoadSettings,
		ResetSettings,
		SettingsLoaded
	}

	/// <summary>
	/// 此事件允许您触发SoundManager设置的保存/加载/重置
	///
	/// 例子 : SoundManagerEvent.Trigger(SoundManagerEventTypes.SaveSettings);
	/// 会存储声音设置
	/// </summary>
	public struct SoundManagerEvent
	{
		public SoundManagerEventTypes EventType;

		public SoundManagerEvent(SoundManagerEventTypes eventType)
		{
			EventType = eventType;
		}

		static SoundManagerEvent e;
		public static void Trigger(SoundManagerEventTypes eventType)
		{
			e.EventType = eventType;
			EventManager.TriggerEvent(e);
		}
	}
	#endregion
	#region SoundManagerSoundControlEvent
	public enum SoundManagerSoundControlEventTypes
	{
		Pause,
		Resume,
		Stop,
		Free
	}

	/// <summary>
	/// 用于控制SoundManager上特定声音的事件。
	/// 你可以通过ID搜索，也可以直接传递音频源（如果你有）。
	///
	/// 例子 : MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Stop, 33);
	/// 将导致ID为33的声音停止播放
	/// </summary>
	public struct SoundManagerSoundControlEvent
	{
		/// the ID of the sound to control (has to match the one used to play it)
		public int SoundID;
		/// the control mode
		public SoundManagerSoundControlEventTypes SoundManagerSoundControlEventType;
		/// the audiosource to control (if specified)
		public AudioSource TargetSource;

		public SoundManagerSoundControlEvent(SoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			SoundID = soundID;
			TargetSource = source;
			SoundManagerSoundControlEventType = eventType;
		}

		static SoundManagerSoundControlEvent e;
		public static void Trigger(SoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			e.SoundID = soundID;
			e.TargetSource = source;
			e.SoundManagerSoundControlEventType = eventType;
			EventManager.TriggerEvent(e);
		}
	}
	#endregion
	#region SoundManagerSoundFadeEvent
	/// <summary>
	/// Example : MMSoundManagerSoundFadeEvent.Trigger(33, 2f, 0.3f, new MMTweenType(MMTween.MMTweenCurve.EaseInElastic));
	/// 将使ID为33的声音在2秒内以弹性曲线衰减到0.3的音量
	/// </summary>
	public struct SoundManagerSoundFadeEvent
	{
		/// 要衰减的声音的ID
		public int SoundID;
		/// 淡入淡出的持续时间（以秒为单位）
		public float FadeDuration;
		/// 最终值
		public float FinalVolume;
		//TODO 暂时屏蔽
		/// the tween over which to fade this sound
		// public MMTweenType FadeTween;

		public SoundManagerSoundFadeEvent(int soundID, float fadeDuration, float finalVolume)//, MMTweenType fadeTween)
		{
			SoundID = soundID;
			FadeDuration = fadeDuration;
			FinalVolume = finalVolume;
			//FadeTween = fadeTween;
		}

		static SoundManagerSoundFadeEvent e;
		public static void Trigger(int soundID, float fadeDuration, float finalVolume)//, MMTweenType fadeTween)
		{
			e.SoundID = soundID;
			e.FadeDuration = fadeDuration;
			e.FinalVolume = finalVolume;
			//e.FadeTween = fadeTween;
			EventManager.TriggerEvent(e);
		}
	}

	#endregion SoundManagerAllSoundsControlEvent
	#region
	public enum SoundManagerAllSoundsControlEventTypes
	{
		Pause, Play, Stop, Free, FreeAllButPersistent, FreeAllLooping
	}

	/// <summary>
	/// 此事件将允许您暂停/播放/停止/释放通过MMSoundManager播放的所有声音
	///
	/// Example : SoundManagerAllSoundsControlEvent.Trigger(SoundManagerAllSoundsControlEventTypes.Stop);
	/// 将立即停止播放所有声音
	/// </summary>
	public struct SoundManagerAllSoundsControlEvent
	{
		public SoundManagerAllSoundsControlEventTypes EventType;

		public SoundManagerAllSoundsControlEvent(SoundManagerAllSoundsControlEventTypes eventType)
		{
			EventType = eventType;
		}

		static SoundManagerAllSoundsControlEvent e;
		public static void Trigger(SoundManagerAllSoundsControlEventTypes eventType)
		{
			e.EventType = eventType;
			EventManager.TriggerEvent(e);
		}
	}

	#endregion
	#region SoundManagerTrackFade
	/// <summary>
	/// This event will let you order the MMSoundManager to fade an entire track's sounds' volume towards the specified FinalVolume
	///
	/// Example : MMSoundManagerTrackFadeEvent.Trigger(MMSoundManager.MMSoundManagerTracks.Music, 2f, 0.5f, new MMTweenType(MMTween.MMTweenCurve.EaseInCubic));
	/// will fade the volume of the music track towards 0.5, over 2 seconds, using an ease in cubic tween 
	/// </summary>
	public struct SoundManagerTrackFadeEvent
	{
		/// the track to fade the volume of
		public SoundManager.SoundManagerTracks Track;
		/// the duration of the fade, in seconds
		public float FadeDuration;
		/// the final volume to fade towards
		public float FinalVolume;
		//TODO 暂时屏蔽
		/// the tween to use when fading
		//public MMTweenType FadeTween;

		public SoundManagerTrackFadeEvent(SoundManager.SoundManagerTracks track, float fadeDuration, float finalVolume)// MMTweenType fadeTween)
		{
			Track = track;
			FadeDuration = fadeDuration;
			FinalVolume = finalVolume;
			//FadeTween = fadeTween;
		}

		static SoundManagerTrackFadeEvent e;
		public static void Trigger(SoundManager.SoundManagerTracks track, float fadeDuration, float finalVolume)// MMTweenType fadeTween)
		{
			e.Track = track;
			e.FadeDuration = fadeDuration;
			e.FinalVolume = finalVolume;
			//e.FadeTween = fadeTween;
			EventManager.TriggerEvent(e);
		}
	}

	#endregion
}
