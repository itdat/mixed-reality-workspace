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

    public GameObject Pointer => pointer;

    private GameObject eraserTip;
    private GameObject pointer;
    private bool isInDrawMod;
    private bool isDrawing;
    private IMixedRealityController detectedHand;
    private Brush brush;
    private Eraser eraser;

    private void OnEnable() {
        brushTip = GameObject.Find("BrushTip");
        if (brushTip) brushTip.SetActive(false);
        eraserTip = GameObject.Find("Eraser");
        if (eraserTip) eraserTip.SetActive(false);
        pointer = brushTip;
        brush = GetComponent<Brush>();
        eraser = GetComponent<Eraser>();
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable() {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    void Update() {
        if (detectedHand == null) return;
        pointer.SetActive(true);
        //if (!isDrawing) return;
        foreach (var inputMapping in detectedHand.Interactions) {
            if (inputMapping.Description != "Grab") continue;

            if (isDrawing) {
                brush.Draw = inputMapping.BoolData;
            }
            else {
                eraser.Draw = inputMapping.BoolData;
            }

            break;
        }
    }

    public bool IsInDrawMod {
        get => isInDrawMod;
        set => isInDrawMod = value;
    }

    public bool IsDrawing {
        get => isDrawing;
        set => isDrawing = value;
    }

    // IMixedRealitySourceStateHandler interface
    public void OnSourceDetected(SourceStateEventData eventData) {
        if (eventData.Controller.InputSource.SourceType != InputSourceType.Hand) return;
        detectedHand = eventData.Controller;
        pointer.SetActive(true);
        Debug.Log("OnSourceDetected");
    }

    public void OnSourceLost(SourceStateEventData eventData) {
        // if (!(eventData.Controller is SimulatedArticulatedHand hand)) return;
        if (eventData.Controller.InputSource.SourceType != InputSourceType.Hand) return;
        detectedHand = null;
        pointer.SetActive(false);
        Debug.Log("OnSourceLost");
    }

    public void OnHandJointsUpdated(
        InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData) {
        if (detectedHand == null) return;
        if (!isInDrawMod) return;
        Debug.Log("OnHandJointsUpdated");

        pointer = isDrawing ? brushTip : eraserTip;
        brushTip.SetActive(true);
        eraserTip.SetActive(!isDrawing);

        if (!eventData.InputData.TryGetValue(TrackedHandJoint.IndexTip, out var pose)) return;
        pointer.transform.position = pose.Position;
        //pointer.SetActive(true);
    }
}
