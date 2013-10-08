using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Cell))]
public class CellEditor : Editor
{
    private Cell _cell;

    private const string TYPE = "Type";
    private const string HEIGHT = "Height";
    private const string PLAYERNAME = "Player name";
    
    private const string MATERIALPATH = "Assets/Prefabs/LevelData/CellMat.mat";

    public void Awake()
    {
        _cell = target as Cell;
    }

    public override void OnInspectorGUI()
    {
        _cell._type = (CellType)EditorGUILayout.EnumPopup(TYPE, _cell._type);

        _cell._height = EditorGUILayout.IntField(HEIGHT, _cell._height);
        _cell._playerName = EditorGUILayout.TextField(PLAYERNAME, _cell._playerName);

        Object prefab = AssetDatabase.LoadAssetAtPath(MATERIALPATH, typeof(Material));

        Material mat = Instantiate(prefab, Vector3.zero, Quaternion.identity) as Material;
        _cell.renderer.sharedMaterial = mat;

        switch (_cell._type)
        {
            case CellType.Empty:
                _cell.renderer.sharedMaterial.color = Color.white;
                break;

            case CellType.BlueTeamSpawn:
                _cell.renderer.sharedMaterial.color = Color.blue;
                break;
            case CellType.BlueFlag:
                _cell.renderer.sharedMaterial.color = Color.cyan;
                break;

            case CellType.RedTeamSpawn:
                _cell.renderer.sharedMaterial.color = Color.red;
                break;
            case CellType.RedFlag:
                _cell.renderer.sharedMaterial.color = Color.magenta;
                break;
        }
    }
}
