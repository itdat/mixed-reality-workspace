﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] listPrefabs;
    private GameObject pointer;

    private void OnEnable()
    {
        var go = GameObject.Find("GlobalHandListener");
        if (go == null) return;
        var listener = go.GetComponent<GlobalHandListener>();
        if (listener == null) return;
        pointer = listener.Pointer;
    }

    public void SpawnStuff(int id)
    {
        if (id < 0 || id > listPrefabs.Length) return;
        var go = Instantiate(listPrefabs[id]);
        go.tag = "Stuff"; 
        //go.transform.position = pointer.transform.position + Vector3.up;
        go.SetActive(true);
    }
}
