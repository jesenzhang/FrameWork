using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public static class SequenceEditorUtils
{
	public static string DataPath()
	{
		return "Export/" + UserName() + "/Data/";
	}

	public static string PlatformPath(BuildTarget buildTarget)
	{
		string platform = "";
		switch(buildTarget)
		{
			case BuildTarget.iOS:
				{
					platform = "iOS";
				}
				break;
			case BuildTarget.Android:
				{
					platform = "Android";
				}
				break;
			case BuildTarget.StandaloneWindows:
				{
					platform = "PC";
				}
				break;
			case BuildTarget.StandaloneWindows64:
				{
					platform = "PC";
				}
				break;
		}

		if(platform == "")
		{
			//Debug.LogError("platform is invaild->" + EditorUserBuildSettings.activeBuildTarget);
			Debug.LogError("platform is invaild->" + buildTarget);
			return null;
		}
		platform += "/";

        if(Application.dataPath.Contains("Client-UI"))
        {
            return "Export/" + platform;
        }
        else
        {
            string userName = System.Environment.UserName;

            return "Export/" + userName + "/" + platform;
        }
	}

	public static string UserName()
	{
		return System.Environment.UserName;
	}

	public static Object GetPrefab(GameObject go, string name)
	{
		string path = "Assets/temp/";
		if(!Directory.Exists(path))
			Directory.CreateDirectory(path);
		Object temp = EditorUtility.CreateEmptyPrefab(path + name + ".prefab");
		temp = EditorUtility.ReplacePrefab(go, temp);
		Object.DestroyImmediate(go);
		return temp;
	}
}
