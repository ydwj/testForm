using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    // Start is called before the first frame update
    public bool drag_Top, drag_Down;
    public float offset;
    void Start()
    {
        Rect safeArea = Screen.safeArea;
        float height = Screen.height - safeArea.height; //  获取刘海高度
#if UNITY_EDITOR
        //Debuger.Log("====== " + height);
#endif
        if (height > 0 && !(drag_Top || drag_Down))    //朝下位移
        {
            float h = height / 2 + offset;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            Vector2 pos = rectTransform.anchoredPosition;
            pos = new Vector2(pos.x,pos.y - (h));
            rectTransform.anchoredPosition = pos;
        }
        else if (height > 0 && (drag_Top || drag_Down))    //朝下拉伸
        {
            float h = height / 2 + offset;
            RectTransform rectTransform = this.GetComponent<RectTransform>();

            if (drag_Top)
            {
                rectTransform.offsetMax -= new Vector2(0, h);
            }

            if (drag_Down)
            {
                rectTransform.offsetMin -= new Vector2(0, h);
            }
            // Vector2 size = rectTransform.sizeDelta;
            // size.y = h;
            // rectTransform.sizeDelta = size;
        }
    }
}