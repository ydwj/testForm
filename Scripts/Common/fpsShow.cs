/// <summary>
/// Set FPS
/// this script use to set FPS if platform is mobile
/// </summary>
using UnityEngine;
public class fpsShow : MonoBehaviour
{
    public float updateInterval = 0.5f;
    private float lastInterval;
    private int frames = 0;
    private float fps;
    GUIStyle fontStyle;
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        fontStyle = new GUIStyle();
        fontStyle.normal.background = null;    //设置背景填充
        fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
        fontStyle.fontSize = 60;       //字体大小
    }

    // Update is called once per frame  
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow >= lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 100, 50), fps.ToString("f0"), fontStyle);
    }
}