using CinemaDirector;
using System;
using System.Collections.Generic;
using UnityEngine;
// ***********************************************************************
//作者：zhm 
//说明： Animation Or Animator Info 用于保存剧情中需要替换的数据
// ***********************************************************************
[Serializable]
public class TiedGameObject
{
    public TimelineItem item;
    public int index;
}

[Serializable]
public class MovieReplaceInfo
{
    /// <summary>
    /// 唯一id
    /// </summary>
    public string ReplaceId = "";
    public string ResName = "";
    /// <summary>
    /// 是否绑定了Animation
    /// </summary> 
    public bool IsAnimation = false;
    /// <summary>
    /// 是否绑定了Animator
    /// </summary> 
    public bool IsAnimator = false;
    /// <summary>
    ///剧情控制器
    /// </summary>
    public MovieData CurrMovieData = null;
    /// <summary>
    /// GameObjct Url
    /// </summary>
    public string CurrObjctName = "";
    /// <summary>
    /// GameObjct Url
    /// </summary>
    public string CurrAnimationController = "";
    /// <summary>
    ///  GameObjct Url base
    /// </summary>
    public string CurrObjctUrlbase = "";
    /// <summary>
    /// 当前被替换物体所在层级
    /// </summary>
    public int CurrObjLevel = 0;
    /// <summary>
    /// 需要替换的物体
    /// </summary>
    public GameObject OldGameObject;
    /// <summary>
    /// 基础数据
    /// </summary>
    public int Layer;
    public Vector3 LocalPosition;
    public Vector3 LocalScale;
    public Quaternion LocalRotation;
    public bool SelfActive = true;

    public bool isParticle = false;
    public bool isParticleRootNode = false;
    public string particleRelativePath = "";
    public Transform ParticleRootNode;
    public Vector3 particleOffset;
    public Vector3 particleRotation;
    /// <summary>
    /// 控制器
    /// </summary>
    [NonSerialized]
    public System.Object Ctrl;

    public ActorTrackGroup[] TrackGroups;
    public int[] ParticleGroups;
    public TiedGameObject[] TiedObjs;
    /// <summary>
    /// 判断当前是否要替换为自己的角色
    /// </summary>
    public bool IsSelf { get { return ReplaceId == "player"; } }
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    /// <param name="movieData"></param>
    public MovieReplaceInfo(string replaceId, string resName, GameObject obj, MovieData movieData)
    {
        CurrMovieData = movieData;
        ReplaceId = replaceId;
        ResName = resName;
        CurrObjLevel = 0;
        if (obj == null) return;
        OldGameObject = obj;
        SelfActive = OldGameObject.activeSelf;
        CurrObjctName = obj.name;
        var tra = obj.transform;
        var url = movieData.GetUrl(tra, out CurrObjLevel);
        LocalPosition = tra.localPosition;
        LocalScale = tra.localScale;
        LocalRotation = tra.localRotation;
        Layer = obj.layer;
        if (CurrObjLevel > 0) CurrObjctUrlbase = url.Substring(0, url.LastIndexOf(CurrObjctName) - 1);

        var anim = obj.GetComponent<Animation>();
        IsAnimation = anim != null;
        var animator = obj.GetComponent<Animator>();
        IsAnimator = animator != null;
        if (IsAnimation && IsAnimator)
        {
            Debug.LogError("剧情物体 " + CurrObjctUrlbase + "/" + CurrObjctName + "  同时存在 Animator 及 Animation");
        }
        if (animator)
        {
            CurrAnimationController = animator.runtimeAnimatorController.name;
        }
        else if (IsAnimation)
        {
            CurrAnimationController = anim.name;
        }
        else
        {
            CurrAnimationController = "";

        }
        var particle = obj.GetComponentInChildren<ParticleSystem>();
        var setting = obj.GetComponent<EffectSetting>();
        isParticle = (particle != null) || (setting!=null);

        if (isParticle)
        {
            if (setting != null)
            {
                ParticleRootNode = setting.rootNode;
                particleRelativePath = setting.relativePath;
                particleOffset = setting.offset;
                particleRotation = setting.rotation;
            }
        }
        else
        {
            if (setting != null)
            {
                ParticleRootNode = setting.rootNode;
                particleRelativePath = setting.relativePath;
                particleOffset = setting.offset;
                particleRotation = setting.rotation;
            }
        }

        List<ActorTrackGroup> groups = new List<ActorTrackGroup>();
        List<TiedGameObject> tiedObjs = new List<TiedGameObject>();
        foreach (TrackGroup g in movieData.cutscene.TrackGroups)
        {
            if (g is ActorTrackGroup)
            {
                ActorTrackGroup ag = (ActorTrackGroup)g;
                if (ag.Actor == obj.transform)
                    groups.Add(ag);
            }

           TimelineTrack[] tracks = g.GetTracks();
            for (int j = 0; j < tracks.Length; j++)
            {
               TimelineItem[] items = tracks[j].GetTimelineItems();
                for (int k = 0; k < items.Length; k++)
                {
                    GameObject[] objs = items[k].GetTiedGameObject();
                    if (objs  !=null && objs.Length > 0)
                    {
                        for (int m = 0; m < items.Length; m++)
                        {
                            if (objs[m] == obj)
                            {
                                TiedGameObject tie = new TiedGameObject();
                                tie.item = items[k];
                                tie.index = m;
                                tiedObjs.Add(tie);
                            }
                        }
                    }
                }
            }

        }
        TiedObjs = tiedObjs.ToArray();
        TrackGroups = groups.ToArray();
    }
    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="typeString"></param>
    private void destroyImmediate(GameObject obj, string typeString)
    {
        if (obj.GetComponent(typeString))
        {
            MonoBehaviour.DestroyImmediate(obj.GetComponent(typeString));
        }
    }
    /// <summary>
    /// 数据拷贝
    /// </summary>
    public void CopyDateToTra(Transform newtra)
    {
        if (newtra == null) return;
        ///销毁替换物体 
        if (OldGameObject != null)
        {
            OldGameObject.name = "_del";
            OldGameObject.SetActive(false);
            MonoBehaviour.Destroy(OldGameObject);
        }
        OldGameObject = null;
        Transform parent = CurrMovieData.transform;
        if (!string.IsNullOrEmpty(CurrObjctUrlbase)) parent = CurrMovieData.FindTra(CurrObjctUrlbase);
        newtra.name = CurrObjctName;
        newtra.parent = parent;
        newtra.localPosition = LocalPosition;
        newtra.localRotation = LocalRotation;
        newtra.localScale = LocalScale;
        newtra.gameObject.SetActive(SelfActive);
        ///当前玩家的话直接返回处理
        if (IsSelf) return;

    }
}
