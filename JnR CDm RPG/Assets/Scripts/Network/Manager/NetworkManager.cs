using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public Transform _playerPrefab;
	public LocalPlayer _localPlayer;
	private ArrayList _playerArray = new ArrayList();
	public string _masterServerGameName = "ComplexMasterServerGameName";
	private void Awake()
	{
		string @string = PlayerPrefs.GetString("playerName");
		Network.isMessageQueueRunning = true;
		if (Network.isServer)
		{
			MasterServer.RegisterHost(this._masterServerGameName, @string + "_Game");
		}
	}
	private void Start()
	{
	}
	private void Update()
	{
		if (Network.isClient && !this._localPlayer._isInstantiated && this._localPlayer._localPlayer.ToString() != "0")
		{
			base.networkView.RPC("SpawnPlayer", RPCMode.AllBuffered, new object[]
			{
				this._localPlayer._localPlayer,
				this._localPlayer._localTransformViewID
			});
			this._localPlayer._isInstantiated = true;
			base.enabled = false;
		}
	}
	private void OnPlayerConnected(NetworkPlayer player)
	{
		string text = string.Empty;
		if (Network.isServer)
		{
			text += "Server: ";
		}
		else
		{
			text += "Client: ";
		}
		text = text + "Sending player initialization to " + player;
		Debug.Log(text);
		NetworkViewID networkViewID = Network.AllocateViewID();
		Debug.Log("Given viewID = " + networkViewID);
		base.networkView.RPC("InitPlayer", player, new object[]
		{
			player,
			networkViewID
		});
	}
	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		string text = string.Empty;
		if (Network.isServer)
		{
			text += "Server: ";
		}
		else
		{
			text += "Client: ";
		}
		text = text + "Cleaning up Player - " + player;
		Debug.Log(text);
		PlayerInfo playerInfo = null;
		foreach (PlayerInfo playerInfo2 in this._playerArray)
		{
			if (player == playerInfo2._player)
			{
				Debug.Log("Destroying objects of View ID " + playerInfo2._viewID);
				Network.Destroy(playerInfo2._viewID);
				playerInfo = playerInfo2;
			}
		}
		if (playerInfo != null)
		{
			this._playerArray.Remove(playerInfo);
			Network.RemoveRPCs(player, 0);
			Network.DestroyPlayerObjects(player);
		}
	}
	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
	}
	[RPC]
	private void SpawnPlayer(NetworkPlayer playerIdentifier, NetworkViewID transformViewID)
	{
		Debug.Log("Spawning and instantiating the Player " + playerIdentifier);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform transform = array[UnityEngine.Random.Range(0, array.Length)].transform;
		Transform transform2 = (Transform)UnityEngine.Object.Instantiate(this._playerPrefab, transform.position, Quaternion.identity);
		NetworkView networkView = (NetworkView)transform2.GetComponent("NetworkView");
		networkView.viewID = transformViewID;
		if (playerIdentifier == this._localPlayer._localPlayer)
		{
			Debug.Log("Enabling Ownership");
			transform2.GetComponent<Movement>().enabled = true;
			transform2.GetComponent<Movement>()._isLocalPlayer = true;
			transform2.GetChild(0).camera.enabled = true;
			transform2.GetChild(0).camera.GetComponent<AudioListener>().enabled = true;
			transform2.GetComponentInChildren<SmoothFollow>().enabled = true;
			transform2.GetComponent<MovementNetwork>().enabled = true;
			transform2.SendMessage("SetOwnership", playerIdentifier);
			return;
		}
		if (Network.isServer)
		{
			transform2.GetChild(0).camera.enabled = false;
			transform2.GetComponentInChildren<SmoothFollow>().enabled = false;
			transform2.GetComponent<Movement>().enabled = true;
			transform2.GetComponent<Movement>()._isLocalPlayer = false;
			transform2.GetComponent<MovementNetwork>().enabled = false;
			PlayerInfo playerInfo = new PlayerInfo();
			playerInfo._player = playerIdentifier;
			playerInfo._viewID = transformViewID;
			this._playerArray.Add(playerInfo);
			Debug.Log("There are now " + this._playerArray.Count + " players active");
		}
	}
	
	//TODO eine getPlayer(NetworkViewID) Funktion
	//Die wichtige Informationen speichert..
}