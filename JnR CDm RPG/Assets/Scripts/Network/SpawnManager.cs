using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the SpawnManager and it allows
/// the player to spawn into the multiplayer game.
/// </summary>


public class SpawnManager : MonoBehaviour
{

    //Variables Start___________________________________

    //Used to determine if the palyer needs to spawn into
    //the game.

    private bool justConnectedToServer = false;


    //Used to determine which team the player is on.

    public bool amIOnTheRedTeam = false;

    public bool amIOnTheBlueTeam = false;


    //Used to define the JoinTeamWindow.

    private Rect joinTeamRect;

    private string joinTeamWindowTitle = "Team Selection";

    private int joinTeamWindowWidth = 330;

    private int joinTeamWindowHeight = 100;

    private int joinTeamLeftIndent;

    private int joinTeamTopIndent;

    private int buttonHeight = 40;


    //The Player prefabs are connected to these in the 
    //inspector

    public Transform redTeamPlayer;

    public Transform blueTeamPlayer;

    private int redTeamGroup = 0;

    private int blueTeamGroup = 1;


    //Used to capture spawn points.

    private GameObject[] redSpawnPoints;

    private GameObject[] blueSpawnPoints;


    //Variables End_____________________________________

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnConnectedToServer()
    {
        justConnectedToServer = true;
    }



    void JoinTeamWindow(int windowID)
    {
        //If the player clicks on the Join Red Team button then
        //assign them to the red team and spawn them into the game.

        if (GUILayout.Button("Join Red Team", GUILayout.Height(buttonHeight)))
        {
            amIOnTheRedTeam = true;

            justConnectedToServer = false;

            SpawnRedTeamPlayer();
        }


        //If the player clicks on the Join Blue Team button then
        //assign them to the blue team and spawn them into the game.

        if (GUILayout.Button("Join Blue Team", GUILayout.Height(buttonHeight)))
        {
            amIOnTheBlueTeam = true;

            justConnectedToServer = false;

            SpawnBlueTeamPlayer();
        }
    }


    void OnGUI()
    {
        //If the player has just connected to the server then draw the 
        //Join Team window.

        if (justConnectedToServer == true)
        {
            joinTeamLeftIndent = Screen.width / 2 - joinTeamWindowWidth / 2;

            joinTeamTopIndent = Screen.height / 2 - joinTeamWindowHeight / 2;

            joinTeamRect = new Rect(joinTeamLeftIndent, joinTeamTopIndent,
                                    joinTeamWindowWidth, joinTeamWindowHeight);

            joinTeamRect = GUILayout.Window(0, joinTeamRect, JoinTeamWindow,
                                            joinTeamWindowTitle);
        }
    }


    void SpawnRedTeamPlayer()
    {
        //Find all red spawn points and place a reference to them in the array
        //redSpawnPoints.

        redSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnRedTeam");


        //Randomly select one of those spawn points.

        GameObject randomRedSpawn = redSpawnPoints[Random.Range(0, redSpawnPoints.Length)];


        //Instantiate the player at the randomly selected spawn point.

        Network.Instantiate(redTeamPlayer, randomRedSpawn.transform.position,
                            randomRedSpawn.transform.rotation, redTeamGroup);
    }



    void SpawnBlueTeamPlayer()
    {
        //Find all blue spawn points and place a reference to them in the array
        //blueSpawnPoints.

        blueSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnBlueTeam");


        //Randomly select one of those spawn points.

        GameObject randomBlueSpawn = blueSpawnPoints[Random.Range(0, blueSpawnPoints.Length)];


        //Instantiate the player at the randomly selected spawn point.

        Network.Instantiate(blueTeamPlayer, randomBlueSpawn.transform.position,
                            randomBlueSpawn.transform.rotation, blueTeamGroup);
    }






























}
