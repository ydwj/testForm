using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using TooSimpleFramework.UI;
using Object = UnityEngine.Object;

public static class ToolsMgr
{

    public static IDisposable Timer(float fSecond, Action OnComplete)
    {
        var timer = Observable.Timer(TimeSpan.FromSeconds(fSecond)).Subscribe(_ => { OnComplete?.Invoke(); });

        return timer;
    }

    public static void ShowTips(string strTips)
    {
        FormMgr.Ins.ShowTipsMsg(strTips);
    }



    public static void ShowTipsByKey(string strLocalizationKey)
    {
        FormMgr.Ins.ShowTipsMsg(Localization.Get(strLocalizationKey));
    }
    

    public static void SetZToZero(Transform transTarget)
    {
        transTarget.localPosition = new Vector3(transTarget.localPosition.x, transTarget.localPosition.y, 0);
    }



    public static TimeSpan getDeltaTime(DateTime start, DateTime end)
    {
        if (start.Ticks >= end.Ticks)
            return new TimeSpan(0);

        var startSpan = new TimeSpan(start.Ticks);
        var endSpan = new TimeSpan(end.Ticks);
        return endSpan.Subtract(startSpan);
    }

    public static string GetDeltaTimeStrFromNow(DateTime dtEnd)
    {
        return getDeltaTimeStr(TimeHandler.Now, dtEnd);
    }

    public static string getDeltaTimeStr(DateTime start, DateTime end)
    {
        if (start.Ticks >= end.Ticks)
            return "00:00";

        TimeSpan ts = getDeltaTime(start, end);
        return getTimeStrOfTimeSpan(ts);
    }

    private static string strTimeFormat1 = "{0:D2}:{1:D2}";
    private static string strTimeFormat2 = "{0:D2}:{1:D2}:{2:D2}";
    private static string strTimeFormat3 = Localization.Get("str_timestr_overday");

