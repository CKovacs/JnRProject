using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InfoBox))]
public class InfoBoxEditor : Editor
{
    public SerializedProperty _infoProp;

    public const string INFOTEXT = "_infoText";

    void OnEnable()
    {
        _infoProp = serializedObject.FindProperty(INFOTEXT);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _infoProp.stringValue = EditorGUILayout.TextArea(_infoProp.stringValue, GUILayout.MaxHeight(75));
        serializedObject.ApplyModifiedProperties();
    }
}
