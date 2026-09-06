using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class FileUtils : Singleton<FileUtils>
{
    /// <summary>
    /// 获取可写目录路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetWritePath(string fileName)
    {
        StringBuilder sb = new StringBuilder();
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            sb.Append(Application.streamingAssetsPath).Append("/").Append(fileName);
        }
        else
        {
            sb.Append(Application.persistentDataPath).Append("/").Append(fileName);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 获取打包资源路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetResPath(string path)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Application.persistentDataPath).Append("/Resources/").Append(path);

        return sb.ToString();
    }
}
