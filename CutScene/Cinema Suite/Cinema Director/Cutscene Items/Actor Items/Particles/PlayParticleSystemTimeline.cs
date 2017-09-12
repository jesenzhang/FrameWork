
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CinemaDirector
{
    [CutsceneItemAttribute("Particle System", "Play Particle System Timeline", CutsceneItemGenre.ActorItem, CutsceneItemGenre.MecanimItem)]
    public class PlayParticleSystemTimeline : CinemaActorAction
    {
        public bool isEditor = false;
#if UNITY_EDITOR
        public Transform currentParent = null;
        public Transform rootNode;
        public Transform mountNode;
        public Vector3 offset;
        public Vector3 rotation;
        private Vector3 ooffset;
        private Vector3 orotation;
#endif
        private Transform ParticleRoot;
        ParticleSystem[] particleSys;
        public ParticleSystem[] ParticleSys
        {
            get
            {
                if (particleSys == null && ParticleRoot!=null)
                {
                    List<ParticleSystem>  aparticleSys = new List<ParticleSystem>();
                    ParticleSystem[] tparticleSys = ParticleRoot.GetComponentsInChildren<ParticleSystem>();
                    for (int i = 0; i < tparticleSys.Length; i++)
                    {
                        if (ParticleSystemUtility.IsRoot(tparticleSys[i]))
                        {
                            aparticleSys.Add(tparticleSys[i]);
                        }
                    }
                    particleSys = aparticleSys.ToArray();
                }
                return particleSys;
            }

            set
            {
                particleSys = value;
            }
        } 
   

        private  string GetFullPath(Transform node)
        {
            string path = node.name;
            while (node.parent != null)
            {
                node = node.parent;
                path = node.name + "/" + path;
            }
            return path;
        }

        public override void End(GameObject actor)
        {
            if (ParticleRoot == null)
            {
                ParticleRoot = actor.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    if (ParticleSys[i] != null)
                    {
                        ParticleSys[i].Stop();
                        ParticleSys[i].Clear();
                    }
                }
#if UNITY_EDITOR
                if (isEditor)
                {
                    ParticleRoot.parent = currentParent;
                    ParticleRoot.localPosition = ooffset;
                    ParticleRoot.localRotation = Quaternion.Euler(orotation);
                    ParticleRoot.localScale = Vector3.one;
                }
#endif
            }
        }

        public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
        {
           
            if (ParticleRoot == null)
            {
                ParticleRoot = actor.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                   ParticleSys[i].Simulate(deltaTime, true,false);
                }
            }
        }

        public override void SetTime(GameObject Actor, float time, float deltaTime)
        {
            base.SetTime(Actor, time, deltaTime);
            if (ParticleRoot == null)
            {
                ParticleRoot = Actor.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Play();
                    ParticleSys[i].Simulate(time, true, false);
                }
            }
        }
        

        public override void Trigger(GameObject actor)
        {
            if (ParticleRoot == null)
            {
                ParticleRoot = actor.transform;
            }
            #if UNITY_EDITOR
            if (isEditor)
            {
                ooffset = ParticleRoot.localPosition;
                orotation = ParticleRoot.localRotation.eulerAngles;
                if (mountNode != null)
                    ParticleRoot.parent = mountNode;
                else
                {
                    if (rootNode != null)
                        ParticleRoot.parent = rootNode;
                }
                ParticleRoot.localPosition = offset;
                ParticleRoot.localRotation = Quaternion.Euler(rotation);
                ParticleRoot.localScale = Vector3.one;
            }
            #endif
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Play();
                    ParticleSys[i].Simulate(0);
                }
            }
            
        }


        public override void Stop(GameObject actor)
        {
            if (ParticleRoot == null)
            {
                ParticleRoot = actor.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    if (ParticleSys[i] != null)
                    {
                        ParticleSys[i].Stop();
                        ParticleSys[i].Clear();
                    }
                }
            }
#if UNITY_EDITOR
            if (isEditor)
            {
                ParticleRoot.parent = currentParent;
                ParticleRoot.localPosition = ooffset;
                ParticleRoot.localRotation = Quaternion.Euler(orotation);
                ParticleRoot.localScale = Vector3.one;
            }
#endif
        }
    }
}