using UnityEngine; 
namespace CinemaDirector
{
    /// <summary>
    /// The ActorTrackGroup maintains an Actor and a set of tracks that affect 
    /// that actor over the course of the Cutscene.
    /// </summary>
    [TrackGroupAttribute("Actor Track Group", TimelineTrackGenre.ActorTrack)]
    public class ActorTrackGroup : TrackGroup, ISetMovieData
    {
        [SerializeField]
        private Transform actor;
        /// <summary>
        /// The Actor that this TrackGroup is focused on.
        /// </summary>
        public Transform Actor
        {
            get
            {
                if (actor == null)
                {
                    if (string.IsNullOrEmpty(actorUrl) || _movieData == null) return null;
                    actor = _movieData.FindTra(actorUrl);
                }
                return actor;
            }
            set { actor = value; }
        }
#region 主要用于导出使用 
        [SerializeField]
        private string actorUrl;
        [SerializeField]
        private MovieData _movieData;
        /// <summary>
        /// Actor 的 url 地址 
        /// zhm
        /// </summary>
        public string ActorUrl
        {
            get { return actorUrl; }
        }

        public void SetMovieData(MovieData movieData)
        {
            if (movieData == null) return;
            _movieData = movieData;
            //获取actorUrl 主要用于后面使用
            actorUrl = movieData.GetUrl(actor); 
        }
#endregion
    }
}