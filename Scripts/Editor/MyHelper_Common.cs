using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;

public static class MyHelper_Common
{
    [MenuItem("GameObject/复制引导路径到剪切板", false, priority = 2)]
    public static void CopyObjectPathToClipboard()
    {
        Transform t = Selection.activeGameObject.transform;
        Stack<string> stackNodesName = new Stack<string>();
        stackNodesName.Push(t.name);

        while (t.parent != null)
        {
            stackNodesName.Push(t.parent.name);

            if (t.parent.GetComponent<Form>() != null)
                break;

            t = t.parent;
        }

        string strPath = "";
        while (stackNodesName.Count > 0)
        {
            strPath = strPath + stackNodesName.Pop() + ';';
        }

        strPath = strPath.TrimEnd(';');
        if (strPath.StartsWith("Form/"))
            strPath = strPath.Substring(5);

        if (!string.IsNullOrEmpty(strPath))
            GUIUtility.systemCopyBuffer = strPath;
    }

    [MenuItem("GameObject/复制LocalPosition到剪贴板", false, priority = 2)]
    public static void CopyLocalPositionToClipboard()
    {
        Transform t = Selection.activeTransform;
        string strPath = "";
        strPath = t.localPosition.x.ToString("0.00") + ";" + t.localPosition.y.ToString("0.00") + ";" + t.localPosition.z.ToString("0.00");

        if (!string.IsNullOrEmpty(strPath))
            GUIUtility.systemCopyBuffer = strPath;
    }

