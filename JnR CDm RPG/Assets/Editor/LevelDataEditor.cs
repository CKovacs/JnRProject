using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    //This will just be a shortcut to the target, ex: the object you clicked on.
    private LevelData data;

    void Awake()
    {
        data = (LevelData)target;
    }

    public override void OnInspectorGUI()
    {
        /*GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        // Grid data

        // CTF data
        groupEnabled = EditorGUILayout.BeginToggleGroup(CTFDATA, groupEnabled);
        _flagRespawn = EditorGUILayout.IntField(FLAGRESPAWN, _flagRespawn);
        EditorGUILayout.EndToggleGroup();*/
    }
}