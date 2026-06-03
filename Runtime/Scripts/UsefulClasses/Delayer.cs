using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Start coroutine at TheSingleton and run actions by delay
/// </summary>
public class Delayer
{
    /// <summary>
    /// Delay by time
    /// </summary>
    /// <param name="action"></param>
    /// <param name="delay"></param>
    public static void Delay(Action action, float delay)
    {
        TheSingleton.Instance.StartCoroutine(DelayCR());
        IEnumerator DelayCR()
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }

    /// <summary>
    /// Delay by time
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    public static void Delay(float delay, Action action) => Delay(action, delay);


    /// <summary>
    /// Delay by frame
    /// </summary>
    /// <param name="action"></param>
    /// <param name="frame"></param>
    public static void DelayFrame(Action action, int frame = 1)
    {
        TheSingleton.Instance.StartCoroutine(DelayFrameCR());
        IEnumerator DelayFrameCR()
        {
            yield return (frame == 1) ? null : new WaitForFrames(frame);
            action.Invoke();
        }
    }

    /// <summary>
    /// Delay while condition true
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="action"></param>
    public static void DelayUntil(Func<bool> condition, Action action)
    {
        TheSingleton.Instance.StartCoroutine(DelayUntilCR());
        IEnumerator DelayUntilCR()
        {
            yield return new WaitUntil(condition);
            action.Invoke();
        }
    }


    /// <summary>
    /// Delay while condition is true
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="action"></param>
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
