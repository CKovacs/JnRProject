using UnityEngine;

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
	private string _connectStatus;
	private Connector _connector;
	private string _ipAddress = "127.0.0.1";
	public int _maxPlayers = 10;
	private Rect _menuHolder;
	private string _playerName = "Player Default Name";
	private int _port;
	private string _portHolder = "23482";
	private bool _quickPlay;
	private float _quickPlayStart;
	private bool _timeToUpdateList;
	private bool _timeToUpdateListForce;
	private Vector2 scrollPos;

	private void Awake()
	{
		_playerName = string.Empty;
		_connector = base.GetComponent<Connector>();
	}

	private void Start()
	{
		_menuHolder = new Rect(50f, 50f, Screen.width - 100, Screen.height - 150);
	}

	private void Update()
	{
		if (_timeToUpdateList)
		{
			_connector.RefreshHostList(false);
			_timeToUpdateList = false;
		}
		if (_timeToUpdateListForce)
		{
			_connector.RefreshHostList(true);
			_timeToUpdateListForce = false;
		}
	}

	private void OnGUI()
	{
		if (Network.isClient || Network.isServer)
		{
			if (Application.CanStreamedLevelBeLoaded(Application.loadedLevel + 1))
			{
				GUI.Label(new Rect(Screen.width/2 - 70, Screen.width/2 - 6, 140f, 12f), "Starting the game!");
				Application.LoadLevel(Application.loadedLevel + 1);
			}
			else
			{
				GUI.Label(new Rect(Screen.width/2 - 70, Screen.width/2 - 6, 140f, 12f),
					"Loading the game: " + Mathf.Floor(Application.GetStreamProgressForLevel("Game")*100f) + " %");
			}
			return;
		}
		_menuHolder = GUILayout.Window(11111, _menuHolder, FillMainMenu, "Networking Prototype", new GUILayoutOption[0]);
	}

	private void FillMainMenu(int windowID)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Player Name:", new GUILayoutOption[0]);
			_playerName = GUILayout.TextField(_playerName, 25, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				PlayerPrefs.SetString("playerName", _playerName);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			if (_playerName == string.Empty)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Please enter a valid player name (No Special Characters!)", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				return;
			}

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (GUILayout.Button("Quickplay", new GUILayoutOption[0]))
			{
				Debug.Log("Quickplay");
				_quickPlay = true;
				_quickPlayStart = Time.time;
			}

			if (_quickPlay)
			{
				_connectStatus = _connector.QuickPlay(_quickPlayStart);
				GUILayout.Label(_connectStatus, new GUILayoutOption[0]);
				if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
				{
					Network.Disconnect();
					_quickPlay = false;
				}
				if (_connectStatus == "failed")
				{
					Debug.Log("PlayNow: No games hosted, so hosting one ourselves");
					_connector.StartHost(9, _playerName, _connector._serverPort);
				}
			}
			if (GUILayout.Button("Start Server WO", new GUILayoutOption[0]))
			{
				Debug.Log("Startserver");
				_connector.StartServer();
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			if (GUILayout.Button("Refresh Hostlist", new GUILayoutOption[0]))
			{
				Debug.Log("Refreshing Manual");
				_timeToUpdateListForce = true;
			}
			_timeToUpdateList = true;
			if (_connector._hostList != null)
			{
				scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[0]);
				HostData[] hostList = _connector._hostList;
				for (int i = 0; i < hostList.Length; i++)
				{
					HostData hostData = hostList[i];
					GUILayout.BeginHorizontal();
					GUILayout.Label(hostData.gameName, new GUILayoutOption[0]);
					//GUILayout.FlexibleSpace();
					GUILayout.Label(hostData.connectedPlayers + "/" + hostData.playerLimit, new GUILayoutOption[0]);
					//GUILayout.FlexibleSpace();
					if (!_quickPlay)
					{
						if (GUILayout.Button("Connect", new GUILayoutOption[0]))
						{
							_connector.Connect(hostData);
						}
					}
					else
					{
						if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
						{
							Network.Disconnect();
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Direct Connect", new GUILayoutOption[0]))
			{
				Debug.Log("Direct Connect");
				_connector.Connect(_ipAddress, _port);
			}
			GUILayout.BeginHorizontal();
			_ipAddress = GUILayout.TextField(_ipAddress, 25, new GUILayoutOption[0]);
			_portHolder = GUILayout.TextField(_portHolder, 25, new GUILayoutOption[0]);
			if (_portHolder != "")
			{
				_port = int.Parse(_portHolder);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
		}
		else
		{
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				Debug.Log("Disconnect");
				_quickPlay = false;
				Network.Disconnect();
			}
		}
	}
}