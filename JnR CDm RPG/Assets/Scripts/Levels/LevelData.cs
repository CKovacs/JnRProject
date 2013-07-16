using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class LevelData : ScriptableObject 
{
    public GameStateObject _state;

    public const string MENUPATH = "JnR/Create/LevelData";
    public const string ASSETPATH = "Prefabs/LevelData";

    [MenuItem(MENUPATH)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<LevelData>(ASSETPATH);
    }
}