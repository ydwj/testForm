using UnityEngine;
using UnityEngine.UI;

public static class UIExtend
{
    static public void setGray(this Image image, bool isGray)
    {
        if (isGray && image.material.shader.name == "Sprites/Gray")
            return;
        
        Material mat;
        if (isGray)
        {
            mat = new Material(Shader.Find("Sprites/Gray"));
        }
        else
        {
            mat = null;
        }
        image.material = mat;
    }
    static public void setGray(this Text text, bool isGray)
    {
        if (isGray && text.material.shader.name == "Sprites/Gray")
            return;

        Material mat;
        if (isGray)
        {
            mat = new Material(Shader.Find("Sprites/Gray"));
        }
        else
        {
            //mat = new Material(Shader.Find("Sprites/Default"));
            mat = null;
        }
        text.material = mat;
    }
    public static Color ParseHtmlColor(this string colorStr)
    {
        ColorUtility.TryParseHtmlString(colorStr, out Color color);
        return color;
    }
}
