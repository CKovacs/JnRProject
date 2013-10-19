using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    private int cellsInX = 1;
    private int cellsInY = 1;

    private const string MENUPATH = "JnR/Level-Editor";
    private const string CELLPREFAB = "Assets/Prefabs/LevelData/Cell.prefab";

    // GameObject names
    private const string GAMEMANAGER = "GameManager";
    private const string LEVELNAME = "CellGrid";
    private const string LEVELDATA = "LevelData";
    private const string CELL = "Cell";

    // Buttons
    private const string CONVERTTOLEVELDATA = "Convert to LevelData";
    private const string CONVERTTOCELLGRID = "Convert to CellGrid";
    private const string CREATEGRID = "Create grid";
    private const string UPDATEGRID = "Update grid";

    // Text
    private const string XTEXT = "Cells in X";
    private const string YTEXT = "Cells in Y";

    [MenuItem(MENUPATH)]
    static void Init()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
    }

    void OnGUI()
    {
        // GameManager stuff
        GameObject gameManager = GameObject.Find(GAMEMANAGER);

        if (gameManager == null)
        {
            gameManager = new GameObject();

            gameManager.name = GAMEMANAGER;
            gameManager.AddComponent<MainLoop>();
        }

        // Level Editor stuff
        GameObject gridObject = GameObject.Find(LEVELNAME);
        LevelData levelDataObject = GameObject.FindObjectOfType(typeof(LevelData)) as LevelData;

        if (gridObject == null && levelDataObject == null)
        {
            cellsInX = EditorGUILayout.IntField(XTEXT, cellsInX);
            cellsInY = EditorGUILayout.IntField(YTEXT, cellsInY);

            if (GUILayout.Button(CREATEGRID))
            {
                GameObject level = new GameObject();

                level.name = LEVELNAME;
                level.transform.parent = gameManager.transform;
                level.AddComponent<Grid>();
                Grid gData = level.GetComponent<Grid>();
                
                gData._cellsInX = cellsInX;
                gData._cellsInY = cellsInY;

                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(CELLPREFAB, typeof(GameObject));

                for (int i = 0; i < cellsInX; ++i)
                {
                    for (int j = 0; j < cellsInY; ++j)
                    {
                        GameObject cell = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

                        cell.name = CELL + i + j;
                        cell.transform.parent = level.transform;
                        cell.transform.position = new Vector3(i * (cell.renderer.bounds.size.x), 0, j * (cell.renderer.bounds.size.z));
                        cell.GetComponent<Cell>()._pos = cell.transform.position;
                    }
                }
            }
        }
        else if (gridObject != null)
        {
            Grid gData = gridObject.GetComponent<Grid>();
            cellsInX = EditorGUILayout.IntField(XTEXT, cellsInX);
            cellsInY = EditorGUILayout.IntField(YTEXT, cellsInY);

            if (GUILayout.Button(CONVERTTOLEVELDATA))
            {
                List<Cell> cells = new List<Cell>();

                foreach(Cell c in gridObject.GetComponentsInChildren<Cell>())
                {
                    if (c._type != CellType.Empty)
                    {
                        cells.Add(c);
                    }
                }

                GameObject levelData = new GameObject();
                levelData.name = LEVELDATA;
                levelData.transform.parent = gameManager.transform;

                levelData.AddComponent<LevelData>();
                LevelData lData = levelData.GetComponent<LevelData>();
                lData._cells = cells;
                lData._gridReference = gridObject;

                gridObject.SetActive(false);
            }
            if (GUILayout.Button(UPDATEGRID))
            {
                for (int i = 0; i < gData._cellsInX; ++i)
                {
                    for (int j = 0; j < gData._cellsInY; ++j)
                    {
                        if (i >= cellsInX || j >= cellsInY)
                        {
                            DestroyImmediate(GameObject.Find(CELL + i + j));
                        }
                    }
                }

                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(CELLPREFAB, typeof(GameObject));

                for (int i = 0; i < cellsInX; ++i)
                {
                    for (int j = 0; j < cellsInY; ++j)
                    {
                        if ((gData._cellsInX < cellsInX || gData._cellsInY < cellsInY) && GameObject.Find(CELL + i + j) == null)
                        {
                            GameObject cell = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

                            cell.name = CELL + i + j;
                            cell.transform.parent = gridObject.transform;
                            cell.transform.position = new Vector3(i * (cell.renderer.bounds.size.x), 0, j * (cell.renderer.bounds.size.z));
                            cell.GetComponent<Cell>()._pos = cell.transform.position;
                        }
                    }
                }

                gData._cellsInX = cellsInX;
                gData._cellsInY = cellsInY;
            }
        }
        else if (levelDataObject != null)
        {
            if (GUILayout.Button(CONVERTTOCELLGRID))
            {
                levelDataObject._gridReference.SetActive(true);
                DestroyImmediate(levelDataObject.gameObject);
            }
        }
    }
}