    [MenuItem("GameObject/主场景相机归位", false, priority = 2)]
    public static void ResetMainCamera()
    {
        Transform t = Selection.activeTransform;
        if (t.TryGetComponent(out Camera c))
        {        
            t.position = new Vector3(93.6f, 69f, 52.8f);
            t.rotation = Quaternion.Euler(49, -132, 0);
            c.fieldOfView = 35f;
        
            EditorUtility.SetDirty(t);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("GameObject/UGUI Text清晰化", false, priority = 2)]
    public static void UGUITextClear()
    {
        Text[] ts = Selection.GetFiltered<Text>(SelectionMode.Unfiltered);
        foreach (var t in ts)
        {
            Undo.RegisterFullObjectHierarchyUndo(t, "UGUI Text Clear");

            if (t.transform.localScale.x >= 1 && t.transform.localScale.x >= 1)
            {
                t.transform.localScale = new Vector3(0.5f,0.5f,1);
                t.fontSize *= 2;
            }
            
            EditorUtility.SetDirty(t);
        }
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/去掉目录所有资源ReadEnable和mipmaps")]
    public static void CheckReadEnable()
    {
        if (Selection.assetGUIDs.Length == 0)
            return;

        var id = Selection.assetGUIDs[0];
        var path = AssetDatabase.GUIDToAssetPath(id);
        path = Application.dataPath + path.Remove(0,6);
        
        if (!Directory.Exists(path))
        {
            Debug.Log("你选中的不是一个目录");
            return;
        }
        
        DirectoryInfo directory = new DirectoryInfo(path);
        var files = directory.GetFiles("*.meta", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite);
            byte[] readBuffer = new byte[(int)stream.Length];
            int nReadLength = stream.Read(readBuffer, 0, (int)stream.Length);
            string strRead = Encoding.UTF8.GetString(readBuffer);

            bool bChanged = false;
            if (strRead.Contains("isReadable: 1"))
            {
                strRead = strRead.Replace("isReadable: 1", "isReadable: 0");
                bChanged = true;
            }
            if (strRead.Contains("enableMipMap: 1"))
            {
                strRead = strRead.Replace("enableMipMap: 1", "enableMipMap: 0");
                bChanged = true;
            }

            if (bChanged)
            {
                byte[] writeBuffer = Encoding.UTF8.GetBytes(strRead);
                stream.SetLength(0);
                stream.Write(writeBuffer, 0, writeBuffer.Length);
            }

            stream.Close();
            stream.Dispose();
        }

        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/修改目录所有fbx动作loopMode为true")]
    public static void SetAllFbxAnimationLoop()
    {
        if (Selection.assetGUIDs.Length == 0)
            return;

        var id = Selection.assetGUIDs[0];
        var path = AssetDatabase.GUIDToAssetPath(id);
        path = Application.dataPath + path.Remove(0,6);
        
        if (!Directory.Exists(path))
        {
            Debug.Log("你选中的不是一个目录");
            return;
        }
        
        DirectoryInfo directory = new DirectoryInfo(path);
        var files = directory.GetFiles("*.meta", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite);
            byte[] readBuffer = new byte[(int)stream.Length];
            int nReadLength = stream.Read(readBuffer, 0, (int)stream.Length);
            string strRead = Encoding.UTF8.GetString(readBuffer);

            bool bChanged = false;
            if (strRead.Contains("loopTime: 0"))
            {
                strRead = strRead.Replace("loopTime: 0", "loopTime: 1");
                bChanged = true;
            }

            if (bChanged)
            {
                byte[] writeBuffer = Encoding.UTF8.GetBytes(strRead);
                stream.SetLength(0);
                stream.Write(writeBuffer, 0, writeBuffer.Length);
            }

            stream.Close();
            stream.Dispose();
        }

        AssetDatabase.Refresh();
    }
    
    // [MenuItem("Assets/修改目录所有fbx动作loopMode为true")]
    // public static void SetAllFbxAnimationLoop()
    // {
    //     if (Selection.assetGUIDs.Length == 0)
    //         return;
    //
    //
    //     var allGos = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
    //     foreach (var o in allGos)
    //     {
    //         if (!(o is GameObject go))
    //             continue;
    //         
    //         var path = AssetDatabase.GetAssetPath(go);
    //         AssetImporter assetImporter = ModelImporter.GetAtPath(path);
    //         if (!(assetImporter is ModelImporter importer))
    //             continue;   
    //         
    //         ModelImporterClipAnimation[] clipAnimations = importer.defaultClipAnimations;
    //         foreach (var clip in clipAnimations)
    //         {
    //             clip.loop = true;
    //             clip.loopTime = true;
    //         }
    //
    //         importer.clipAnimations = clipAnimations;
    //         importer.SaveAndReimport();
    //         EditorUtility.SetDirty(go);
    //     }
    //     
    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();
    // }
    
    [MenuItem("GameObject/Sprite转Image", false, priority = 2)]
    public static void SpriteToImage()
    {
        SpriteRenderer[] ts = Selection.GetFiltered<SpriteRenderer>(SelectionMode.Unfiltered);
        for (int i = ts.Length - 1; i >= 0; i--)
        {
            var t = ts[i];
            Undo.RegisterFullObjectHierarchyUndo(t, "Sprite To Image");

            Image img = t.gameObject.AddComponent<Image>();
            img.sprite = t.sprite;
            img.SetNativeSize();
            
            Object.DestroyImmediate(t,true);

            EditorUtility.SetDirty(img);
        }
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/清除游戏数据", false, priority = 2000)]
    public static void DeleteGameData()
    {
        string strDataPath = "Assets/StreamingAssets/gameData.data";

        AssetDatabase.DeleteAsset(strDataPath);
        AssetDatabase.Refresh();
    }
    
    [MenuItem("GameObject/MyHelper/CreateMyText", false, priority = 2)]
    public static void CreateMyText()
    {
        Transform parent = Selection.activeTransform;
        EditorApplication.ExecuteMenuItem("GameObject/UI/Text");
        GameObject goNew = Selection.activeGameObject;
        goNew.transform.parent = parent;
        goNew.transform.localPosition = Vector3.zero;

        Text text = goNew.GetComponent<Text>();
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CommonFont.ttf");
        text.color = Color.white;
        text.fontSize = 30;

        EditorUtility.SetDirty(goNew);
        AssetDatabase.Refresh();
    }
}
