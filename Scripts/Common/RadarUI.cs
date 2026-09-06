using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarUI : BaseMeshEffect
{
    public Color mainColor = Color.blue;
    [Range(0, 1)]
    public List<float> listProgress;
    //顶点数组
    private List<UIVertex> m_VetexList = new List<UIVertex>();
    
    //网格模型顶点数量
    private const int VERTICES_COUNT = 6;
    private const float RADIUS = 45;
    //三角形数组
    List<int> indices = new List<int>();

    public float scale;

    public void SetProgress(int nIndex, float fProgress)
    {
        fProgress = Mathf.Clamp(fProgress, 0, 1);
        listProgress[nIndex] = fProgress;
        Refresh();
    }

    void SetVertices()
    {
        int triangles_count = VERTICES_COUNT - 1;

        indices.Clear();

        //每个三角形角度
        float everyAngle = 360 / triangles_count;
        
        //设定原点坐标
        SetUIVertex(0, new Vector3(0, 0, 0));
        //首个在x轴上的坐标点
        //SetUIVertex(1, new Vector3(RADIUS * Mathf.Cos(Deg2Rad(90 - everyAngle)), RADIUS * Mathf.Sin(Deg2Rad(90 - everyAngle)),0));

        for (int i = 1; i < m_VetexList.Count; i++)
        {
            var angle = Deg2Rad(everyAngle * (i - 1) + (90 - everyAngle));
            float fRadius = RADIUS * listProgress[i - 1];
            SetUIVertex(i, new Vector3(fRadius * Mathf.Cos(angle), fRadius * Mathf.Sin(angle), 0));
        }

        int idx = 0;
        int value = 0;
        for (int i = 0; i < triangles_count * 3; i++)
        {
            if (i % 3 == 0)
            {
                indices.Add(0);
                value = idx + 3;
                idx++;
            }
            else
            {
                value--;

                indices.Add(value);
            }
        }

        indices[indices.Count - 2] = 1;
    }

    void SetUIVertex(int nIndex, Vector3 pos)
    {
        var vert = m_VetexList[nIndex];
        vert.position = pos * scale;
        vert.color = mainColor;
        m_VetexList[nIndex] = vert;
    }

    float Deg2Rad(float degree)
    {
        return degree * Mathf.Deg2Rad;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.Refresh();
    }
#endif

    private void Refresh()
    {
        base.graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(m_VetexList);

        SetVertices();

        vh.Clear();
        vh.AddUIVertexStream(m_VetexList, indices);
    }
}