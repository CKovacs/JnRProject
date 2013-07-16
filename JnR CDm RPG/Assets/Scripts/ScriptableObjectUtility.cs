using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    public const string PATH    = "Assets";
    public const string SLASH   = "/";
    public const string NEW     = "New ";
    public const string EXT     = ".asset";

    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary> 
    public static void CreateAsset<T>(string pathAdd) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(PATH + SLASH + pathAdd + SLASH + NEW + typeof(T).ToString() + EXT);

        Debug.Log(assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}