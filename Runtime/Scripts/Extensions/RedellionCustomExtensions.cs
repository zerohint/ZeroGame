using System;

/// <summary>
/// Small extensions
/// </summary>
public static class RedellionCustomExtensions
{
    /// <summary>
    /// Remap number from range to a range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    /// <param name="paddingChar"></param>
    /// <returns></returns>
    public static string PadBoth(this string str, int length, char paddingChar = ' ')
    {
        int spaces = length - str.Length;
        int padLeft = spaces / 2 + str.Length;
        return str.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
    }

    /// <summary>
    /// Get string until a specific character
    /// </summary>
    /// <param name="text"></param>
    /// <param name="stopAt"></param>
    /// <returns></returns>
    public static string GetUntilOrEmpty(this string text, string stopAt)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return string.Empty;
    }

    public static bool IsNullOrEmpty(this string value)
    {
        return value == null || value.Length == 0;
    }

    /// <summary>
    /// If not null or fake null. It exists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool IsExists<T>(this T item) => item != null && !item.Equals(null);
}