
// Cinema Suite
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using GameBase;
namespace CinemaDirector
{
    /// <summary>
    /// Event for loading a new level
    /// </summary>
    [CutsceneItem("Utility", "Send Message", CutsceneItemGenre.GlobalItem)]
    public class SendMessage : CinemaGlobalEvent
    {
        public string msg = "";
        public int param = -1;
        public bool hasParam = false;
        public Cutscene cutscene;
        /// <summary>
        /// Trigger the level load. Only in Runtime.
        /// </summary>
        
        public override void Trigger()
        {
           if (Application.isPlaying)
            {
                if(hasParam)
                    cutscene.OnMessage(msg,param);
                else
                    cutscene.OnMessage(msg);
            }
        }
    }
}