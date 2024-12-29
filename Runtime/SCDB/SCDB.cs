using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroGame
{
    public static class SCDB
    {
        public const string TITLE = "SCDB";

        /// <summary>
        /// Get the first one matches
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T Get<T>(Func<T, bool> selector) where T : ScriptableObject
        {
            foreach(var sc in SCDBSC.Instance.scriptables)
                if (sc is T t && selector(t))
                    return t;
            return null;
        }

        public static IEnumerable<T> GetAll<T>(Func<T, bool> selector = null) where T : ScriptableObject
        {
            foreach (var sc in SCDBSC.Instance.scriptables)
                if (sc is T t && (selector == null || selector(t)))
                    yield return t;
        }
    }
}

