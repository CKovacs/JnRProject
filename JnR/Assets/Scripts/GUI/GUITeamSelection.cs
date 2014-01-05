using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUITeamSelection : MonoBehaviour
{
	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	public Transform _gameManagementObject;
	private GameManager _gameManager;
	public IEnumerable<PlayerState> _playerList; 
	private Vector3 _scale;

	public GUIStyle playerNotSelectedGUIStyle;
	public GUIStyle playerSelectedGUIStyle;
	public Texture2D teamSelectionBackground;

	// Use this for initialization
	private void Start()
	{
		_playerList = new List<PlayerState>();
		_gameManager = _gameManagementObject.GetComponent<GameManager>();
	}

	// Update is called once per frame
	private void Update()
	{
		_playerList = _gameManager.GetPlayerList();
		
	}

	private void OnGUI()
	{
		if (Network.isClient)
		{
			//scaling stuff for different resolutions
			float rx = Screen.width / _originalWidth;
			float ry = Screen.height / _originalHeight;

			//Background
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), teamSelectionBackground);

			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

			//Draw non-connected Players
			int countNotConnectedPlayers = 0;
			int tmp;
			foreach (PlayerState player in _playerList)
			{
				if (player._team == Team.None)
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

					GUI.Box(new Rect((Screen.width / 2) + tmp, 280 + offset, 283, 56), "" + player.name, playerNotSelectedGUIStyle);
					countNotConnectedPlayers++;
				}
			}

			//Draw players in Team A
			int countTeamAPlayers = 0;
			foreach (PlayerState player in _playerList)
			{
				if (player._team == Team.Blue)
				{
					int offset = countTeamAPlayers * 113;

					GUI.Box(new Rect(168, 282 + offset, 283, 56), "" + player.name, playerSelectedGUIStyle);
					countTeamAPlayers++;
				}
			}

			//Draw players in Team B
			int countTeamBPlayers = 0;
			foreach (PlayerState player in _playerList)
			{
				if (player._team == Team.Red)
				{
					int offset = countTeamBPlayers * 113;

					GUI.Box(new Rect(1473, 282 + offset, 283, 56), "" + player.name, playerSelectedGUIStyle);
					countTeamBPlayers++;
				}
			} 
		}
	}
}