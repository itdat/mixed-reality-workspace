using System.Collections;
using System.Collections.Generic;
using Dialogflow;
using UnityEngine;

public class KeyPressHandler : MonoBehaviour
{
    [SerializeField] private DialogFlowClient client;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            client.OnButtonRecord();
        }
    }
}
