using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(MapDisplay))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDisplay mapDis = (MapDisplay)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapDis.GenerateMap();
        }
    }
}
