using CinemaDirector.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Enable the Actor related to this event.
    /// </summary>
    [CutsceneItemAttribute("Particle System", "Pause", CutsceneItemGenre.ActorItem)]
    public class PauseParticleSystemEvent : CinemaActorEvent
    {
        /// <summary>
        /// Trigger this event and pause the particle system component.
        /// </summary>
        /// <param name="actor">The actor to be triggered.</param>
        public override void Trigger(GameObject actor)
        {
            if (actor != null)
            {
                ParticleSystem[] particleSys = actor.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < particleSys.Length; i++)
                {
                    if (ParticleSystemUtility.IsRoot(particleSys[i]))
                    {
                        particleSys[i].Pause();
                    }
                }
            }
        }
    }
}