using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameStateObject))]
public class GameStateObjectEditor : Editor
{
    private GameStateObject _state;

    public const string TEAMBLUE        = "Team blue";
    public const string TEAMRED         = "Team red";
    public const string ADDBLUE         = "Add blue player";
    public const string ADDRED          = "Add red player";
    public const string GAMETYPE        = "Game type";
    public const string MAXTIME         = "Max time in seconds";

    //CTF
    public const string MAXFLAGCOUNT    = "Flag max count";

    //TDM
    public const string MAXDDEEATHBLUE  = "Blue max death count";
    public const string MAXDDEEATHRED   = "Red max death count";

    void Awake()
    {
        _state = target as GameStateObject;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField(TEAMBLUE);

        for (int i = 0; i < _state._blue.Count; ++i)
        {
            _state._blue[i] = (EditorGUILayout.ObjectField(_state._blue[i], typeof(Player)) as Player);
        }

        if(GUILayout.Button(ADDBLUE)) 
        {
            _state._blue.Add(new Player());
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(TEAMRED);

        for (int i = 0; i < _state._red.Count; ++i)
        {
            _state._red[i] = (EditorGUILayout.ObjectField(_state._red[i], typeof(Player)) as Player);
        }

        if (GUILayout.Button(ADDRED))
        {
            _state._red.Add(new Player());
        }

        EditorGUILayout.Space();

        _state._type = (JnRGameType)EditorGUILayout.EnumPopup(GAMETYPE, _state._type);

        _state._timeInSeconds = EditorGUILayout.IntField(MAXTIME, _state._timeInSeconds);

        switch (_state._type)
        {
            case JnRGameType.CaptureTheFlag:
                _state._flagMaxCount = EditorGUILayout.IntField(MAXFLAGCOUNT, _state._flagMaxCount);
                break;
            case JnRGameType.DeathMatch:
                _state._blueMaxDeaths = EditorGUILayout.IntField(MAXDDEEATHBLUE, _state._blueMaxDeaths);
                _state._redMaxDeaths = EditorGUILayout.IntField(MAXDDEEATHRED, _state._redMaxDeaths);
                break;
        }
    }
}
