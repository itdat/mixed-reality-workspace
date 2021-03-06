using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : Brush {
    [SerializeField]
    private float eraseRange = 0.1f;

    [SerializeField]
    private float eraseTime = 0.15f;

    private bool erasingStrokes = false;
    private Queue<LineRenderer> erasedStrokes = new Queue<LineRenderer>();

    private Stack<GameObject> undo = new Stack<GameObject>();
    private Stack<GameObject> redo = new Stack<GameObject>();

    // Instead of drawing, the eraser will remove existing strokes
    protected override IEnumerator DrawOverTime() {
        //if (inputData.type != InputData.Type.ERASER) yield return null;
        // Get all the brush strokes that currently exist
        var brushStrokes = new List<GameObject>(GameObject.FindGameObjectsWithTag("BrushStroke"));

        while (mDraw) {
            // Move backwards through the brush strokes, removing any we intersect with
            for (int i = brushStrokes.Count - 1; i >= 0; i--) {
                // Do a crude check for proximity with the brush stroke's render bounds
                var lineRenderer = brushStrokes[i].GetComponent<LineRenderer>();
                if (erasedStrokes.Contains(lineRenderer))
                    continue;

                if (lineRenderer.bounds.Contains(TipPosition)) {
                    // If we're in bounds, check whether any point of the stroke is within range
                    var positions = new Vector3[lineRenderer.positionCount];
                    lineRenderer.GetPositions(positions);
                    for (var j = 0; j < positions.Length; j++) {
                        if (!(Vector3.Distance(positions[j], TipPosition) < eraseRange)) continue;
                        // Un-tag and erase the brush stroke
                        brushStrokes[i].tag = "Untagged";
                        brushStrokes.RemoveAt(i);
                        erasedStrokes.Enqueue(lineRenderer);
                        if (!erasingStrokes) {
                            // If we've just added a new stroke, start erasing them
                            erasingStrokes = true;
                            StartCoroutine(EraseStrokesOverTime());
                        }

                        break;
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator EraseStrokesOverTime() {
        Debug.Log("erasedStrokes.Count: " + erasedStrokes.Count);
        while (erasedStrokes.Count > 0) {
            var lineRenderer = erasedStrokes.Dequeue();
            var startTime = Time.unscaledTime;
            var startWidth = lineRenderer.widthMultiplier;
            // Get the positions from the line renderer
            var startPositions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(startPositions);
            // Create a new array for their positions as they're being erased
            var endPositions = new Vector3[lineRenderer.positionCount];
            while (Time.unscaledTime < startTime + eraseTime) {
                var normalizedTime = (Time.unscaledTime - startTime) / eraseTime;
                for (int i = 0; i < startPositions.Length; i++) {
                    // Add a bit of random noise based on how far away the point is
                    Vector3 randomNoise = Random.insideUnitSphere *
                                          (Vector3.Distance(endPositions[i], TipPosition) * 0.1f);
                    endPositions[i] = Vector3.Lerp(startPositions[i], TipPosition, normalizedTime) + randomNoise;
                    lineRenderer.SetPositions(endPositions);
                    // Make the line skinnier as we go
                    lineRenderer.widthMultiplier = Mathf.Lerp(startWidth, startWidth * 0.1f, normalizedTime);
                }

                yield return null;
            }

            Destroy(lineRenderer.gameObject);
            yield return null;
        }

        erasingStrokes = false;
    }
}
