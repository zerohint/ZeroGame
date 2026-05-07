using System;
using System.Threading.Tasks;

public static class TaskUtils
{
    public static async Task WaitUntil(Func<bool> predicate, int checkPeriod = 100, int timeoutSeconds = -1)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        while (!predicate.Invoke())
        {
            if (timeoutSeconds > 0 && stopwatch.Elapsed.TotalSeconds >= timeoutSeconds)
                throw new TimeoutException($"WaitUntil timed out after {timeoutSeconds} seconds");
            await Task.Delay(checkPeriod);
        }
    }

    public static async Task WaitWhile(Func<bool> predicate, int checkPeriod = 100) => await WaitUntil(() => !predicate(), checkPeriod);

    public static async Task WaitForSeconds(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
    }
}
