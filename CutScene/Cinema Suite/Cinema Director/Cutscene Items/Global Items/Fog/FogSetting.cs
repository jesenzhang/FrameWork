
// Cinema Suite
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Transition from Clear to White over time by overlaying a guiTexture.
    /// </summary>
    [CutsceneItem("FogSetting", "FogSetting", CutsceneItemGenre.GlobalItem)]
    public class FogSetting : CinemaGlobalEvent
    {
        public float fogDensity = 0;
        public float fogStartDistance = 0;
        public float fogEndDistance = 0;
        public bool fog = false;
        public FogMode mode;
        public Color fogColor;
        /// <summary>
        /// Trigger the level load. Only in Runtime.
        /// </summary>

        public override void Trigger()
        {
           // if (Application.isPlaying)
            {
                RenderSettings.fog = fog;
                RenderSettings.fogMode = mode;
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fogStartDistance = fogStartDistance;
                RenderSettings.fogEndDistance = fogEndDistance;
                RenderSettings.fogColor = fogColor;
            }
        }
    }
}