using System;
using System.Text;
using UnityEngine;
using System.IO;

public static class ResourcePath
{
#region PathDir
    private static string m_CSVDir = "CSV/";
    public static string CSVDir
    {
        get
        {
            return m_CSVDir;
        }
    }

    private static string m_FashionDir = "Fashion/";
    public static string FashionDir
    {
        get
        {
            return m_FashionDir;
        }
    }

    private static string m_CSVBundle = "csv";
    public static string CSVBundle
    {
        get
        {
            return m_CSVBundle;
        }
    }

    private static string m_SFXDir = "SFX/";
    public static string SFXDir
    {
        get
        {
            return m_SFXDir;
        }
    } 

    private static string m_PlayerDir = "Player/";
    public static string PlayerDir
    {
        get
        {
            return m_PlayerDir;
        }
    }
    private static string m_CreatureDir = "Creature/";
    public static string CreatureDir
    {
        get
        {
            return m_CreatureDir;
        }
    }
    private static string m_WeaponDir = "Weapon/";
    public static string WeaponDir
    {
        get
        {
            return m_WeaponDir;
        }
    }
    private static string m_StaticDir = "Static/";
    public static string StaticDir
    {
        get
        {
            return m_StaticDir;
        }
    }
    private static string m_EffectDir = "Effect/";
    public static string EffectDir
    {
        get
        {
            return m_EffectDir;
        }
    }
    private static string m_NpcDir = "Npc/";
    public static string NpcDir
    {
        get
        {
            return m_NpcDir;
        }
    }

    private static string m_GuiDir = "Gui/";
    public static string GuiDir
    {
        get
        {
            return m_GuiDir;
        }
    }

    private static string m_GameDir = "Game/";
    public static string GameDir
    {
        get
        {
            return m_GameDir;
        }
    }

    private static string m_SceneDir = "Scene/";
    public static string SceneDir
    {
        get
        {
            return m_SceneDir;
        }
    }
    private static string m_QuestDir = "QuestState/";
    public static string QuestDir
    {
        get
        {
            return m_QuestDir;
        }
    }
    private static string m_ItemDir = "Item/";
    public static string ItemDir
    {
        get
        {
            return m_ItemDir;
        }
    }
#endregion

    private static StringBuilder m_localPath;
    private static string productDir = "/Craftman/LOST/";
    private static string m_RawFileDir = null;
    private const string Environment_MEDIA_MOUNTED = "mounted";

    public static string GetRealExternalStorageDirectory()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment"))
        {
            if (Environment.CallStatic<string>("getExternalStorageState") != Environment_MEDIA_MOUNTED)
            {
                Debuger.Log("Environment_MEDIA_MOUNTED");
                using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        string path = jo.Call<string>("GetDataDirectory");
                        Debuger.Log("GetDataDirectory:" + path);
                        return path;
                    }
                }
            }

            using (AndroidJavaObject externalStorageDirectory = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            {                
                string root = externalStorageDirectory.Call<string>("getPath");
                Debuger.Log("GetRealExternalStorageDirectory:" + root);
                return root;
            }
        }
#else
        return null;
#endif
    }

    public static string GetExternalStorageDirectory()
    {
#if UNITY_ANDROID
        return GetRealExternalStorageDirectory();
#else
        return null;
#endif
    }

    public static string GetPersistentDataPath()
    {
        return Application.persistentDataPath + "/";
    }

    private static void Init()
	{
        string local;
      
		if (Application.platform == RuntimePlatform.Android)
        {
            local = GetPersistentDataPath() + productDir;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
            local = ResourcePath.GetiPhoneDocumentsPath() + productDir;
        }
		else
		{
            local = Application.dataPath + "/StreamingAssets/";
		}

        //Debug.Log(local);
        ResourcePath.m_localPath = new StringBuilder(local);
	}

    public static string GetDataSreamingAssetsPath()
    {
        string path = Application.dataPath + "/StreamingAssets/";
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.dataPath + "!/assets/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Application.dataPath + "/Raw/";
        }
        else
        {
            path = Application.dataPath + "/StreamingAssets/";
        }
        return path;
    }

    private static void InitRawPath()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            m_RawFileDir = "jar:file://" + Application.dataPath + "!/assets/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_RawFileDir = "file://" + Application.dataPath + "/Raw/";
        }
        else
        {
            m_RawFileDir = "file://" + Application.dataPath + "/StreamingAssets/";
        }
    }

    public static string GetLocalUriPath()
    {
        string localDir = GetLocalPath();
        return "file://" + localDir;
    }

    private static string m_GraphmlDir = "Graphml/";
    public static string GraphmlDir
    {
        get
        {
            return m_GraphmlDir;
        }
    }

    public static string GetRawFilePath()
    {
        if (ResourcePath.m_RawFileDir == null)
        {
            ResourcePath.InitRawPath();
        }
        return m_RawFileDir;
    }

    public static string GetLocalPath()
    {
        if (ResourcePath.m_localPath == null)
        {
            ResourcePath.Init();
        }
        return ResourcePath.m_localPath.ToString();
    }
	public static string GetiPhoneDocumentsPath()
	{
		string text = Application.persistentDataPath.Substring(0, Application.persistentDataPath.Length - 5);
		text = text.Substring(0, text.LastIndexOf('/'));
		return Application.temporaryCachePath;// text + "/Documents";
	}

    public static string GetUriScenePath(string sceneName)
    {
        return ResourcePath.GetLocalUriPath() + ResourcePath.SceneDir + sceneName + ".assetbundle";
    }
    public static string GetLocalScenePath(string sceneName)
    {
        return ResourcePath.GetLocalPath() + ResourcePath.SceneDir + sceneName + ".assetbundle";
    }
    public static string GetRawScenePath(string sceneName)
    {
        return ResourcePath.GetRawFilePath() + ResourcePath.SceneDir + sceneName + ".assetbundle";
    }
    public static string GetScenePath(string sceneName)
    {
        return ResourcePath.GetLocalPath() + ResourcePath.SceneDir + sceneName + ".assetbundle";
    }

    public static string GetUriSceneShaderPath(string sceneName)
    {
        return ResourcePath.GetLocalUriPath() + ResourcePath.SceneDir + sceneName + "_BindShader.unity3d";
    }
    public static string GetLocalSceneShaderPath(string sceneName)
    {
        return ResourcePath.GetLocalPath() + ResourcePath.SceneDir + sceneName + "_BindShader.unity3d";
    }
    public static string GetRawSceneShaderPath(string sceneName)
    {
        return ResourcePath.GetRawFilePath() + ResourcePath.SceneDir + sceneName + "_BindShader.unity3d";
    }
    public static string GetSceneShaderPath(string sceneName)
    {
        return ResourcePath.GetLocalPath() + ResourcePath.SceneDir + sceneName + "_BindShader.unity3d";
    }
    public static string GetLocalUrlPath()
    {
        string localDir = GetLocalPath();
        return "file://" + localDir;
    }
}
