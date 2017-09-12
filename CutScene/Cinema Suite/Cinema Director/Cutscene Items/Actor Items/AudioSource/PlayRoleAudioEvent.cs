// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : wangdi_m
// Created          : 07-07-2017
//
// Last Modified By : wangdi_m
// Last Modified On : 07-07-2017
// ***********************************************************************

using UnityEngine;
using System.Collections;
using System;

namespace CinemaDirector
{
    [Serializable]
    public enum RoleType
    {
        TUOBA = 1,
        LINGHU = 2,
        YUNYUAN = 3,
        MOJIA = 4
    }

    [CutsceneItemAttribute("Audio Source", "Play Role Audio", CutsceneItemGenre.ActorItem)]
    public class PlayRoleAudioEvent : CinemaActorAction
    {
        public AudioClip audioClip = null;

        public RoleType roleType;

        private bool wasPlaying = false;

        /// <summary>
        /// 检查是否是当前角色播放
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckPlay()
        {
            return (int)roleType != (MovieData.CurrProfessionType % 100);
        }

        public override void Trigger(GameObject Actor)
        {
            if(CheckPlay())
            {
                return;
            }

            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = Actor.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }
            if (audio.clip != audioClip)
                audio.clip = audioClip;
            audio.volume = MovieData.EffectVolume;
            audio.time = 0.0f;
            audio.loop = false;
            audio.Play();
            Duration = audioClip.length;
        }

        public override void UpdateTime(GameObject Actor, float runningTime, float deltaTime)
        {
            if (CheckPlay())
            {
                return;
            }
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = Actor.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }
            if (audio.clip != audioClip)
                audio.clip = audioClip;
            if (audio.isPlaying)
                return;
            audio.time = deltaTime;
            audio.Play();
        }

        public override void Resume(GameObject Actor)
        {
            if (CheckPlay())
            {
                return;
            }
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
                return;
            audio.time = Cutscene.RunningTime - Firetime;
            if (wasPlaying)
                audio.Play();
        }

        public override void Pause(GameObject Actor)
        {
            if (CheckPlay())
            {
                return;
            }
            AudioSource audio = Actor.GetComponent<AudioSource>();
            wasPlaying = false;
            if (audio && audio.isPlaying)
                wasPlaying = true;
            if (audio)
                audio.Pause();
        }

        public override void End(GameObject Actor)
        {
            if (CheckPlay())
            {
                return;
            }
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (audio)
                audio.Stop();
        }
    }
}