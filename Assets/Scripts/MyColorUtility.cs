using UnityEngine;

public static class MyColorUtility
{
    public static string ToHtmlStringRGB(this Color color)
    {
        return UnityEngine.ColorUtility.ToHtmlStringRGB(color);
    }
}