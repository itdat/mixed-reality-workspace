using System;
using Dialogflow;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(ClientView))]
    public class ClientEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            var foldout = true;
            EditorGUILayout.Foldout(foldout, "Audio config");

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recordButtonText"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Foldout(foldout, "Text config");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("content"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outputText"));
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("imageResult"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
