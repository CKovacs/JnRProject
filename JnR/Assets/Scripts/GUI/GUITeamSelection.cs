using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUITeamSelection : MonoBehaviour
{
    private const float _originalWidth = 1920.0f;
    private const float _originalHeight = 1080.0f;
    public Transform _gameManagementObject;
    private GameManager _gameManager;
    public IEnumerable<PlayerMock> _players;
    private Vector3 _scale;
  
    public Texture2D teamSelectionBackground;
    public GUIStyle playerSelectedGUIStyle, playerNotSelectedGUIStyle;
  
    // Use this for initialization
    private void Start()
    {
       
        _gameManager = _gameManagementObject.GetComponent<GameManager>();
        _players = _gameManager.GetConnectedPlayers();
    }

    // Update is called once per frame
    private void Update()
    {
        _players = _gameManager.GetConnectedPlayers();
    }

    private void OnGUI()
    {
        //scaling stuff for different resolutions
        float rx = Screen.width/_originalWidth;
        float ry = Screen.height/_originalHeight;

        //Background
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), teamSelectionBackground);

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

        //Draw non-connected Players
        int countNotConnectedPlayers = 0;
        int tmp;
        foreach (PlayerMock player in _players)
        {
            if (player.team == SelectedTeam.None)
            {
                int offset = countNotConnectedPlayers * 60;

                if (countNotConnectedPlayers < 5)
                {
                    tmp = -350;
                }
                else
                {
                    tmp = 67;
                    offset = (countNotConnectedPlayers - 5) * 60;
                }

                GUI.Box(new Rect((Screen.width/2) + tmp, 280 + offset, 283, 56), "" + player.name, playerNotSelectedGUIStyle);
                countNotConnectedPlayers++;
            }
        }

        //Draw players in Team A
        int countTeamAPlayers = 0;
        foreach (PlayerMock player in _players)
        {
            if (player.team == SelectedTeam.Red)
            {
                int offset = countTeamAPlayers * 113;

                GUI.Box(new Rect(168, 282 + offset, 283, 56), "" + player.name, playerSelectedGUIStyle);
                countTeamAPlayers++;
            }
        }

        //Draw players in Team B
        int countTeamBPlayers = 0;
        foreach (PlayerMock player in _players)
        {
            if (player.team == SelectedTeam.Blue)
            {
                int offset = countTeamBPlayers * 113;

                GUI.Box(new Rect(1473, 282 + offset, 283, 56), "" + player.name, playerSelectedGUIStyle);
                countTeamBPlayers++;
            }
        }

        
    }
   

}