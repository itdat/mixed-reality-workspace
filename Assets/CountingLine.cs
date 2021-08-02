using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountingLine : MonoBehaviour
{
    [SerializeField] private int numberToAppear;
    [SerializeField] private GameObject appearTarget;

    private void OnEnable()
    {
        appearTarget.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (appearTarget.activeSelf) return;
        var strokes = GameObject.FindGameObjectsWithTag("BrushStroke");
        if (strokes == null) return;

        var count = 0;
        foreach (var stroke in strokes)
        {
            var line = stroke.GetComponent<LineRenderer>();
            if (line == null) continue;
            count += line.positionCount;
        }

        Debug.Log(count);
        
        if (count >= numberToAppear)
        {
            appearTarget.SetActive(true);
            foreach (var stroke in strokes)
            {
                Destroy(stroke);
            }
        }
    }
}
