using UnityEngine;
using System.Collections.Generic;
using GameBase;
using CinemaDirector;

public class SequenceData : MonoBehaviour
{
    public Cutscene cutscene = null;
    public MovieData moviedata = null;
    private Transform _transform;
    public Camera[] Cameras;
    public int mainindex = 0;
    void Awake()
    {
        //if(sequencer)
        //{
        //	sequencer.PlaybackFinished += OnFinished;

        // }
        if (cutscene)
        {
            cutscene.CutsceneFinished += OnCuteSceneFinished;
            cutscene.OnMessageEvent += OnMessage;
        }
        _transform = transform;

    }

    // Use this for initialization
    void Start()
    {
    }

    void OnEnable()
    {
    }

    public Camera GetMainCamera()
    {
        return Cameras[mainindex];
    }
    public void Stop()
    {
        if (cutscene == null) return;
        cutscene.Pause();
    }

    public void Play()
    {
        if (cutscene == null) return;
        cutscene.Play();
    }

    public void Pause()
    {
        if (cutscene == null) return;
        cutscene.Pause();
    }

    public void Resume()
    {
        if (cutscene == null) return;
        cutscene.Play();
    }
    public void Skip()
    {
        if (cutscene == null) return;
        cutscene.Skip();
    }
    void OnCuteSceneFinished(object sender, CutsceneEventArgs arg)
    {
        if (cutscene == null)
            return;
        LuaInterface.LuaFunction func = LuaManager.GetFunction("SequenceCall.OnFinished");
        string name = cutscene.name;
        int index = name.IndexOf("(");
        if (index >= 0)
            name = name.Substring(0, index);
        LuaManager.CallFunc_VX(func, name);
    }

    public void ReplaceObj(string id, GameObject obj)
    {
        if (moviedata != null)
        {
            moviedata.ReplaceObj(id, obj);
        }
    }

    public void ReTieParticles()
    {
        if (moviedata != null)
        {
            moviedata.ReTieParticles();
        }
    }

    public int GetReplaceCount()
    {
        if (moviedata != null)
        {
            return moviedata.MovieReplaceInfos.Count;
        }
        return 0;
    }

    public string GetReplaceInfoId(int index)
    {
        if (moviedata != null)
        {
            return moviedata.MovieReplaceInfos[index].ReplaceId;
        }
        return "";
    }
    public string GetControllerName(int index)
    {
        if (moviedata != null)
        {
            return moviedata.MovieReplaceInfos[index].CurrAnimationController;
        }
        return "";
    }

    public string GetResName(int index)
    {
        if (moviedata != null)
        {
            return moviedata.MovieReplaceInfos[index].ResName;
        }
        return "";
    }
    void OnMessage(object sender, CutsceneEventArgs arg)
    {
        if (cutscene == null)
            return;
        LuaInterface.LuaFunction func = LuaManager.GetFunction("SequenceCall.OnMessage");
        string name = cutscene.name;
        int index = name.IndexOf("(");
        if (index >= 0)
            name = name.Substring(0, index);
        LuaManager.CallFunc_VX(func, arg.msg, arg.param);
    }
    /* void OnFinished(USSequencer sequencer)
	{
       
		if(sequencer == null)
			return;
		LuaInterface.LuaFunction func = LuaManager.GetFunction("SequenceCall.OnFinished");
		string name = sequencer.name;
		int index = name.IndexOf("(");
		if(index >= 0)
			name = name.Substring(0, index);
		LuaManager.CallFunc_VX(func, name);
       
	} */
    public void Init()
    {
        /*
		try
		{
			if(timelines.Count != timelinesObjectPath.Count)
				return;
			USTimelineContainer container = null;
			GameObject go = null;
			string path = "";
			for(int i = 0, count = timelines.Count; i < count; i++)
			{
				container = timelines[i];
				path = timelinesObjectPath[i];
				if(path.Length < 2)
					continue;
				go = GameObject.Find(path);
				if(!go)
				{
					string[] arr = path.Split("/");
					if(arr.Length < 2)
						continue;
					GameObject root = GameObject.Find("/" + arr[1]);
					if(root)
					{
						path = null; 
						for(int j = 2, jcount = arr.Length; j < jcount; j++)
						{
							if(j < jcount - 1)
								path += arr[j] + "/";
							else
								path += arr[j];
						}
						Debug.LogError("find active false ->" + path);
						Transform temptrans = root.transform.Find(path);
						if(temptrans)
							go = temptrans.gameObject;
					}
				}
				//Debug.LogError("find timeline obj->" + path + "^" + (go == null) + "^" + (GameObject.Find("SequenceSrc(Clone)") == null));
				if(go)
					container.AffectedObject = go.transform;
			}
		}
		catch(System.Exception e)
		{
			Debug.LogError("sequence data init error->" + e.ToString());
		}
		*/
    }
}
