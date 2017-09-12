using UnityEngine;
using System.Collections;
namespace CinemaDirector
{
    [CutsceneItemAttribute("Audio Source", "Play Sound Audio", CutsceneItemGenre.ActorItem)]
    public class PlaySoundEvent : CinemaActorEvent
    {
        public AudioClip audioClip = null;

        public override void Trigger(GameObject Actor)
        {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = Actor.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }

            if (audio.clip != audioClip)
                audio.clip = audioClip;

            audio.volume = MovieData.SoundVOLUME;
            audio.time = 0.0f;
            audio.loop = true;
            audio.Play();
        }

        public override void Stop(GameObject Actor) {
            AudioSource audio = Actor.GetComponent<AudioSource>();
            if(audio != null)
            {
                audio.Stop();
            }
        }
    }
}