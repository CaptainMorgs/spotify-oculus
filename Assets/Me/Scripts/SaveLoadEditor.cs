using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveLoad))]
public class SaveLoadEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveLoad saveLoad = (SaveLoad)target;
        if (GUILayout.Button("Reload"))
        {
            saveLoad.Reload();
        }
        if (GUILayout.Button("Clear Data"))
        {
            saveLoad.ClearData();
        }
        if (GUILayout.Button("Save"))
        {
            saveLoad.Save();
        }
    }
}
