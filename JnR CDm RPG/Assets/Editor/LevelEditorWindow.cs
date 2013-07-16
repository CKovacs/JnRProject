using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Window/My Window")]
    static void Init()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        if (groupEnabled) 
        {
            GameObject level = new GameObject();

            level.name = "Level";
            GameObject gridCell = GameObject.CreatePrimitive(PrimitiveType.Plane);

            gridCell.transform.parent = level.transform;  
        }
    }
}