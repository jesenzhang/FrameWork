using System;
using System.Collections.Generic; 
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The MultiActorTrackGroup maintains a list of Actors that have something in 
    /// common and a set of tracks that act upon those Actors.
    /// </summary>
    [TrackGroupAttribute("MultiActor Track Group", TimelineTrackGenre.MultiActorTrack)]
    public class MultiActorTrackGroup : TrackGroup, ISetMovieData
    {
        [SerializeField]
        private List<Transform> actors = new List<Transform>();
        /// <summary>
        /// The Actors that this TrackGroup is focused on
        /// </summary>
        public List<Transform> Actors
        {
            get
            {
                if (_movieData != null && actorNames.Count > 0)
                {
                    Transform tra;
                    for (int i = 0, len = actors.Count; i < len; i++)
                    {
                        tra = actors[i];
                        if (tra != null) continue;
                        actors[i] = _movieData.FindTra(actorNames[i]);
                    }
                    actorNames.Clear();///防止二次执行
                }
                return actors;
            }
            set
            {
                actors = value;
            }
        }
        #region 主要用于导出使用 
        [SerializeField]
        private MovieData _movieData;
        [SerializeField]
        private List<string> actorNames = new List<string>();
        public List<string> ActorNames { get { return actorNames; } }
        /// <summary>
        /// 设置 剧情链接控制器
        /// </summary>
        /// <param name="movieData"></param>
        public void SetMovieData(MovieData movieData)
        {
            if (movieData == null) return;
            _movieData = movieData;
            Transform item;
            for (int i = 0, len = actors.Count; i < len; i++)
            {
                item = actors[i];
                actorNames.Add(_movieData.GetUrl(item));
            }
        }
        #endregion
    }
}
