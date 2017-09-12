using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using CinemaDirector;
using System.IO;
using System;


[CustomEditor(typeof(MovieData))]
public class MovieDataEditor : Editor
{
    private MovieData movieItem;

    private List<string> keys;

    private ReorderableList replaceList;
    private Vector2 replace;
    private bool isShowdic;
    GUIContent showdic = new GUIContent("显示详细信息", "显示详细信息 主要是显示隐藏的序列化数据方便查看数据");
    GUIContent noShowdic = new GUIContent("隐藏详细信息", "隐藏详细信息 主要是显示隐藏的序列化数据方便查看数据");
    /// <summary>
    /// 绘制界面
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (movieItem == null)
        {
            movieItem = target as MovieData;
            keys = new List<string>();
            foreach (var item in movieItem.replaceDict)
            {
                keys.Add(item.Key);
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical("Box");
        DrawCutscene();
        DrawReplay();
        CheckOut(movieItem);
        EditorGUILayout.EndVertical();

        if (isShowdic ? GUILayout.Button(noShowdic) : GUILayout.Button(showdic))
        {
            isShowdic = !isShowdic;
        }
        if (isShowdic) base.OnInspectorGUI();
    }

    /// <summary>
    /// 绘制剧情编辑结果的设置控件
    /// </summary>
    private void DrawCutscene()
    {
        movieItem.cutscene = EditorGUILayout.ObjectField("剧情控制器：", movieItem.cutscene, typeof(Cutscene), true) as Cutscene;
    }

    /// <summary>
    /// 绘制需要重置的界面配置
    /// </summary>
    private void DrawReplay()
    {
        replace = EditorGUILayout.BeginScrollView(replace, GUILayout.Height(200));

        if (replaceList == null)
        {
            replaceList = new ReorderableList(keys, typeof(List<string>), false, false, true, true);
        }

        replaceList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            string id = keys[index];
            EditorGUI.BeginChangeCheck();
            id = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "替换编号：", id);
            if (EditorGUI.EndChangeCheck())
            {
                if (movieItem.replaceDict.ContainsKey(id) == false)
                {
                    movieItem.replaceDict.Remove(keys[index]);
                    keys[index] = id;
                    movieItem.replaceDict.Add(keys[index], null);
                }
                else
                {
                    id = keys[index];
                }
            }
            movieItem.replaceDict[id].ResName = EditorGUI.TextField(new Rect(rect.x, rect.y + 20, rect.width, EditorGUIUtility.singleLineHeight), "资源名称：", movieItem.replaceDict[id].ResName);
            movieItem.replaceDict[id].Obj = EditorGUI.ObjectField(new Rect(rect.x, rect.y + 40, rect.width, EditorGUIUtility.singleLineHeight), "替换物件：", movieItem.replaceDict[id].Obj, typeof(GameObject), true) as GameObject;
        };
        // 绘制表头
        replaceList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "替换对象");
        };
        // 选择回调
        replaceList.onSelectCallback = (ReorderableList l) =>
        {
        };
        replaceList.onRemoveCallback = (ReorderableList l) =>
        {
            if (movieItem.replaceDict.ContainsKey(keys[l.index]) == false)
            {
                movieItem.replaceDict.Remove(keys[l.index]);
            }
            keys.RemoveAt(l.index);
        };
        replaceList.onAddCallback = (ReorderableList l) =>
        {
            string id = "请修改该参数";
            keys.Add(id);
            if (movieItem.replaceDict.ContainsKey(id) == false)
            {
                movieItem.replaceDict.Add(id, null);
            }
        };
        replaceList.elementHeight = 60;
        replaceList.DoLayoutList();
        EditorGUILayout.EndScrollView();

    }
    GameObject replaceobj;
    public void CheckOut(MovieData movieItem)
    {
        if (GUILayout.Button("播放"))
        {
            movieItem.cutscene.Play();
        }

        replaceobj = EditorGUILayout.ObjectField("replaceobj：", replaceobj, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("替换资源"))
        {
            movieItem.ReplaceObj("player", replaceobj);
        }

        if (GUILayout.Button("生成导出数据资源"))
        {
            movieItem.SetMovieReplaceInfos();
        }
        if (GUILayout.Button("导出资源"))
        {
            Debug.Log("导出资源");
            List<string> paths = new List<string>();
            string[] strAtrr = Application.dataPath.Split('/');
            string projectName = strAtrr[strAtrr.Length - 2];
            GameObject go = movieItem.gameObject;

            // 确认打包数据抽取
            exportPackage(projectName, go, paths, true);

        }
    }

    public const string BasePath = "MyAssets/Movies";
    public const string ResPath = "res";
    public const string ShaderUrl = "Assets/Shader";
    public const string ActorsUrl = "Assets/Actors";
    private Dictionary<string, string> _formattingResName;
    /// <summary>
    /// 将剧情导出资产
    /// </summary>
    /// <param name="projectName">Name of the project.</param>
    /// <param name="obj">The object.</param>
    /// <param name="list">The list.</param>
    /// <param name="destroyAssets">if set to <c>true</c> [destroy assets].</param>
    public void exportPackage(string projectName, GameObject item, List<string> list, bool destroyAssets)
    {
   
        // 创建一个新的对象用于导出
        GameObject obj = Instantiate(item) as GameObject;
        obj.name = item.name;
        MovieData mit = obj.GetComponent<MovieData>();

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
            for (int k = 0; k < removeObj.Count;k++)
            {
                GameObject.DestroyImmediate(removeObj[k].gameObject);
            }
        }

        if (!Directory.Exists("Assets/temp"))
        {
            Directory.CreateDirectory("Assets/temp");
        }
        // 保存该物件预制体
        GameObject prefab = PrefabUtility.CreatePrefab("Assets/temp/"+obj.name+".prefab" , obj);
        SequenceExportor.ExportSequenceObj(new UnityEngine.Object[] { prefab });

        AssetDatabase.DeleteAsset("Assets/temp/" + obj.name + ".prefab");
        GameObject.DestroyImmediate(obj);

    }

    private void transformCheck(Transform tf)
    {
        destoryCommon(tf.gameObject, typeof(Animator));
        destoryCommon(tf.gameObject, typeof(SkinnedMeshRenderer));
        destoryCommon(tf.gameObject, typeof(ParticleSystem));
        destoryCommon(tf.gameObject, typeof(EffectSetting));
        int c = tf.childCount;
        for (int i = 0; i < c; i++)
        {
            transformCheck(tf.GetChild(i));
        }
    }

    private void destoryCommon(GameObject currentTran, Type typeString)
    {
        if (currentTran.GetComponent(typeString))
        {
            DestroyImmediate(currentTran.GetComponent(typeString));
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