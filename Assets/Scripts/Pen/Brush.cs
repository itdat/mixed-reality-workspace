using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Brush : MonoBehaviour {
    private Renderer renderer;

    private void OnEnable() {
        renderer = GetComponent<MeshRenderer>();
    }


    public bool Draw {
        set {
            if (mDraw == value) return;
            mDraw = value;
            if (!mDraw) return;
            if (!isActiveAndEnabled) return;
            currentStrokeColor = renderer.material.color;
            StartCoroutine(DrawOverTime());
        }
    }

    [SerializeField]
    private float minPositionDelta = 0.01f;

    [SerializeField]
    private float maxTimeDelta = 0.25f;

    [SerializeField]
    private GameObject strokePrefab;

    public GameObject StrokePrefab {
        set => strokePrefab = value;
    }

    protected bool mDraw;
    public float width;

    private Color currentStrokeColor = Color.white;
    private float lastPointAddedTime = 0f;

    protected virtual IEnumerator DrawOverTime() {
        // Get the position of the tip
        var lastPointPosition = transform.position;
        // Then wait one frame and get the position again
        yield return null;

        // If we're not still drawing after one frame
        if (!mDraw) {
            // This was a fluke, abort!
            yield break;
        }

        var position = transform.position;
        // Create a new brush stroke
        var newStroke = Instantiate(strokePrefab);
        var line = newStroke.GetComponent<LineRenderer>();
        newStroke.transform.position = position;
        line.SetPosition(0, position);
        var initialWidth = line.widthMultiplier;
        while (mDraw) {
            // Move the last point to the draw point position
            line.SetPosition(line.positionCount - 1, transform.position);
            line.material.color = currentStrokeColor;
            lastPointAddedTime = Time.unscaledTime;

            // Adjust the width between 0.1x and 0.5x width based on strength of trigger pull
            line.widthMultiplier = Mathf.Lerp(initialWidth * 0.2f, initialWidth, width);

            if (Vector3.Distance(lastPointPosition, transform.position) > minPositionDelta ||
                Time.unscaledTime > lastPointAddedTime + maxTimeDelta) {
                // Spawn a new point
                lastPointAddedTime = Time.unscaledTime;
                lastPointPosition = transform.position;
                var positionCount = line.positionCount;
                positionCount += 1;
                line.positionCount = positionCount;
                line.SetPosition(positionCount - 1, lastPointPosition);
            }

            yield return null;
        }
    }

    public void UpdateWidth(SliderEventData eventData) {
        width = eventData.NewValue;
    }

    protected Vector3 TipPosition => transform.position;
}
