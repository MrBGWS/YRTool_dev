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



        /// 管理声轨的可能方法 特效 音乐 UI 主声轨 其他
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
