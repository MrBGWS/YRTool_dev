using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YRTool
{
    public class SoundManager : PersistentSingleton<SoundManager>,
        EventListener<SoundManagerTrackEvent>,
        EventListener<SoundManagerEvent>,
        EventListener<SoundManagerSoundControlEvent>,
        EventListener<SoundManagerSoundFadeEvent>,
        EventListener<SoundManagerAllSoundsControlEvent>,
        EventListener<SoundManagerTrackFadeEvent>
    {



        /// ��������Ŀ��ܷ��� ��Ч ���� UI ������ ����
        public enum SoundManagerTracks { Sfx, Music, UI, Master, Other }

        public void OnYREvent(SoundManagerEvent eventType)
        {
            throw new System.NotImplementedException();
        }

        public void OnYREvent(SoundManagerTrackEvent eventType)
        {
            throw new System.NotImplementedException();
        }

        public void OnYREvent(SoundManagerSoundControlEvent eventType)
        {
            throw new System.NotImplementedException();
        }

        public void OnYREvent(SoundManagerSoundFadeEvent eventType)
        {
            throw new System.NotImplementedException();
        }

        public void OnYREvent(SoundManagerAllSoundsControlEvent eventType)
        {
            throw new System.NotImplementedException();
        }

        public void OnYREvent(SoundManagerTrackFadeEvent eventType)
        {
            throw new System.NotImplementedException();
        }
    }
}
