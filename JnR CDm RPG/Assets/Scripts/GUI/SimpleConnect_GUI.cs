using UnityEngine;
using System.Collections;

public class SimpleConnect_GUI : MonoBehaviour 
{
	private string _playerName = "Player Default Name";
	private DirectConnector _connector;
	private Rect _menuHolder;
	
	private void Awake ()
	{
		this._playerName = PlayerPrefs.GetString ("playerName");
		this._connector = base.GetComponent<DirectConnector> ();
	}
	
	private void Start ()
	{
		this._menuHolder = new Rect (50f, 50f, (float)(Screen.width - 100), (float)(Screen.height - 150));
	}
	
	private void OnGUI ()
	{
		if (Network.isClient || Network.isServer) {
			if (Application.CanStreamedLevelBeLoaded (Application.loadedLevel + 1)) {
				GUI.Label (new Rect ((float)(Screen.width / 2 - 70), (float)(Screen.width / 2 - 6), 140f, 12f), "Starting the game!");
				Application.LoadLevel (Application.loadedLevel + 1);
			} else {
				GUI.Label (new Rect ((float)(Screen.width / 2 - 70), (float)(Screen.width / 2 - 6), 140f, 12f), "Loading the game: " + Mathf.Floor (Application.GetStreamProgressForLevel ("Game_Test_Scene") * 100f) + " %");
			}
			return;
		}
		this._menuHolder = GUILayout.Window (11111, this._menuHolder, new GUI.WindowFunction (this.FillMainMenu), "Networking Prototype", new GUILayoutOption[0]);
	}

	private void FillMainMenu (int windowID)
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (GUILayout.Button ("Start Server WO", new GUILayoutOption[0])) {
				Debug.Log ("Startserver");
				this._connector.StartServer ();
			}
			
			if (GUILayout.Button ("Direct Connect", new GUILayoutOption[0])) 
			{
				Debug.Log ("Direct Connect");
				this._connector.Connect ();
			}
		}
	}
}
