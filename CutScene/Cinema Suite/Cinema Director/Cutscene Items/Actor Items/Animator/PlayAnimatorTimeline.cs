using UnityEngine;
using System.Collections;
using System;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Play Mecanim Animation Timeline", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class PlayAnimatorTimeline : CinemaActorAction
    {
        public string StateName;
        public int Layer = -1;
        float Normalizedtime = float.NegativeInfinity;

        public override void End(GameObject actor)
        {
            ac = null;
        }

        AnimationClip ac;
        Animator animator;
        public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
        {

            if (animator == null)
            {
                animator = actor.GetComponentInChildren<Animator>();
            }
            if (animator == null)
            {
                Debug.Log(actor.name + " animator  == null ");
                return;
            }
            if (ac == null)
            {
                animator.Play(StateName, Layer, Normalizedtime);
                /*
               AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
               for (int i = 0; i < clips.Length; i++)
               {
                   if (clips[i].name == StateName)
                   {
                       ac = clips[i];
                       Duration = clips[i].length;
                       ac.SampleAnimation(actor, runningTime);
                       break;
                   }
               }
               */
                AnimatorOverrideController overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
                AnimationClipPair[] pairclips = overrideController.clips;
                for (int i = 0; i < pairclips.Length; i++)
                {
                    if (pairclips[i].originalClip.name.IndexOf(StateName) != -1)
                    {
                        ac = pairclips[i].originalClip;
                        Duration = pairclips[i].originalClip.length;
                        ac.SampleAnimation(actor, runningTime);
                        break;
                    }
                }
            }
            else
            {
                ac.SampleAnimation(actor, runningTime);
            }
        }

        public override void Trigger(GameObject actor)
        {
            if (animator == null)
            {
                animator = actor.GetComponentInChildren<Animator>();
            }
            if (animator == null)
            {
                return;
            }

            animator.Play(StateName, Layer, Normalizedtime);
            //animator.StopPlayback();
            Debug.Log("StateName " + StateName + "Layer " + Layer);

            AnimatorOverrideController overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            /*
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == StateName)
                {
                    ac = clips[i];
                    Duration = clips[i].length;
                }
            }*/
            AnimationClipPair[] pairclips = overrideController.clips;
            for (int i = 0; i < pairclips.Length; i++)
            {
                if (pairclips[i].originalClip.name.IndexOf(StateName) != -1)
                {
                    ac = pairclips[i].originalClip;
                    Duration = pairclips[i].originalClip.length;
                }
            }
        }

        public override void Stop(GameObject actor)
        {
            if (animator == null)
            {
                animator = actor.GetComponentInChildren<Animator>();
            }
            if (animator == null)
            {
                return;
            }

            if (ac == null)
            {
                Debug.Log(actor.name + " AnimationClip = null");
                animator.StopPlayback();
            }
            else
            {
                animator.StopPlayback();
            }
        }
    }
}