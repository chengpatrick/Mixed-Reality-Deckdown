using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Oculus;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;

[CustomEditor(typeof(PokeableButtonComponentsAttachTool))]
public class PokeableButtonComponentsAttachToolEditor : Editor
{
    SerializedProperty isAttachedProp;

    private void OnEnable()
    {
        isAttachedProp = serializedObject.FindProperty("isAttached");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(isAttachedProp);

        if (GUILayout.Button("Attach"))
        {
            isAttachedProp.boolValue = !isAttachedProp.boolValue;
            serializedObject.ApplyModifiedProperties();

            if (isAttachedProp.boolValue)
            {
                PokeableButtonComponentsAttachTool pokeableButton = (PokeableButtonComponentsAttachTool)target;
                pokeableButton.AttachScripts(); 
            }
        }
    }
}