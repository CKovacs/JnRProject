using UnityEngine;
using System.IO;

public static class ScriptableObjectUtility
{
    private const string PATH = "Assets";
    private const string SLASH = "/";
    private const string NEW = "New ";
    private const string EXT = ".asset";
    private const string MENUPATHSKILL = "JnR/Create/Skill";
    private const string ASSETPATHSKILL = "Prefabs/Skills";

    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary> 
    public static void CreateAsset<T>(string pathAdd) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(PATH + SLASH + pathAdd + SLASH + NEW + typeof(T).ToString() + EXT);

        Debug.Log(assetPathAndName);
        UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem(MENUPATHSKILL)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Skill>(ASSETPATHSKILL);
    }
}