using System;
using System.Collections;
using UnityEngine;

public class Delayer
{
    public static void Delay(Action action, float delay)
    {
        TheSingleton.Instance.StartCoroutine(DelayCR());
        IEnumerator DelayCR()
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }

    public static void Delay(float delay, Action action) => Delay(action, delay);


    public static void DelayUntil(Func<bool> condition, Action action)
    {
        TheSingleton.Instance.StartCoroutine(DelayUntilCR());
        IEnumerator DelayUntilCR()
        {
            yield return new WaitUntil(condition);
            action.Invoke();
        }
    }

    public static void DelayWhile(Func<bool> condition, Action action)
    {
        TheSingleton.Instance.StartCoroutine(DelayWhileCR());
        IEnumerator DelayWhileCR()
        {
            yield return new WaitWhile(condition);
            action.Invoke();
        }
    }
}
