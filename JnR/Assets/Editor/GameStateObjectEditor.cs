using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameStateObject))]
public class GameStateObjectEditor : Editor
{
    private GameStateObject _state;

    private const string TEAMBLUE = "Team blue";
    private const string TEAMRED = "Team red";
    private const string GAMETYPE = "Game type";
    private const string MAXTIME = "Max time in seconds";

    // Buttons
    private const string ADDBLUE = "Add blue player";
    private const string ADDRED = "Add red player";
    private const string DELBLUE = "Delete blue player";
    private const string DELRED = "Delete red player";

    // CTF
    private const string MAXFLAGCOUNT = "Flag max count";

    // TDM
    private const string MAXDDEEATHBLUE = "Blue max death count";
    private const string MAXDDEEATHRED = "Red max death count";

    public void Awake()
    {
        _state = target as GameStateObject;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField(TEAMBLUE);

        for (int i = 0; i < _state._blue.Count; ++i)
        {
            _state._blue[i] = (EditorGUILayout.ObjectField(_state._blue[i], typeof(Player), false) as Player);
        }

        if(GUILayout.Button(ADDBLUE)) 
        {
            _state._blue.Add(new Player());
        }
        if (GUILayout.Button(DELBLUE)) 
        {
            _state._blue.RemoveAt(_state._blue.Count - 1);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(TEAMRED);

        for (int i = 0; i < _state._red.Count; ++i)
        {
            _state._red[i] = (EditorGUILayout.ObjectField(_state._red[i], typeof(Player), false) as Player);
        }

        if (GUILayout.Button(ADDRED))
        {
            _state._red.Add(new Player());
        }
        if (GUILayout.Button(DELRED))
        {
            _state._red.RemoveAt(_state._red.Count - 1);
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
