using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class InputData : MonoBehaviour
{
    private Brush drawBrush;
    private Eraser eraser;

    public Type type = Type.DRAW;

    private void OnEnable()
    {
        drawBrush = GetComponent<Brush>();
        eraser = GetComponent<Eraser>();
    }

    void Update()
    {
        bool isDraw = false;
        foreach (var controller in CoreServices.InputSystem.DetectedControllers)
        {
            var sourceName = controller.InputSource.SourceName;
            if (sourceName != "Right Hand")
                continue;

            foreach (MixedRealityInteractionMapping inputMapping in controller.Interactions)
            {
                if (inputMapping.Description != "Grab") continue;
                if (inputMapping.BoolData)
                {
                    UpdateState(true);
                    isDraw = true;
                }
                else
                    UpdateState(false);

                break;
            }
        }

        if (!isDraw) drawBrush.Draw = false;
    }

    private void UpdateState(bool value)
    {
        if (type == Type.DRAW)
            drawBrush.Draw = value;
        else eraser.Draw = value;
    }

    public void ChangeType()
    {
        if (type == Type.DRAW)
            type = Type.ERASER;
        else
            type = Type.DRAW;
    }
    public enum Type
    {
        DRAW,
        ERASER
    }
    
    public void Start()
    {
        // Disable the hand and gaze ray, we don't want then for this demo and the conflict
        // with the visuals
        //PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
    }

}