using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CinemaDirector
{
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

    [CutsceneItemAttribute("Animator", "Play Mecanim Animation", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class PlayAnimatorEvent : CinemaActorEvent
    {
        public string StateName;
        public int Layer = -1;
        float Normalizedtime = float.NegativeInfinity;
        AnimationClip ac;
        float runningTime;
        float lastTime;
        GameObject animatorActor;

        public override void Resume() {
            ac = null;
            animatorActor = null;
            runningTime = 0;
        }

        void Update()
        {
            if (ac == null)
            {
                return;
            }
            runningTime += Time.deltaTime;
            if (runningTime > lastTime)
                runningTime = 0;
            ac.SampleAnimation(animatorActor, runningTime);
        }
        
        public override void Trigger(GameObject actor)
        {
            Animator animator = actor.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }
            animatorActor = actor;
            animator.Play(StateName, Layer, Normalizedtime);
            runningTime = 0f;
            AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
            // overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            //AnimationClipOverrides clipOverrides = new AnimationClipOverrides(overrideController.overridesCount);
            //overrideController.GetOverrides(clipOverrides);

            AnimationClipPair[] pairclips = overrideController.clips;
            for (int i = 0; i < pairclips.Length; i++)
            {
                if (pairclips[i].originalClip.name.IndexOf(StateName) != -1)
                {
                    ac = pairclips[i].originalClip;
                    lastTime = ac.length;
                }
            }
        }
    }
}