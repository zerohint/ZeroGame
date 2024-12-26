using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class StringExtensions
{
    public static string Colorize(this string text, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color={colorHex}>{text}</color>";
    }

    public static string Colorize(this string text, string color) => $"<color={color}>{text}</color>";

    public static string Colorize(this string text, int colorHash)
    {
        Color color = ColorUtility.IntToColor(colorHash);
        return text.Colorize(color);
    }

    public static string ColorifyClass<T>(T instance, int propColor = 0xFFBB5C, int valColor = 0xA6FF96)
    {
        Type myClassType = typeof(T);
        FieldInfo[] fields = myClassType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        string[] props = new string[fields.Count()];
        int counter = 0;
        foreach (FieldInfo field in fields)
        {
            var val = field.GetValue(instance);
            props[counter++] = $"{field.Name.Colorize(propColor)} {(val != null ? val.ToString().Colorize(valColor) : "null".Colorize(Color.red))}";
        }
        return $"{myClassType.Name.ToUpper().Colorize(Color.cyan)} - " + string.Join(" | ".Colorize(Color.black), props);
    }
}

internal static class ColorUtility
{
    public static string ToHtmlStringRGB(Color color) => ToHtmlStringRGB((Color32)color);

    public static string ToHtmlStringRGB(Color32 color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
    }

    public static Color IntToColor(int colorHash)
    {
        byte r = (byte)((colorHash >> 16) & 0xFF);
        byte g = (byte)((colorHash >> 8) & 0xFF);
        byte b = (byte)(colorHash & 0xFF);

        return new Color32(r, g, b, 255);
    }
}
