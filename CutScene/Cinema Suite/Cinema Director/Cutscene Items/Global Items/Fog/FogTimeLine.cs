// Cinema Suite
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CinemaDirector
{

    /// <summary>
    /// Use a kind of effect on Camera
    /// </summary>
    [CutsceneItem("FogSetting", "FogTimeLine", CutsceneItemGenre.GlobalItem)]
    public class FogTimeLine : CinemaGlobalAction
    {
        public float fogDensity = 0;
        public float fogStartDistance = 0;
        public float fogEndDistance = 0;
        public bool fog = false;
        public FogMode mode;
        public Color fogColor;
        /// <summary>
        /// Setup the effect when the script is loaded.
        /// </summary>
        void Awake()
        {
        }


        private void OnDestroy()
        {
           
        }
        /// <summary>
        /// Enable the overlay texture and set to From colour
        /// </summary>
        public override void Trigger()
        {
             
        }

        /// <summary>
        /// Firetime is reached when playing in reverse, disable the effect.
        /// </summary>
        public override void ReverseTrigger()
        {
            End();
        }

        /// <summary>
        /// Update the effect over time, progressing the transition
        /// </summary>
        /// <param name="time">The time this action has been active</param>
        /// <param name="deltaTime">The time since the last update</param>
        public override void UpdateTime(float time, float deltaTime)
        {
        }

        /// <summary>
        /// Set the transition to an arbitrary time.
        /// </summary>
        /// <param name="time">The time of this action</param>
        /// <param name="deltaTime">the deltaTime since the last update call.</param>
        public override void SetTime(float time, float deltaTime)
        {

        }

        /// <summary>
        /// End the effect by disabling the overlay texture.
        /// </summary>
        public override void End()
        {
            
        }

        /// <summary>
        /// The end of the action has been triggered while playing the Cutscene in reverse.
        /// </summary>
        public override void ReverseEnd()
        {
            End();
        }

        /// <summary>
        /// Disable the overlay texture
        /// </summary>
        public override void Stop()
        {
            End();
        }
    }
}