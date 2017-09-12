using UnityEngine;
using System.Collections;
using System;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Animator", "Play Role Animation Timeline", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class PlayAnimatorToAnimation : CinemaActorAction
    {
        public RoleType roleType = RoleType.TUOBA;

        public AnimationClip tuoba;
        public AnimationClip yunyuan;
        public AnimationClip luoli;
        public AnimationClip mojia;
        public WrapMode wrapMode;

#if UNITY_EDITOR
        public void Update()
        {
            AnimationClip animationClip = null;
            switch (roleType)
            {
                case RoleType.TUOBA:
                    animationClip = tuoba;
                    break;
                case RoleType.LINGHU:
                    animationClip = luoli;
                    break;
                case RoleType.YUNYUAN:
                    animationClip = yunyuan;
                    break;
                case RoleType.MOJIA:
                    animationClip = mojia;
                    break;
            }
            if (wrapMode != WrapMode.Loop && wrapMode != WrapMode.PingPong && animationClip)
            {
                if (base.Duration > animationClip.length)
                {
                    base.Duration = animationClip.length;
                }
            }
        }
#endif

        public override void End(GameObject Actor)
        {
            Animation animation = Actor.GetComponent<Animation>();
            if (animation)
                animation.Stop();
        }

        public override void UpdateTime(GameObject Actor, float runningTime, float deltaTime)
        {
            Animation animation = Actor.GetComponent<Animation>();
            AnimationClip animationClip = null;
            switch (MovieData.CurrProfessionType % 100)
            {
                case 1:
                    animationClip = tuoba;
                    break;
                case 2:
                    animationClip = luoli;
                    break;
                case 3:
                    animationClip = yunyuan;
                    break;
                case 4:
                    animationClip = mojia;
                    break;
            }
            if (!animation || animationClip == null)
            {
                return;
            }

            if (animation[animationClip.name] == null)
            {
                animation.AddClip(animationClip, animationClip.name);
            }

            AnimationState state = animation[animationClip.name];

            if (!animation.IsPlaying(animationClip.name))
            {
                animation.Play(animationClip.name);
            }

            state.time = runningTime;
            state.enabled = true;
            animation.Sample();
            state.enabled = false;
        }

        public override void Trigger(GameObject Actor)
        {
            Animation animation = Actor.GetComponent<Animation>();
            if (!animation)
            {
                animation = Actor.AddComponent<Animation>();
            }
            animation.wrapMode = wrapMode;
        }

    }
}