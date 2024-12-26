public static class EnumExtensions
{
    /// <summary>
    /// Get random element of enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandom<T>() where T : System.Enum
    {
        T[] enumValues = (T[])System.Enum.GetValues(typeof(T));

        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
        return enumValues[randomIndex];
    }
}