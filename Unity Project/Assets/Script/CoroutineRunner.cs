using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static void RunCoroutine(IEnumerator coroutine)
    {
        var go = new GameObject("runner");
        DontDestroyOnLoad(go);

        var runner = go.AddComponent<CoroutineRunner>();

        runner.StartCoroutine(runner.MonitorRunning(coroutine));
    }

    IEnumerator MonitorRunning(IEnumerator coroutine)
    {
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }

        Destroy(gameObject);
    }
}
