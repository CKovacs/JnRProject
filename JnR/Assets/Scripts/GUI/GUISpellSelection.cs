using System.Collections.Generic;
using UnityEngine;

public class GUISpellSelection : MonoBehaviour
{
	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	public Transform _gameManagementObject;
	private GameManager _gameManager;
	public IEnumerable<PlayerState> _playerList;
	private Vector3 _scale;
	public Texture2D spellSelectionBackground;
	// Use this for initialization
	private void Start()
	{
		_playerList = new List<PlayerState>();
		_gameManager = _gameManagementObject.GetComponent<GameManager>();
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void OnGUI()
	{
		if (Network.isClient)
		{
			//scaling stuff for different resolutions
			float rx = Screen.width/_originalWidth;
			float ry = Screen.height/_originalHeight;

			//Background
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), spellSelectionBackground);

			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));
		}
	}
}