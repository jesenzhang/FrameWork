using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using CinemaDirector;
using System.Collections;
using UnityEditorInternal;
using System;

public class SequenceExportor
{
	[MenuItem("Export/Export Sequence")]
    public static void ExportSequence()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if(objs == null || objs.Length == 0)
        {
            Debug.LogError("no sequence selected");
            return;
        }

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        string exportPath = SequenceEditorUtils.PlatformPath(buildTarget);
        if(exportPath == null)
        {
            Debug.LogError("get export path error");
            return;
        }

        exportPath += "Sequence/";

        if(!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        for(int m = 0, mcount = objs.Length; m < mcount; m++)
        {
            UnityEngine.Object obj = objs[m];
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(obj);
            Cutscene sequencer = go.GetComponent<Cutscene>();
            MovieData moviedata = go.GetComponent<MovieData>();
            if (sequencer == null)
            {
                Debug.LogError("target is not sequence->" + go.name);
                return;
            }
            string name = go.name;
            int index = name.IndexOf("(");
            if(index >= 0)
                name = name.Substring(0, index);

            SequenceData data = go.AddComponent<SequenceData>();
            data.cutscene = sequencer;
            data.moviedata = moviedata;
            data.Cameras = go.GetComponentsInChildren<Camera>();
            for (int j = 0; j < data.Cameras.Length; j++)
            {
                if (data.Cameras[j].name == "Main Camera")
                {
                    data.mainindex = j;
                }
            }

            UnityEngine.Object prefab = SequenceEditorUtils.GetPrefab(go, name);
            UnityEngine.Object.DestroyImmediate(go);

            BuildPipeline.BuildAssetBundle(prefab, null, exportPath + prefab.name, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
        }
    }

    public static void ExportSequenceObj(UnityEngine.Object[] objs)
    {
        if (objs == null || objs.Length == 0)
        {
            Debug.LogError("no sequence selected");
            return;
        }

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        string exportPath = SequenceEditorUtils.PlatformPath(buildTarget);
        if (exportPath == null)
        {
            Debug.LogError("get export path error");
            return;
        }

        exportPath += "Sequence/";

        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        for (int m = 0, mcount = objs.Length; m < mcount; m++)
        {
            UnityEngine.Object obj = objs[m];
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(obj);
            Cutscene sequencer = go.GetComponent<Cutscene>();
            MovieData moviedata = go.GetComponent<MovieData>();
            if (sequencer == null)
            {
                Debug.LogError("target is not sequence->" + go.name);
                return;
            }
            string name = go.name;
            int index = name.IndexOf("(");
            if (index >= 0)
                name = name.Substring(0, index);

            SequenceData data = go.AddComponent<SequenceData>();
            data.cutscene = sequencer;
            data.moviedata = moviedata;
            data.Cameras = go.GetComponentsInChildren<Camera>();
            for (int j = 0; j < data.Cameras.Length; j++)
            {
                if (data.Cameras[j].name == "Main Camera")
                {
                    data.mainindex = j;
                }
            }

            UnityEngine.Object prefab = SequenceEditorUtils.GetPrefab(go, name);
            UnityEngine.Object.DestroyImmediate(go);

            BuildPipeline.BuildAssetBundle(prefab, null, exportPath + prefab.name, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
        }
    }
    /*static void ExportSequence()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if(objs == null || objs.Length == 0)
        {
            Debug.LogError("no sequence selected");
            return;
        }

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        string exportPath = SequenceEditorUtils.PlatformPath(buildTarget);
        if(exportPath == null)
        {
            Debug.LogError("get export path error");
            return;
        }

        exportPath += "Sequence/";

        if(!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        for(int m = 0, mcount = objs.Length; m < mcount; m++)
        {
            Object obj = objs[m];
            GameObject go = (GameObject)Object.Instantiate(obj);
            USSequencer sequencer = go.GetComponent<USSequencer>();
            if(sequencer == null)
            {
                Debug.LogError("target is not sequence->" + go.name);
                return;
            }
            string name = go.name;
            int index = name.IndexOf("(");
            if(index >= 0)
                name = name.Substring(0, index);

            SequenceData data = go.AddComponent<SequenceData>();
            data.sequencer = sequencer;
            USTimelineContainer[] arr = go.GetComponentsInChildren<USTimelineContainer>();
            for(int i = 0, count = arr.Length; i < count; i++)
            {
                USTimelineContainer container = arr[i];
                data.timelines.Add(container);
                string str = container.AffectedObjectPath;
                string[] strArr = str.Split("/");
                str = "/";
                for(int j = 1, jcount = strArr.Length; j < jcount; j++)
                {
                    if(j == 1)
                        str += strArr[j] + "(Clone)";
                    else
                        str += strArr[j];

                    if(j < jcount - 1)
                        str += "/";
                }
                data.timelinesObjectPath.Add(str);
            }

            Object prefab = SequenceEditorUtils.GetPrefab(go, name);
            Object.DestroyImmediate(go);

            BuildPipeline.BuildAssetBundle(prefab, null, exportPath + prefab.name, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, buildTarget);
        }
    }
    */

    [MenuItem("Export/导出可替换的剧情资源")]
    public static void ExportExchangeSequence()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (objs == null || objs.Length == 0)
        {
            Debug.LogError("no sequence selected");
            return;
        }

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;



        string exportPath = SequenceEditorUtils.PlatformPath(buildTarget);
        if (exportPath == null)
        {
            Debug.LogError("get export path error");
            return;
        }

        exportPath += "Sequence/";

        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        for (int m = 0, mcount = objs.Length; m < mcount; m++)
        {
            UnityEngine.Object obj = objs[m];
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(obj);
            go.name = obj.name;
            Cutscene sequencer = go.GetComponent<Cutscene>();
            MovieData moviedata = go.GetComponent<MovieData>();
            if (sequencer == null)
            {
                Debug.LogError("target is not sequence->" + go.name);
                return;
            }
            if (moviedata == null)
            {
                Debug.LogError("moviedata is null->" + go.name);
                return;
            }
            CheckOut(moviedata);
           }
    }

    static public void CheckOut(MovieData movieItem)
    {
        List<string> paths = new List<string>();
        string[] strAtrr = Application.dataPath.Split('/');
        string projectName = strAtrr[strAtrr.Length - 2];
        GameObject go = movieItem.gameObject;
        // 确认打包数据抽取
        exportPackage(projectName, go, paths, true);
    }
    
    /// <summary>
    /// 将剧情导出资产
    /// </summary>
    /// <param name="projectName">Name of the project.</param>
    /// <param name="obj">The object.</param>
    /// <param name="list">The list.</param>
    /// <param name="destroyAssets">if set to <c>true</c> [destroy assets].</param>
    public static void exportPackage(string projectName, GameObject item, List<string> list, bool destroyAssets)
    {
        // 创建一个新的对象用于导出
        GameObject obj = MonoBehaviour.Instantiate(item) as GameObject;
        obj.name = item.name;
        MovieData mit = obj.GetComponent<MovieData>();
        #region 重构显示对象节点结构数据

        // 创建不可删除点hash
        HashSet<GameObject> dontDstory = new HashSet<GameObject>();

        ///设置剧情配置的引用
        mit.SetMovieReplaceInfos();

        // 保护绑点数据结构不被删除
        foreach (var i in mit.replaceDict)
        {
            // 保护绑点数据结构不被删除
            GameObject reObj = i.Value.Obj;
            transformCheck(reObj.transform);
            if (reObj == null) continue;
            if (dontDstory.Contains(reObj) == false) dontDstory.Add(reObj);
            while (reObj.transform.parent != null)
            {
                if (dontDstory.Contains(reObj) == false) dontDstory.Add(reObj);
                reObj = reObj.transform.parent.gameObject;
            }
            reObj = i.Value.Obj;
            // 需要进行删除筛选的OBJ根节点
            List<GameObject> removeObj = new List<GameObject>();
            for (int j = 0; j < reObj.transform.childCount; j++)
            {
                GameObject tmp = reObj.transform.GetChild(j).gameObject;
                // 该节点为需要删除筛选的节点
                if (!dontDstory.Contains(tmp))
                {
                    removeObj.Add(tmp);
                }

            }
            for (int k = 0; k < removeObj.Count; k++)
            {
                GameObject.DestroyImmediate(removeObj[k].gameObject);
            }
        }
        // 保存该物件预制体
        if (!Directory.Exists("Assets/temp"))
        {
            Directory.CreateDirectory("Assets/temp");
        }
        PrefabUtility.CreatePrefab("Assets/temp/" + obj.name + ".prefab", obj);
        SequenceExportor.ExportSequenceObj(new UnityEngine.Object[] { obj });

        AssetDatabase.DeleteAsset("Assets/temp/" + obj.name + ".prefab");
        GameObject.DestroyImmediate(obj);
        GameObject.DestroyImmediate(item);
        
        #endregion




    }

    private static void transformCheck(Transform tf)
    {
        destoryCommon(tf.gameObject, typeof(Animator));
        destoryCommon(tf.gameObject, typeof(SkinnedMeshRenderer));
        destoryCommon(tf.gameObject, typeof(ParticleSystem));
        int c = tf.childCount;
        for (int i = 0; i < c; i++)
        {
            transformCheck(tf.GetChild(i));
        }
    }

    private static void destoryCommon(GameObject currentTran, Type typeString)
    {
        if (currentTran.GetComponent(typeString))
        {
           GameObject.DestroyImmediate(currentTran.GetComponent(typeString));
        }
    }

    /// <summary>
    /// 针对当前对象的transform确定是否应该保留该节点
    /// </summary>
    /// <param name="trans">The trans.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private void remove(Transform trans, HashSet<GameObject> replaceDict)
    {
        Transform[] gos = new Transform[trans.childCount];
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i] = trans.GetChild(i);
        }
        // 删除节点
        for (int i = 0; i < gos.Length; i++)
        {
            if (replaceDict.Contains(gos[i].gameObject) == false)
            {
                GameObject.DestroyImmediate(gos[i].gameObject);
            }
            else
            {
                remove(gos[i], replaceDict);
            }
        }
    }

    private bool CreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return true;
    }
}
