using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// An event for setting a navigation destination.
    /// Only executes in runtime. Not reversable.
    /// </summary>
    [CutsceneItemAttribute("Navigation", "Set Destination", CutsceneItemGenre.ActorItem)]
    public class SetDestinationEvent : CinemaActorEvent
    {
        // The destination target
        public Vector3 target;

        /// <summary>
        /// Trigger this event and set a new destination.
        /// </summary>
        /// <param name="actor">The actor with a NavMeshAgent to set a new destination for.</param>
        public override void Trigger(GameObject actor)
        {
            UnityEngine.AI.NavMeshAgent agent = actor.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(target);
            }
        }
    }
}