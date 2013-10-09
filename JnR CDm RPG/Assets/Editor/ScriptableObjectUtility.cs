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
    private const string MENUPATHGSO = "JnR/Create/GameState";
    private const string ASSETPATHGSO = "Prefabs/GameStates";
    private const string MENUPATHPLAYER = "JnR/Create/Player";
    private const string ASSETPATHPLAYER = "Prefabs/Players";

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

    public class SkillMenu 
    {
        [UnityEditor.MenuItem(MENUPATHSKILL)]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<Skill>(ASSETPATHSKILL);
        }
    }

    public class GameStateObjectMenu
    {
        [UnityEditor.MenuItem(MENUPATHGSO)]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<GameStateObject>(ASSETPATHGSO);
        }
    }

    public class PlayerMenu
    {
        [UnityEditor.MenuItem(MENUPATHPLAYER)]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<Player>(ASSETPATHPLAYER);
        }
    }
}