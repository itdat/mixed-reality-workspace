// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Brush : MonoBehaviour
{
    protected InputData inputData;
    private Transform tip;
    private Renderer brushRenderer;
    
    private void OnEnable()
    {
        var brushTip = GameObject.Find("BrushTip");
        if (brushTip == null) return;
        tip = brushTip.transform;
        brushRenderer = brushTip.GetComponent<Renderer>();
    }

    public bool Draw
    {
        set
        {
            if (mDraw != value)
            {
                mDraw = value;
                if (mDraw)
                {
                    currentStrokeColor = brushRenderer.material.color;
                    StartCoroutine(DrawOverTime());
                }
            }
        }
    }
    
    [SerializeField] private float minPositionDelta = 0.01f;
    [SerializeField] private float maxTimeDelta = 0.25f;

    [SerializeField] private GameObject strokePrefab;

    public GameObject StrokePrefab
    {
        set => strokePrefab = value;
    }

    protected bool mDraw;
    public float width;

    private Color currentStrokeColor = Color.white;
    private float lastPointAddedTime = 0f;

    protected virtual IEnumerator DrawOverTime()
    {
        // Get the position of the tip
        Vector3 lastPointPosition = tip.position;
        // Then wait one frame and get the position again
        yield return null;

        // If we're not still drawing after one frame
        if (!mDraw)
        {
            // This was a fluke, abort!
            yield break;
        }

        Vector3 startPosition = tip.position;
        // Create a new brush stroke
        GameObject newStroke = Instantiate(strokePrefab);
        LineRenderer line = newStroke.GetComponent<LineRenderer>();
        newStroke.transform.position = startPosition;
        line.SetPosition(0, tip.position);
        float initialWidth = line.widthMultiplier;
        while (mDraw)
        {
            // Move the last point to the draw point position
            line.SetPosition(line.positionCount - 1, tip.position);
            line.material.color = currentStrokeColor;
            lastPointAddedTime = Time.unscaledTime;

            // Adjust the width between 0.1x and 0.5x width based on strength of trigger pull
            line.widthMultiplier = Mathf.Lerp(initialWidth * 0.2f, initialWidth, width);

            if (Vector3.Distance(lastPointPosition, tip.position) > minPositionDelta ||
                Time.unscaledTime > lastPointAddedTime + maxTimeDelta)
            {
                // Spawn a new point
                lastPointAddedTime = Time.unscaledTime;
                lastPointPosition = tip.position;
                var positionCount = line.positionCount;
                positionCount += 1;
                line.positionCount = positionCount;
                line.SetPosition(positionCount - 1, lastPointPosition);
            }

            yield return null;
        }
    }

    public void UpdateWidth(SliderEventData eventData)
    {
        width = eventData.NewValue;
    }

    public Vector3 TipPosition => tip.position;
}