using UnityEngine;
using System.Collections;

public class MainLoop : MonoBehaviour 
{
    public GameStateObject _state;
    private Spawn _spawn;

	void Start () 
    {
        _spawn = new Spawn(_state, GetComponentInChildren<LevelData>());
        _spawn.DoSpawn();
	}
}
