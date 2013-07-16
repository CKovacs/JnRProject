using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class GameStateObject : ScriptableObject 
{
    public List<Player> _blue;
    public List<Player> _red;
    //Spectators?
    public JnRGameType  _type;

    public int          _timeInSeconds;

    //Capture the flag specific
    public int          _flagMaxCount;

    //Team Deathmatch specific
    public int          _blueMaxDeaths;
    public int          _redMaxDeaths;

    public const string MENUPATH    = "JnR/Create/GameState";
    public const string ASSETPATH   = "Prefabs/GameStates";

    [MenuItem(MENUPATH)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GameStateObject>(ASSETPATH);
    }
}
