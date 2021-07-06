using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRKTExtensions.Gesture;
using UnityEditor;
using UnityEngine;

public class GlobalHandListener : MonoBehaviour,
    IMixedRealitySourceStateHandler, // Handle source detected and lost
    IMixedRealityHandJointHandler // handle joint position updates for hands
{
    private GameObject brushTip;
    private GameObject eraser;
    private GameObject pointer;
    private bool isInDrawMod;
    private bool isDrawing;
    private SimulatedArticulatedHand detectedHand;
    private Brush brush;
    private void OnEnable()
    {
        brushTip = GameObject.Find("BrushTip");
        if (brushTip) brushTip.SetActive(false);
        eraser = GameObject.Find("Eraser");
        if (eraser) eraser.SetActive(false);

        brush = GetComponent<Brush>();
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    void Update()
    {
        if (detectedHand == null) return;
        pointer?.SetActive(isInDrawMod);
        if (!isDrawing) return;
        foreach (var inputMapping in detectedHand.Interactions)
        {
            if (inputMapping.Description != "Grab") continue;
            brush.Draw = inputMapping.BoolData;
            break;
        }
    }

    public bool IsInDrawMod
    {
        get => isInDrawMod;
        set => isInDrawMod = value;
    }

    public bool IsDrawing
    {
        get => isDrawing;
        set => isDrawing = value;
    }

    // IMixedRealitySourceStateHandler interface
    public void OnSourceDetected(SourceStateEventData eventData)
    {
        if (!(eventData.Controller is SimulatedArticulatedHand hand)) return;
        detectedHand = hand;
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (!(eventData.Controller is SimulatedArticulatedHand hand)) return;
        detectedHand = null;
        pointer?.SetActive(false);
    }

    public void OnHandJointsUpdated(
                InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        if (detectedHand == null) return;
        if (!isInDrawMod) return;
        
        pointer = isDrawing ? brushTip : eraser;
        
        if (eventData.InputData.TryGetValue(TrackedHandJoint.IndexTip, out var pose))
        {
            pointer.transform.position = pose.Position;
            pointer.SetActive(true);
        }
    }
}