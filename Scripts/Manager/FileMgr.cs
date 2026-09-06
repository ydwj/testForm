using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System.Threading.Tasks;

/// <summary>
/// 配置储存管理
/// </summary>
public class FileMgr : UnitySingleton<FileMgr>
{
    //缓存目录
    private Dictionary<string, string> configInfo = new Dictionary<string, string>();
    //文件流
    private FileStream fileStream;
    //读取器
    private StreamReader streamReader;
    private StringBuilder saveString = new StringBuilder();

    private BinaryFormatter binary;

    public string UserKey { get; set; }

    public FileMgr()
    {
        binary = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        var streamingContext = new StreamingContext(StreamingContextStates.All);
        ss.AddSurrogate(typeof(Vector3), streamingContext, new Vector3SerializationSurrogate());
        ss.AddSurrogate(typeof(Quaternion), streamingContext, new QuaternionSerializationSurrogate());
        binary.SurrogateSelector = ss;
    }


    //获取文件流读取器
    private StreamReader GetFileStreamReader()
    {
        if (fileStream == null)
        {
            StringBuilder configPath = new StringBuilder();
            configPath.Append(Application.persistentDataPath).Append("/").Append("commonConfig.cfg");
            fileStream = new FileStream(configPath.ToString(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            streamReader = new StreamReader(fileStream);
        }
        return streamReader;
    }
    /// <summary>
    /// 初始话本地储存数据
    /// </summary>
    public void Init()
    {
        streamReader = GetFileStreamReader();
        string line;
        string[] key_v;
        while (!streamReader.EndOfStream)
        {
            line = streamReader.ReadLine();
            key_v = line.Split(':');
            if (key_v.Length == 2)
            {
                configInfo.Add(key_v[0], key_v[1]);
            }
        }
    }
    /// <summary>
    /// 从本地储存获取值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetValue(string key)
    {
        key = UserKey + key;
        if (configInfo.ContainsKey(key))
        {
            return configInfo[key];
        }

        return null;
    }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddValue(string key, string value)
    {
        key = UserKey + key;
        if (configInfo.ContainsKey(key))
        {
            configInfo[key] = value;
        }
        else
        {
            configInfo.Add(key, value);
        }
    }
    /// <summary>
    /// 把数据储存到本地
    /// </summary>
    public void Save()
    {
        lock (this)
        {
            if (fileStream == null)
            {
                GetFileStreamReader();
            }
            fileStream.SetLength(0);
            if (saveString.Length > 0)
                saveString.Remove(0, saveString.Length);

            foreach (KeyValuePair<string, string> key_v in configInfo)
            {
                saveString.Append(key_v.Key).Append(":").Append(key_v.Value).Append("\n");
            }
            byte[] d = System.Text.Encoding.Default.GetBytes(saveString.ToString());
            fileStream.Write(d, 0, d.Length);
            fileStream.Close();
            fileStream = null;
            streamReader.Close();
            streamReader = null;
        }
    }

    /// <summary>
	/// 找到对应名字文件存入数据
	/// </summary>
	/// <param name="fileName">文件名</param>
	/// <param name="text">储存内容</param>
	/// <returns></returns>
	public void SaveFile(string fileName, string text)
    {
        FileStream fs = new FileStream(FileUtils.Ins.GetWritePath(fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.SetLength(0);
        byte[] d = System.Text.Encoding.UTF8.GetBytes(text);
        fs.Write(d, 0, d.Length);
        fs.Close();
    }



    /// <summary>
    /// 加载本地配置文件
    /// </summary>
    /// <param name="fileName">要加载的配置文件名</param>
    /// <returns></returns>
    public string LoadFile(string fileName)
    {
        FileStream fs = new FileStream(FileUtils.Ins.GetWritePath(fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        string ret = sr.ReadToEnd();
        fs.Close();
        sr.Close();

        return ret;
    }

    /// <summary>
    /// 序列化数据
    /// </summary>
    public void WriteFile<T>(T data, string fileName)
    {
        try
        {
            string strFilePath = FileUtils.Ins.GetWritePath(fileName);
            Task.Run(() =>
            {
                using (FileStream fs = File.OpenWrite(strFilePath))
                {
                    binary.Serialize(fs, data);
                }
            });
            //TDDebug.DebugLog("----------序列化成功-----------");
            //AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

    }

    /// <summary>
    /// 反序列化数据
    /// </summary>
    public T ReadFile<T>(string fileName)
    {
        try
        {
            string filePath = FileUtils.Ins.GetWritePath(fileName);
            if (!File.Exists(filePath)) return default;
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                return (T)binary.Deserialize(fs);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return default;
        }
    }

    /// <summary>
    /// 反序列化数据2
    /// </summary>
    public bool TryReadFile<T>(string fileName, out T t)
    {
        try
        {
            string filePath = FileUtils.Ins.GetWritePath(fileName);
            if (!File.Exists(filePath))
            {
                t = default;
                return false;
            }
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                t = (T)binary.Deserialize(fs);
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            t = default;
            return false;
        }
    }


    /// <summary>
    /// 删除文件
    /// </summary>
    public void DeleteFile(string fileName)
    {
        try
        {
            string filePath = FileUtils.Ins.GetWritePath(fileName);
            if (!File.Exists(filePath)) return;
            File.Delete(filePath);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return;
        }
    }

    /// <summary>
    /// 清空某文件 （测试）
    /// </summary>
    /// <param name="fileName"></param>
    public void Clear(string fileName)
    {
        FileStream fs = new FileStream(FileUtils.Ins.GetWritePath(fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        byte[] d = new byte[0];
        fs.SetLength(0);
        fs.Write(d, 0, 0);
        fs.Close();
    }
    public bool DeleteAllFile(string fullPath)
    {
        //获取指定路径下面的所有资源文件  然后进行删除
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string FilePath = fullPath + "/" + files[i].Name;
                print(FilePath);
                File.Delete(FilePath);
            }
            return true;
        }
        return false;
    }
}
public sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = info.GetSingle("x");
        v3.y = info.GetSingle("y");
        v3.z = info.GetSingle("z");
        obj = v3;
        return obj;
    }
}

class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion q = (Quaternion)obj;
        info.AddValue("x", q.x);
        info.AddValue("y", q.y);
        info.AddValue("z", q.z);
        info.AddValue("w", q.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion q = (Quaternion)obj;
        q.x = info.GetSingle("x");
        q.y = info.GetSingle("y");
        q.z = info.GetSingle("z");
        q.w = info.GetSingle("w");
        obj = q;
        return obj;
    }
}