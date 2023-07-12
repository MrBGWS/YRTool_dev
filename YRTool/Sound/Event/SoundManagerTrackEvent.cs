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
	/// �˷��������ھ�����ȡ�����������š���ͣ��ֹͣ���ͷŻ�����ѡ����Ŀ������
	/// ���� :  MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.PauseTrack,MMSoundManager.MMSoundManagerTracks.UI);
	/// ����ͣ����UI�켣
	/// </summary>
	public struct SoundManagerTrackEvent
	{
		/// �����촫������
		public SoundManagerTrackEventTypes TrackEventType;
		/// Ŀ������
		public SoundManager.SoundManagerTracks Track;
		/// ����ڡ�����������ģʽ�£�Ҫ���ݵ������˳�򣬼�Ҫ���������õ�������
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
	/// ���¼�����������SoundManager���õı���/����/����
	///
	/// ���� : SoundManagerEvent.Trigger(SoundManagerEventTypes.SaveSettings);
	/// ��洢��������
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
	/// ���ڿ���SoundManager���ض��������¼���
	/// �����ͨ��ID������Ҳ����ֱ�Ӵ�����ƵԴ��������У���
	///
	/// ���� : MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Stop, 33);
	/// ������IDΪ33������ֹͣ����
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
	/// ��ʹIDΪ33��������2�����Ե�������˥����0.3������
	/// </summary>
	public struct SoundManagerSoundFadeEvent
	{
		/// Ҫ˥����������ID
		public int SoundID;
		/// ���뵭���ĳ���ʱ�䣨����Ϊ��λ��
		public float FadeDuration;
		/// ����ֵ
		public float FinalVolume;
		//TODO ��ʱ����
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
	/// ���¼�����������ͣ/����/ֹͣ/�ͷ�ͨ��MMSoundManager���ŵ���������
	///
	/// Example : SoundManagerAllSoundsControlEvent.Trigger(SoundManagerAllSoundsControlEventTypes.Stop);
	/// ������ֹͣ������������
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
		//TODO ��ʱ����
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
