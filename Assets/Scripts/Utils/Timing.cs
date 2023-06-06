using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timing : Singleton<Timing>
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void DelayedExecute(float delay, Action onExecute)
    {
        // Check if delay is valid.
        if (delay < 0) return;

        StartCoroutine(DelayRoutine(delay, onExecute));
    }

    private IEnumerator DelayRoutine(float delay, Action onExecute)
    {
        // Wait for given delay
        yield return new WaitForSeconds(delay);

        onExecute?.Invoke();
    }
}
