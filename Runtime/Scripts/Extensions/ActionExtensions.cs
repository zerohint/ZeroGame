using System;

public static class ActionExtensions
{
	public static void InvokeSafe(this Action action)
	{
		try
		{
			if (action != null) action();
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("InvokeSafe: " + e.Message);
		}
	}

    public static void InvokeSafe<T>(this Action<T> action, T t)
    {
        try
        {
            if (action != null) action(t);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("InvokeSafe: " + e.Message);
        }
    }

    public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
    {
        try
        {
            if (action != null) action(t1, t2);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("InvokeSafe: " + e.Message);
        }
    }
}