using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

namespace Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("FMOD Event References")]
        [SerializeField] private EventReference _backgroundMusicEvent;
        [SerializeField] private EventInstance _backgroundMusicInstance;
        public void PlayBGM(EventReference bgmEvent)
        {
            // 같은 BGM이면 무시
            if (_backgroundMusicInstance.isValid())
            {
                _backgroundMusicInstance.getDescription(out EventDescription desc);
                desc.getPath(out string path);

                // if (path == bgmEvent.Path)
                //     return;

                StopBGM();
            }

            _backgroundMusicEvent = bgmEvent;
            _backgroundMusicInstance = RuntimeManager.CreateInstance(_backgroundMusicEvent);
            _backgroundMusicInstance.start();
        }

        public void StopBGM()
        {
            if (_backgroundMusicInstance.isValid())
            {
                _backgroundMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _backgroundMusicInstance.release();
                _backgroundMusicInstance.clearHandle();
            }
        }
    }
}