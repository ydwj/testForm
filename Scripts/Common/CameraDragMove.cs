using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragMove : MonoBehaviour
{
    public enum MoveMode
    {
        cam, 
        obj,
    }
 
    //上次鼠标位置
    Vector2 prevMousePos = Vector3.zero;
    //滑动结束时的瞬时速度
    Vector3 Speed = Vector3.zero;
    //每帧偏差
    Vector3 offSet = Vector3.zero;
    //鼠标开始位置
    Vector3 startMousePosition = Vector3.zero;
    
    //速度衰減率
    public float decelerationRate = 0.1f; 
    //摄像机
    public Camera m_camera;
    //移动模式
    public MoveMode m_moveMode = MoveMode.obj;
 
    void Update()
    {
        HandleMouseInput();
    }
 
    private void HandleMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        //按下时记录位置
        if (Input.GetMouseButtonDown(0))
        {
            prevMousePos = Input.mousePosition;
            startMousePosition = Input.mousePosition;
        }
        //移动时更新位置
        if (Input.GetMouseButton(0))
        {
            Vector3 curMousePosition = Input.mousePosition;   //当前鼠标的屏幕坐标系
            //偏差值
            offSet = m_camera.ScreenToWorldPoint(curMousePosition) - m_camera.ScreenToWorldPoint(prevMousePos);
            prevMousePos = curMousePosition;
            //瞬时速度
            Speed = offSet / Time.deltaTime * 0.7f;
        }
        else   //最后递减
        {
            Speed *= Mathf.Pow(decelerationRate, Time.deltaTime);
            if (Mathf.Abs(Vector3.Magnitude(Speed)) < 1)
            {
                Speed = Vector3.zero;
            }
        }
 
        Move(Speed);
    }
 
    public void Move(Vector3 speed)
    {
        if (Vector3.Magnitude(Speed) == 0)
        {
            return;
        }
 
        //Debuger.Log("Current Speed" + Vector3.Magnitude(speed));
        if (m_moveMode == MoveMode.obj)
        {
            transform.localPosition += speed * Time.deltaTime;
        }
        else
        {
            m_camera.transform.localPosition -= speed * Time.deltaTime;
        }
    }
}