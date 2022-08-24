using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimUtility
{
    /// <summary>
    /// Lerps between two floats over the specified time span, invoking a handler every step
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="time"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static IEnumerator LerpOverTime(float from, float to, float time, System.Action<float> handler)
    {

        float timer = time;
        handler.Invoke(from);

        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            var val = Mathf.Lerp(to, from, timer / time);
            handler.Invoke(val);

            yield return new WaitForEndOfFrame();
        }
        handler.Invoke(to);
    }
}
