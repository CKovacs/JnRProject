using UnityEngine;
using System.Collections;

public class Player : Target
{
    public Class _class;

    public const string MENUPATH    = "JnR/Create/Player";
    public const string ASSETPATH   = "Prefabs/Players";

    [UnityEditor.MenuItem(MENUPATH)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Player>(ASSETPATH);
    }
}