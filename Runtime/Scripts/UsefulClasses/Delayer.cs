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
}
