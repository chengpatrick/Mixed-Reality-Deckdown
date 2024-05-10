using Oculus.Interaction.Surfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PokeableButtonComponentsAttachTool : MonoBehaviour
{
    public bool isAttached;

    public void AttachScripts()
    {
        // Add scripts
        Type[] scriptTypes = {
            typeof(Oculus.Interaction.Surfaces.BoundsClipper),
            typeof(Oculus.Interaction.Surfaces.ClippedPlaneSurface),
            typeof(Oculus.Interaction.Surfaces.PlaneSurface),
            typeof(Oculus.Interaction.PokeInteractable),
            typeof(Oculus.Interaction.InteractableUnityEventWrapper)
        };

        foreach (Type scriptType in scriptTypes)
        {
            // Ensure the scriptType is derived from MonoBehaviour before trying to add it
            if (typeof(MonoBehaviour).IsAssignableFrom(scriptType))
            {
                // Add the script to the GameObject
                MonoBehaviour scriptInstance = gameObject.AddComponent(scriptType) as MonoBehaviour;

                if (scriptInstance != null)
                {
                    // Optionally, you can perform additional setup or configuration here
                    // For example, you might want to set properties on the attached scripts.
                }
                else
                {
                    Debug.LogError($"Failed to add script: {scriptType}");
                }
            }
            else
            {
                Debug.LogError($"Script type is not derived from MonoBehaviour: {scriptType}");
            }
        }
    }
}
