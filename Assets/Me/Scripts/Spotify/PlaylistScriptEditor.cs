using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaylistScript))]
public class PlaylistScriptEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaylistScript playlistScript = (PlaylistScript)target;
        if (GUILayout.Button("Play Track"))
        {
            playlistScript.PlaySomething();
        }
    }
}
