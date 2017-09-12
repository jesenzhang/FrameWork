
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CinemaDirector;

/// <summary>
/// 剧情动画配置数据
/// </summary>
/// <seealso cref="UnityEngine.MonoBehaviour" />
public class MovieData : MonoBehaviour
{
    /// <summary>
    /// 音效
    /// </summary>
    public static float EffectVolume = 1;
    /// <summary>
    /// 音乐控制
    /// </summary>
    public static float SoundVOLUME = 1;
    /// <summary>
    /// 当前播放的职业类型
    /// </summary>
    public static int CurrProfessionType = 1;
    /// <summary>
    /// 可替换结构
    /// </summary>
    public SerializableDictionaryGameObject replaceDict = new SerializableDictionaryGameObject();
    public List<MovieReplaceInfo> MovieReplaceInfos;
    /// <summary>
    /// 动画控制器
    /// </summary>
    public Cutscene cutscene;

    private Transform _transform;

    private void Awake()
    {
        enabled = false;
        _transform = transform;
    }

    #region 编辑器调用
    /// <summary>
    /// 设置AnimationOrAnimatorInfo
    /// </summary>
    public void SetMovieReplaceInfos()
    {
        MovieReplaceInfos = null;
        if (replaceDict == null || replaceDict.Count == 0) return;
        MovieReplaceInfos = new List<MovieReplaceInfo>(replaceDict.Count);
        GameObject obj;
        foreach (var item in replaceDict)
        {
            obj = item.Value.Obj;
            if (obj == null)
            {
                MovieReplaceInfos.Add(null);
                continue;
            }

            MovieReplaceInfos.Add(new MovieReplaceInfo(item.Key,item.Value.ResName, obj, this));

        }
        foreach (var info in MovieReplaceInfos)
        {
            if (info.isParticle)
            {
                Transform infotra = info.ParticleRootNode;
                    foreach (var item in MovieReplaceInfos)
                    {
                        Transform tra = item.OldGameObject.transform;
                        if (infotra == tra)
                        {
                            item.isParticleRootNode = true;
                            if (item.ParticleGroups == null)
                            {
                                item.ParticleGroups = new int[0];
                            }
                            List<int> temp = new List<int>(item.ParticleGroups);
                            temp.Add(MovieReplaceInfos.IndexOf(info));
                            item.ParticleGroups = temp.ToArray();
                            break;
                        }
                    }
                

            }
        }

        //MovieReplaceInfos.Sort(MovieReplaceInfosSort);
    }


    //重铸替换对象
    public void ReplaceObj(string id, GameObject tobj)
    {
        for (int i = 0; i < MovieReplaceInfos.Count; i++)
        {
            MovieReplaceInfo info = MovieReplaceInfos[i];
            if (id == info.ReplaceId)
            {
                GameObject oldobj = info.OldGameObject;
                GameObject obj = GameObject.Instantiate<GameObject>(tobj);
                obj.name = info.OldGameObject.name;
                obj.transform.SetParent(oldobj.transform.parent);
                info.OldGameObject = obj;
                if (info.isParticleRootNode)
                {
                    for (int j = 0; j < info.ParticleGroups.Length; j++)
                    {
                        int index = info.ParticleGroups[j];
                        MovieReplaceInfos[index].ParticleRootNode = obj.transform;
                    }
                }

                obj.transform.localPosition = info.LocalPosition;
                obj.transform.localRotation = info.LocalRotation;
                obj.transform.localScale = info.LocalScale;
                obj.SetActive(info.SelfActive);
                obj.layer = info.Layer;

                Transform[] alltranforms = obj.GetComponentsInChildren<Transform>();
                foreach (Transform t in alltranforms)
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Sequence");
                }


                for (int j = 0; j < info.TrackGroups.Length; j++)
                {
                    info.TrackGroups[j].Actor = obj.transform;
                }

                for (int j = 0; j < info.TiedObjs.Length; j++)
                {
                    int index = info.TiedObjs[j].index;
                    info.TiedObjs[j].item.SetTieGameObject(obj, index);
                }

                GameObject.DestroyImmediate(oldobj);
                GameObject.DestroyImmediate(tobj);
            }
        }
    }
    //设置粒子位置
    public void ReTieParticles()
    {
        for (int i = 0; i < MovieReplaceInfos.Count; i++)
        {
            MovieReplaceInfo info = MovieReplaceInfos[i];
            if (info.isParticle)
            {
                if (info.ParticleRootNode != null)
                {
                    if (info.particleRelativePath != "")
                    {
                        Transform node = info.ParticleRootNode.Find(info.particleRelativePath);
                        if (node == null)
                        {
                            Debug.LogWarningFormat("EffectSetting root path of {0} not found", info.particleRelativePath);

                        }
                        else
                        {
                            var old = info.OldGameObject.transform;
                            info.OldGameObject.transform.SetParent(node);
                            old.localScale = Vector3.one;
                            old.localPosition = info.particleOffset;
                            old.localRotation = Quaternion.Euler(info.particleRotation);
                        }
                    }
                    else
                    {
                        var old = info.OldGameObject.transform;
                        info.OldGameObject.transform.SetParent(info.ParticleRootNode);
                        old.localScale = Vector3.one;
                        old.localPosition = info.particleOffset;
                        old.localRotation = Quaternion.Euler(info.particleRotation);
                    }
                }
            }
        }
    }

    public MovieReplaceInfo GetGameObjectByKey(string key)
    {
        for (int i = 0; i < MovieReplaceInfos.Count; i++)
        {
            if (MovieReplaceInfos[i].ReplaceId == key)
            {
                return MovieReplaceInfos[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 排序规则方便运行时替换物体
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int MovieReplaceInfosSort(MovieReplaceInfo a, MovieReplaceInfo b)
    {
        return a.CurrObjLevel >= b.CurrObjLevel ? 1 : -1;
    }
    #endregion
    /// <summary>
    /// 通过物体的引用名字find 一个物体
    /// </summary>
    /// <returns></returns>
    public Transform FindTra(string url)
    {
        if (_transform == null) _transform = transform;
        if (_transform == null)
        {
            Debug.LogError("MovieData _transform is null");
            return null;
        }
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("要获取的物体 URL is null");
            return null;
        }
        return _transform.Find(url);
    }
    /// <summary>
    /// 获取控制对象相对于MovieData 对象的相对位置
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public string GetUrl(Transform tra)
    {
        int level = 0;
        return GetUrlBase(tra, out level);
    }
    /// <summary>
    ///  获取控制对象相对于MovieData 对象的相对位置 并返回所在的层级
    /// </summary>
    /// <param name="tra"></param>
    /// <param name="levelNum"></param>
    /// <returns></returns>
    public string GetUrl(Transform tra, out int levelNum)
    {
        return GetUrlBase(tra, out levelNum);
    }
    /// <summary>
    /// 获取相对url
    /// </summary>
    /// <param name="tra"></param>
    /// <returns></returns>
    private string GetUrlBase(Transform tra, out int levelNum)
    {
        levelNum = 0;
        if (tra == null) return null;
        if (_transform == null) _transform = transform;
        string url = tra.name;
        var tparent = tra.parent;
        while (tparent != null && tparent != _transform)
        {
            levelNum++;
            url = tparent.name + "/" + url;
            tparent = tparent.parent;
        }
        return url;
    }


}