using System;
using Dialogflow.Handler;
using UnityEditor;
using UnityEngine;

namespace Dialogflow.Editor {
    [CustomEditor(typeof(DialogFlowClient))]
    public class DialogFlowClientEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var client = target as DialogFlowClient;
            serializedObject.Update();
            var microphoneNames = Microphone.devices;

            var microphoneIndex = string.IsNullOrEmpty(client.microphoneName)
                ? 0
                : Array.IndexOf(microphoneNames, client.microphoneName);
            if (microphoneIndex == -1) {
                microphoneIndex = 0;
            }

            client.microphoneName =
                microphoneNames[EditorGUILayout.Popup("Microphone", microphoneIndex, microphoneNames)];

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onAudioResponse"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onTextResponse"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