    public static string getTimeStrOfTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds <= 0)
            return "00:00";
        
        if (ts.TotalDays < 1)
        {
            if (ts.TotalHours < 1)
                return string.Format(strTimeFormat1, (Int32)ts.Minutes, (Int32)ts.Seconds);
            return string.Format(strTimeFormat2, (Int32)ts.TotalHours, (Int32)ts.Minutes, (Int32)ts.Seconds);
        }
        else
        {
            return string.Format(strTimeFormat3, (Int32)ts.Days, (Int32)ts.Hours,
                (Int32)ts.Minutes, (Int32)ts.Seconds);
        }
    }



    public static int GetTargetIndexByWeights(int[] listWeights)
    {
        int nIndex = 0;

        int totalWeight = listWeights.Sum();
        int randomNum = Random.Range(0, totalWeight);

        totalWeight = 0;
        for (int k = 0; k < listWeights.Length; k++)
        {
            totalWeight += listWeights[k];
            if (randomNum <= totalWeight)
            {
                nIndex = k;
                break;
            }
        }

        return nIndex;
    }

    /// <summary>
    /// 获得金币特效
    /// </summary>
    public static void ShowGetGoldFlyEff(double dbCount = 0)
    {
        MsgMgr.Ins.Publish(GameMsg.Main_ShowGetGoldFlyEff, dbCount);
    }

    /// <summary>
    /// 获得钻石特效
    /// </summary>
    public static void ShowGetDiamondFlyEff(double dbCount = 0)
    {
        MsgMgr.Ins.Publish(GameMsg.Main_ShowGetDiamondFlyEff, dbCount);
    }

    /// <summary>
    /// double比较大小
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool DoubleEquals(double dbVal1, double dbVal2)
    {
        //双精度误差
        var DOUBLE_DELTA = 1E-6;
        return dbVal1 == dbVal2 || Math.Abs(dbVal1 - dbVal2) < DOUBLE_DELTA;
    }

    /// <summary>
    /// float比较大小
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool FloatEquals(float val1, float val2)
    {
        //误差
        float DELTA = 0.000001f;
        return val1 == val2 || Math.Abs(val1 - val2) < DELTA;
    }

    /// <summary>
    /// DateTime是否是同一天
    /// </summary>
    /// <returns></returns>
    public static bool DayEquals(DateTime dtVal1, DateTime dtVal2)
    {
        return dtVal1.Year == dtVal2.Year && dtVal1.DayOfYear == dtVal2.DayOfYear;
    }

    public static string GetPercentStrFromFloat(float fRate)
    {
        return string.Format(ConstHandler.e, fRate * 100);
    }
    
    /// <summary>
    /// 30 => return 30%
    /// </summary>
    /// <param name="nRate"></param>
    /// <returns></returns>
    public static string GetPercentStrFromInt(int nRate)
    {
        return $"{nRate}%";
    }

    /// <summary>
    /// 随机间隔计时器
    /// </summary>
    /// <param name="fSecondMin"></param>
    /// <param name="fSecondMax"></param>
    /// <param name="OnInterval"></param>
    public static void RandomInterval(float fSecondMin, float fSecondMax, Action OnInterval, Component componentAddTo = default)
    {
        Timer(Random.Range(fSecondMin, fSecondMax), () =>
        {
            OnInterval?.Invoke();
            RandomInterval(fSecondMin, fSecondMax, OnInterval, componentAddTo);
        }).AddTo(componentAddTo);
    }



    public static Color GetHDRColor(Color clr, float fIntensity)
    {
        float factor = Mathf.Pow(2, fIntensity);
        Color clrHDR = new Color(clr.r * factor, clr.g * factor, clr.b * factor);
        return clrHDR;
    }

    public static RenderTexture CreateTmpRenderTex(int nWidth, int nHeight)
    {
        return RenderTexture.GetTemporary(nWidth, nHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8);
    }

    public static void GetBoundsMinMaxPtInScreen(Camera sceneCamera, Bounds bounds, out Vector2 minScreenPt, out Vector2 maxScreenPt)
    {
        minScreenPt = Vector2.zero;
        maxScreenPt = Vector2.zero;

        var targetBoundsExtents = bounds.extents;
        float x = targetBoundsExtents.x;
        float y = targetBoundsExtents.y;
        float z = targetBoundsExtents.z;

        Vector3[] arrPt = new Vector3[8];
        arrPt[0] = bounds.center + new Vector3(x, y, z);
        arrPt[1] = bounds.center + new Vector3(x, -y, z);
        arrPt[2] = bounds.center + new Vector3(x, y, -z);
        arrPt[3] = bounds.center + new Vector3(x, -y, -z);
        arrPt[4] = bounds.center + new Vector3(-x, y, z);
        arrPt[5] = bounds.center + new Vector3(-x, -y, z);
        arrPt[6] = bounds.center + new Vector3(-x, y, -z);
        arrPt[7] = bounds.center + new Vector3(-x, -y, -z);

        foreach (var t in arrPt)
        {
            Vector2 screenPt = sceneCamera.WorldToScreenPoint(t);
            if (maxScreenPt == default || screenPt.x >= maxScreenPt.x && screenPt.y >= maxScreenPt.y)
                maxScreenPt = screenPt;
            if (minScreenPt == default || screenPt.x <= minScreenPt.x && screenPt.y <= minScreenPt.y)
                minScreenPt = screenPt;
        }
    }

    public static GameObject ShowEffPoolObject(string strResName, Transform transParent, Vector3 vecPos, float fTimerRecycle, float fScale = -1, ResouceType resouceType = ResouceType.Effect)
    {
        var goEff = ResMgr.Ins.GetInstantiateFromPool(strResName, transParent, resouceType);
        if (!goEff)
        {
            Debuger.Log($"特效{strResName}不存在！");
            return default;
        }

        Transform transEff = goEff.transform;
        if (vecPos == default)
            transEff.localPosition = Vector3.zero;
        else
            transEff.position = vecPos;
        
        transEff.localRotation = Quaternion.identity;

        if (fScale > 0)
        {
            transEff.localScale = new Vector3(fScale, fScale, fScale);
        }

        if (fTimerRecycle >= 0)
        {
            Timer(fTimerRecycle, () =>
            {
                ResMgr.Ins.RecycleObj(goEff);
            }).AddTo(goEff);
        }

        return goEff;
    }

    public static Color ParseHtmlColor(string colorStr)
    {
        ColorUtility.TryParseHtmlString(colorStr, out Color color);
        return color;
    }

    public static bool IsPointInRect(Vector2[] rectVertexs, Vector2 t)
    {
        if (rectVertexs.Length != 4)
        {
            return false;
        }

        Vector2 vec1 = rectVertexs[1] - rectVertexs[0];
        Vector2 vec2 = rectVertexs[2] - rectVertexs[1];
        Vector2 vec3 = rectVertexs[3] - rectVertexs[2];
        Vector2 vec4 = rectVertexs[0] - rectVertexs[3];

        Vector2 vec_t1 = t - rectVertexs[0];
        Vector2 vec_t2 = t - rectVertexs[1];
        Vector2 vec_t3 = t - rectVertexs[2];
        Vector2 vec_t4 = t - rectVertexs[3];

        var c1 = Vector3.Cross(vec1, vec_t1);
        var c2 = Vector3.Cross(vec2, vec_t2);
        var c3 = Vector3.Cross(vec3, vec_t3);
        var c4 = Vector3.Cross(vec4, vec_t4);

        if (Vector3.Dot(c1, c2) < 0 || Vector3.Dot(c1, c3) < 0 || Vector3.Dot(c1, c4) < 0)
        {
            return false;
        }

        return true;
    }

    public static List<T> Shuffle<T>(List<T> original)
    {
        System.Random randomNum = new System.Random();
        int index = 0;
        T temp;
        for (int i = 0; i < original.Count; i++)
        {
            index = randomNum.Next(0, original.Count - 1);
            if (index != i)
            {
                temp = original[i];
                original[i] = original[index];
                original[index] = temp;
            }
        }
        return original;
    }

    public static List<T> Shuffle2<T>(List<T> original)
    {
        List<T> listNew = new List<T>();

        foreach (var item in original)
        {
            listNew.Insert(Random.Range(0, listNew.Count), item);
        }

        return listNew;
    }

    public static bool IsTouchedOnUI()
    {
#if UNITY_EDITOR
        if (!Input.GetMouseButton(0))
            return false;
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
#else
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return true;
#endif
        return false;
    }


    /// <summary>
    /// 获取区域随机点
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPosInRect(Bounds bounds)
    {
        float fHalfWidth = bounds.size.x * 0.5f;
        float fHalfHeight = bounds.size.z * 0.5f;

        var pos = bounds.center;
        pos += new Vector3(Random.Range(-fHalfWidth, fHalfWidth), 0, Random.Range(-fHalfHeight, fHalfHeight));

        return pos;
    }
    
    public static Vector3 GetTargetPosByLocalOffset(Transform baseTransform, Vector3 localOffSet)
    {
        return baseTransform.position + (baseTransform.right * localOffSet.x +
                                             baseTransform.up * localOffSet.y +
                                             baseTransform.forward * localOffSet.z);
    }
    //==================Common End



    //================== Game Start
    public static string GetStrFromNum(int nNum)
    {
        string strNum = default;
        switch (nNum)
        {
            case 0:
                strNum = "零";
                break;
            case 1:
                strNum = "一";
                break;
            case 2:
                strNum = "二";
                break;
            case 3:
                strNum = "三";
                break;
            case 4:
                strNum = "四";
                break;
            case 5:
                strNum = "五";
                break;
            default:
                strNum = "";
                break;
        }

        return strNum;
    }

    public static long GenerateId()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0);
    }


    #region WjTool

    //将按钮置灰
    public  static void FalseBtn(Button button)
    {
        button.enabled = false;
        button.GetComponent<Image>().color = Color.grey;
        Text text = button.transform.GetChild(0).GetComponent<Text>();
        if (text != null)
            text.color = new Color32(255, 255, 255, 140);
    }


    public static void InvokeBtn(Button button)
    {
        button.enabled = true;
        button.GetComponent<Image>().color = Color.white;
        Text text = button.transform.GetChild(0).GetComponent<Text>();
        if (text != null)
            text.color = Color.white;

    }



    /// <summary>
    /// double和float 相乘返回Double
    /// </summary>
    /// <param name="d"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static double DxF(double d, float f)
    {
        decimal temp = Convert.ToDecimal(d) * Convert.ToDecimal(f);
        return Convert.ToDouble(temp);
    }

    /// <summary>
    /// 获取文字播放的时间
    /// </summary>
    /// <param name="text"></param>
    /// <param name="tSpeed"></param>
    /// <returns></returns>
    public static float GetTextTime(string textValue, float tSpeed )
    {
        float a = 1 / tSpeed;
        string temp = System.Text.RegularExpressions.Regex.Replace(textValue, @"<[^>]*>", "");

        return temp.Length * a;
    }
     
    #endregion



    public static Sprite GetQualitySprite(eQualityType qualityType)
    {
        string strResName = ConstHandler.ResName_QualityBg_0;
        switch (qualityType)
        {
            case eQualityType.Gray:
                strResName = ConstHandler.ResName_QualityBg_0;
                break;
            case eQualityType.Green:
                strResName = ConstHandler.ResName_QualityBg_1;
                break;
            case eQualityType.Blue:
                strResName = ConstHandler.ResName_QualityBg_2;
                break;
            case eQualityType.Purple:
                strResName = ConstHandler.ResName_QualityBg_3;
                break;
            case eQualityType.Orange:
                strResName = ConstHandler.ResName_QualityBg_4;  
                break;
            case eQualityType.Red:
                strResName = ConstHandler.ResName_QualityBg_5;
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }
    
    public static Sprite GetQualitySprite_Hero1(eQualityType qualityType)
    {
        string strResName = ConstHandler.ResName_QualityBg_Hero1_0;
        switch (qualityType)
        {
            case eQualityType.Gray:
                strResName = ConstHandler.ResName_QualityBg_Hero1_0;
                break;
            case eQualityType.Green:
                strResName = ConstHandler.ResName_QualityBg_Hero1_1;
                break;
            case eQualityType.Blue:
                strResName = ConstHandler.ResName_QualityBg_Hero1_2;
                break;
            case eQualityType.Purple:
                strResName = ConstHandler.ResName_QualityBg_Hero1_3;
                break;
            case eQualityType.Orange:
                strResName = ConstHandler.ResName_QualityBg_Hero1_4;  
                break;
            case eQualityType.Red:
                strResName = ConstHandler.ResName_QualityBg_Hero1_5;
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }
    
    public static Sprite GetQualitySprite_Hero2(eQualityType qualityType)
    {
        string strResName = ConstHandler.ResName_QualityBg_Hero2_0;
        switch (qualityType)
        {
            case eQualityType.Gray:
                strResName = ConstHandler.ResName_QualityBg_Hero2_0;
                break;
            case eQualityType.Green:
                strResName = ConstHandler.ResName_QualityBg_Hero2_1;
                break;
            case eQualityType.Blue:
                strResName = ConstHandler.ResName_QualityBg_Hero2_2;
                break;
            case eQualityType.Purple:
                strResName = ConstHandler.ResName_QualityBg_Hero2_3;
                break;
            case eQualityType.Orange:
                strResName = ConstHandler.ResName_QualityBg_Hero2_4;  
                break;
            case eQualityType.Red:
                strResName = ConstHandler.ResName_QualityBg_Hero2_5;
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }
    
    public static Sprite GetQualitySprite_Skill(eQualityType qualityType)
    {
        string strResName = String.Empty;
        switch (qualityType)
        {
            case eQualityType.Green:
                strResName = "quality_bg_skill_1";
                break;
            case eQualityType.Blue:
                strResName = "quality_bg_skill_2";
                break;
            case eQualityType.Purple:
                strResName = "quality_bg_skill_3";
                break;
            case eQualityType.Orange:
                strResName = "quality_bg_skill_4";  
                break;
            case eQualityType.Red:
                strResName = "quality_bg_skill_5";
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }

    public static Sprite GetQualityPet(eQualityType qualityType)
    {
        string strResName = String.Empty;
        switch (qualityType)
        {
            case eQualityType.Gray:
                strResName = "quality_Bg_Pet_0";
                break;
            case eQualityType.Green:
                strResName = "quality_Bg_Pet_1";
                break;
            case eQualityType.Blue:
                strResName = "quality_Bg_Pet_2";
                break;
            case eQualityType.Purple:
                strResName = "quality_Bg_Pet_3";
                break;
            case eQualityType.Orange:
                strResName = "quality_Bg_Pet_4";
                break;
            case eQualityType.Red:
                strResName = "quality_Bg_Pet_5";
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }

    public static Sprite GetQualitySoldierTrain(eQualityType qualityType)
    {
        string strResName = String.Empty;
        switch (qualityType)
        {
            case eQualityType.Gray:
                strResName = "yingxiong_37";
                break;
            case eQualityType.Green:
                strResName = "yingxiong_39";
                break;
            case eQualityType.Blue:
                strResName = "yingxiong_35";
                break;
            case eQualityType.Purple:
                strResName = "yingxiong_33";
                break;
            case eQualityType.Orange:
                strResName = "yingxiong_41";
                break;
            case eQualityType.Red:
                strResName = "yingxiong_69";
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }


    public static Sprite GetQualityOrder(eQualityType qualityType)
    {
        string strResName = String.Empty;
        switch (qualityType)
        {
            case eQualityType.Green:
                strResName = "dingdan_16";
                break;
            case eQualityType.Blue:
                strResName = "dingdan_04";
                break;
            case eQualityType.Purple:
                strResName = "dingdan_03";
                break;
            case eQualityType.Orange:
                strResName = "dingdan_05";
                break;
        }

        Sprite sprite = ResMgr.Ins.GetSpriteResource(strResName, ResouceType.UI);
        return sprite;
    }

    public static string GetQualityName(eQualityType qualityType)
    {
        string strQuality = Localization.Get("str_Quality1");

        switch (qualityType)
        {
            case eQualityType.Gray:
                strQuality = Localization.Get("str_Quality1");
                break;
            case eQualityType.Green:
                strQuality = Localization.Get("str_Quality2");
                break;
            case eQualityType.Blue:
                strQuality = Localization.Get("str_Quality3");
                break;
            case eQualityType.Purple:
                strQuality = Localization.Get("str_Quality4");
                break;
            case eQualityType.Orange:
                strQuality = Localization.Get("str_Quality5");
                break;
            case eQualityType.Red:
                strQuality = Localization.Get("str_Quality6");
                break;
        }

        return strQuality;
    }

    

    /// <summary>
    /// 道具类型名称
    /// </summary>
    /// <returns></returns>
    public static string GetItemTypeString(eItemType itemType)
    {
        return Localization.Get($"str_itemtype_{(int)itemType}");
    }
    



    public static string GetFoodTypeString(eFoodType foodType)
    {
        switch (foodType)
        {
            case eFoodType.Meat:
                return Localization.Get("str_foodtype_01");
            case eFoodType.Fruit:
                return Localization.Get("str_foodtype_02");
            case eFoodType.Seafood:
                return Localization.Get("str_foodtype_03");
        }

        return default;
    }

    public static string GetItemPropertyTypeString(eAttributeType attributeType)
    {
        string strAttributeKey = "";
        switch (attributeType)
        {
            case eAttributeType.Attack:
                strAttributeKey = "name_attribute_4";
                break;
            case eAttributeType.Defence:
                strAttributeKey = "name_attribute_5";
                break;
            case eAttributeType.Hp:
                strAttributeKey = "name_attribute_6";
                break;
        }
        return Localization.Get(strAttributeKey);
    }



  
    /// <summary>
    /// 保留小数点后N位
    /// </summary>
    /// <returns></returns>
    public static float ToDecimalPlaces(float f, int nPlacesCount)
    {
        int nTmp = (int) Mathf.Pow(10f, nPlacesCount);
        int i =(int)(f * nTmp);
        float fFinal = i * 1.0f / nTmp;
        return fFinal;
    }


    /// <summary>
    /// 求vp在法线为vn的平面上的投影
    /// </summary>
    /// <param name="vp"></param>
    /// <param name="vn"></param>
    /// <returns></returns>
    public static Vector3 ProjectOnPlane(Vector3 vp,Vector3 vn)
    {
        Vector3 vt = new Vector3();

        vt = vp - vn * Vector3.Dot(vp, vn) / Vector3.Dot(vn, vn);

        return vt;
    }




    /// <summary>
    /// 文字专用，输入一个速度，返回DoText所需要的时间
    /// </summary>
    public static float GetPlayTextTime(string text ,float playTextSpeed)
    {
        float a = 1 / playTextSpeed;
        string temp = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]*>", "");

        return temp.Length * a;
    }

    
}