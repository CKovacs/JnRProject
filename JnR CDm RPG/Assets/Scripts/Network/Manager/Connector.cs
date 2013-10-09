using UnityEngine;
using System.Collections;

public class Connector : MonoBehaviour {
	public class QuickPlayStatus
	{
		public const int ERROR = -1;
		public const int NOHOSTFOUND = 1;
		public const int HOSTFOUND = 2;
		public const int TRYINGTOCONNECT = 3;
	}
	////////
	//Relevante Daten um eine Verbindung mit dem MasterServer aufzubauen
	////////
	//Im Produktivsystem muss der MasterServer von uns gestellt werden daher
	//muessen folgende Codezeilen gesetzt werden
	//MasterServer.ipAddress = "127.0.0.1";
	//MasterServer.port = 112233;
	
	public string _masterServerGameName = "ComplexMasterServerGameName";
	public HostData[] _hostList;
	public int _serverPort = 23482;
	public float CONNECT_TIMEOUT = 1000f;
	private bool _tryingToConnectPlayNow;
	public int _tryingToConnectPlayNowNumber;
	private float _lastHostListRequest;
	private bool _currentlyWorkingOnRequest;
	private bool _refreshed = false;
	private float _lastPlayNowConnectionTime;
	
	public void RefreshHostList(bool manual)
	{
		if( _refreshed == false || (_lastHostListRequest - Time.realtimeSinceStartup ) > 120f || manual)
		{
			StartCoroutine(Refreshing(manual));
		}
	}

	private IEnumerator Refreshing(bool manual)
	{
		if (!_currentlyWorkingOnRequest){  // do nothing if GetList already running
	        _currentlyWorkingOnRequest = true; // signal "I'm working"
	       
	        MasterServer.ClearHostList();
	        MasterServer.RequestHostList(_masterServerGameName);
	        Debug.Log("Refresh metod");
	        yield return new WaitForSeconds(3);
	        _currentlyWorkingOnRequest = false;
		}
	}
	
	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			this._hostList = MasterServer.PollHostList();
			this._lastHostListRequest = Time.realtimeSinceStartup;
			this._refreshed = true;
			Debug.Log("Refreshed");
			Debug.Log(this._hostList.Length + " host objects found.");
		}
	}
	
	public void Connect()
	{
		Network.Connect("127.0.0.1", 25000);
	}
	
	public void Connect(HostData hd)
	{
		Debug.Log("Connecting to " + hd.ip + ":" + hd.port);
		Network.Connect(hd);
	}
	
	public string QuickPlay(float timeStarted)
	{
		int num = 0;
		if (this._hostList == null)
		{
			return "failed";
		}
		HostData[] hostList = this._hostList;
		for (int i = 0; i < hostList.Length; i++)
		{
			HostData hostData = hostList[i];
			if (hostData.connectedPlayers < hostData.playerLimit)
			{
				if (this._tryingToConnectPlayNow)
				{
					if (this._lastPlayNowConnectionTime + this.CONNECT_TIMEOUT <= Time.time)
					{
						Debug.Log("Interrupted by timer");
						this.FailedConnRetry(NetworkConnectionError.ConnectionFailed);
					}
					return "Trying to connect...";
				}
				if (!this._tryingToConnectPlayNow && this._tryingToConnectPlayNowNumber <= num)
				{
					this._tryingToConnectPlayNow = true;
					this._tryingToConnectPlayNowNumber = num;
					int port = hostData.port;
					Debug.Log("connecting to " + hostData.gameName + " " + hostData.ip + ":" + port);
					Network.Connect(hostData.ip, port);
					this._lastPlayNowConnectionTime = Time.time;
				}
			}
			num++;
		}
		if (Time.time < timeStarted + 7f)
		{
			this.RefreshHostList(true);
			return "Waiting for masterserver..." + Mathf.Ceil(timeStarted + 7f - Time.time);
		}
		if (!this._tryingToConnectPlayNow)
		{
			return "failed";
		}
		return string.Empty;
	}
	
	private void FailedConnRetry(NetworkConnectionError info)
	{
		if (info == NetworkConnectionError.InvalidPassword)
		{
		}
		if (this._tryingToConnectPlayNow)
		{
			this._tryingToConnectPlayNowNumber++;
			this._tryingToConnectPlayNow = false;
		}
	}
	
	public void StartHost(int maxPlayers, string playerName, int port)
	{
		if (maxPlayers <= 1)
		{
			maxPlayers = 1;
		}
		Network.InitializeSecurity();
		Network.InitializeServer(maxPlayers, port, !Network.HavePublicAddress());
	}
	
	public void StartServer()
	{
		Network.InitializeServer(10, 25000, false);
	}
	
	private void OnConnectedToServer()
	{
		Network.isMessageQueueRunning = false;
		PlayerPrefs.SetString("connectIP", Network.connections[0].ipAddress);
		PlayerPrefs.SetInt("connectPort", Network.connections[0].port);
	}
}