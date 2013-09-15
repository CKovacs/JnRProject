using UnityEngine;
using System.Collections;
using UnityEditor;

public class Player : ScriptableObject
{
    public GameObject _3dData;
    public string _name;

    public const string MENUPATH    = "JnR/Create/Player";
    public const string ASSETPATH   = "Prefabs/Players";

    [MenuItem(MENUPATH)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Player>(ASSETPATH);
    }
}