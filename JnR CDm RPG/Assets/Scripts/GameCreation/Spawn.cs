using UnityEngine;
using System.Collections;

public class Spawn
{
    private GameStateObject _state;
    private LevelData _data;

    public Spawn(GameStateObject state, LevelData data)
    {
        _state = state;
        _data = data;
    }

    public void DoSpawn()
    {
        foreach (Cell c in _data._cells)
        {
            switch (c._type)
            {
                case CellType.BlueTeamSpawn:
                    //Player player = _state._blue.Find(p => p != null && p._name == c._playerName);

                    foreach (Player blue in _state._blue) 
                    {
                        GameObject spawnedPlayer = GameObject.Instantiate(blue._3dData, Vector3.zero, Quaternion.identity) as GameObject;
                    }
                    
                    break;
                case CellType.RedTeamSpawn:
                    foreach (Player red in _state._red)
                    {
                        GameObject spawnedPlayer = GameObject.Instantiate(red._3dData, Vector3.zero, Quaternion.identity) as GameObject;
                    }

                    break;
                /*
                case CellType.BlueFlag:
                    _cell.renderer.sharedMaterial.color = Color.cyan;
                    break;

                case CellType.RedSpawn:
                    _cell.renderer.sharedMaterial.color = Color.red;
                    break;
                case CellType.RedFlag:
                    _cell.renderer.sharedMaterial.color = Color.magenta;
                    break;*/

            }

            if (_state._type == JnRGameType.CaptureTheFlag)
            {
                //Flag stuff
            }
        }
    }
}
