using UnityEngine;
using System.Collections;

public class Connect_GUI : MonoBehaviour
{
	public const int margin = 50;
	public const string MENU_CAPTION = "Networking Prototype";
	public const string MENU_BTN_QUICKPLAY = "Quickplay";
	public const string MENU_BTN_REFRESH = "Refresh Hostlist";
	public const string MENU_BTN_START = "Start Server WO";
	public const string MENU_BTN_DCONNECT = "Direct Connect";
	public const string MENU_BTN_CANCEL = "Cancel";
	public const string MENU_LBL_PLAYERNAME = "Player Name:";
	public const string MENU_LBL_ENTERPLAYERNAME = "Please enter a valid player name (No Special Characters!)";
	private Rect _menuHolder;
	private string _connectStatus;
	private bool _quickPlay;
	private float _quickPlayStart;
	public int _maxPlayers = 10;
	private string _ipAddress = "127.0.0.1";
	private string _portHolder = "23482";
	private int _port;
	private Vector2 scrollPos;
	private string _playerName = "Player Default Name";
	private Connector _connector;
	
	private void Awake ()
	{
		this._playerName = PlayerPrefs.GetString ("playerName");
		this._connector = base.GetComponent<Connector> ();
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
				GUI.Label (new Rect ((float)(Screen.width / 2 - 70), (float)(Screen.width / 2 - 6), 140f, 12f), "Loading the game: " + Mathf.Floor (Application.GetStreamProgressForLevel ("Game") * 100f) + " %");
			}
			return;
		}
		this._menuHolder = GUILayout.Window (11111, this._menuHolder, new GUI.WindowFunction (this.FillMainMenu), "Networking Prototype", new GUILayoutOption[0]);
	}

	private void FillMainMenu (int windowID)
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			GUILayout.BeginVertical (new GUILayoutOption[0]);
			GUILayout.BeginHorizontal (new GUILayoutOption[0]);
			GUILayout.Label ("Player Name:", new GUILayoutOption[0]);
			this._playerName = GUILayout.TextField (this._playerName, 25, new GUILayoutOption[0]);
			if (GUI.changed) {
				PlayerPrefs.SetString ("playerName", this._playerName);
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			if (this._playerName == string.Empty) {
				GUILayout.BeginHorizontal (new GUILayoutOption[0]);
				GUILayout.Label ("Please enter a valid player name (No Special Characters!)", new GUILayoutOption[0]);
				GUILayout.EndHorizontal ();
				return;
			}
			GUILayout.BeginHorizontal (new GUILayoutOption[0]);
			GUILayout.BeginVertical (new GUILayoutOption[0]);
			if (GUILayout.Button ("Quickplay", new GUILayoutOption[0])) {
				Debug.Log ("Quickplay");
				this._quickPlay = true;
				this._quickPlayStart = Time.time;
			}
			if (this._quickPlay) {
				this._connectStatus = this._connector.QuickPlay (this._quickPlayStart);
				GUILayout.Label (this._connectStatus, new GUILayoutOption[0]);
				if (GUILayout.Button ("Cancel", new GUILayoutOption[0])) {
					Network.Disconnect ();
					this._quickPlay = false;
				}
				if (this._connectStatus == "failed") {
					Debug.Log ("PlayNow: No games hosted, so hosting one ourselves");
					this._connector.StartHost (9, this._playerName, this._connector._serverPort);
				}
			}
			if (GUILayout.Button ("Start Server WO", new GUILayoutOption[0])) {
				Debug.Log ("Startserver");
				this._connector.StartServer ();
			}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical (new GUILayoutOption[0]);
			if (GUILayout.Button ("Refresh Hostlist", new GUILayoutOption[0])) {
				Debug.Log ("Refreshing Manual");
				this._connector.RefreshHostList (true);
			}
			this._connector.RefreshHostList (false);
			if (this._connector._hostList != null) 
			{
				this.scrollPos = GUILayout.BeginScrollView (this.scrollPos, new GUILayoutOption[0]);
				HostData[] hostList = this._connector._hostList;
				for (int i = 0; i < hostList.Length; i++) {
					HostData hostData = hostList [i];
					GUILayout.BeginHorizontal (new GUILayoutOption[0]);
					GUILayout.Label (hostData.gameName, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace ();
					GUILayout.Label (hostData.connectedPlayers + "/" + hostData.playerLimit, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace ();
					if (!this._quickPlay) 
					{
						if (GUILayout.Button ("Connect", new GUILayoutOption[0])) 
						{
							this._connector.Connect (hostData);
						}
					} else 
					{
						if (GUILayout.Button ("Cancel", new GUILayoutOption[0])) 
						{
							Network.Disconnect ();
						}
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndScrollView ();
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal (new GUILayoutOption[0]);
			if (GUILayout.Button ("Direct Connect", new GUILayoutOption[0])) 
			{
				Debug.Log ("Direct Connect");
                this._connector.Connect (_ipAddress,_port);
			}
			GUILayout.BeginHorizontal (new GUILayoutOption[0]);
			this._ipAddress = GUILayout.TextField (this._ipAddress, 25, new GUILayoutOption[0]);
			this._portHolder = GUILayout.TextField (this._portHolder, 25, new GUILayoutOption[0]);
            if(_portHolder != "") 
            {
			    this._port = int.Parse (this._portHolder);
			}
            GUILayout.EndHorizontal ();
			GUILayout.EndHorizontal ();
		} 
		else 
		{
			if (GUILayout.Button ("Cancel", new GUILayoutOption[0])) {
				Debug.Log ("Disconnect");
				this._quickPlay = false;
				Network.Disconnect ();
			}
		}
	}
}
