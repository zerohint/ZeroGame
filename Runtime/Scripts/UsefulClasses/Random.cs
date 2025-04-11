using System;

namespace ZeroGame
{
    public static class Random
    {
        /// <summary>
        /// Returns a random float within [0.0..1.0] (range is inclusive) (Read Only).
        /// </summary>
        public static float Value => (float)new System.Random().NextDouble();

        /// <summary>
        /// Same as UnityEngine.Random.Range but could be called from constructor.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            System.Random random = new();
            return random.Next(min, max + 1);
        }

        /// <summary>
        /// Same as UnityEngine.Random.Range but could be called from constructor.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            System.Random random = new();
            return (float)(random.NextDouble() * (max - min)) + min;
        }

        /// <summary>
        /// Return random enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum RandomEnum<TEnum>() where TEnum : Enum
        {
            Array enumValues = Enum.GetValues(typeof(TEnum));
            int randomIndex = new System.Random().Next(enumValues.Length);
            return (TEnum)enumValues.GetValue(randomIndex);
        }
    }
}