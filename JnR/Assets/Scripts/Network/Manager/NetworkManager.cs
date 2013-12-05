using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	public string _masterServerGameName = "ComplexMasterServerGameName";
	public Transform _gameManagementObject;
	public Camera _serverGuiCameraDEBUG;
	
	private void Awake()
	{
		string playerName = PlayerPrefs.GetString ("playerName");
		Network.isMessageQueueRunning = true;
		if(Network.isServer)
		{
			MasterServer.RegisterHost(_masterServerGameName, playerName + "_Game");	
			_serverGuiCameraDEBUG.enabled = true;
		}
	}
	
	//Change this function to be a Co Routine!
	private void Update()
	{
		if (Network.isClient)
		{
			LocalPlayer lp = _gameManagementObject.GetComponent<LocalPlayer>();
			if(!lp._isInstantiated && lp._networkPlayer.ToString() != "0") 
			{
				//From Client to All Remote Clients and the Server
				GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
				Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
				Debug.Log ("Position is..." + spawnPoint.position.ToString());
				_gameManagementObject.networkView.RPC("SC_SpawnPlayer", RPCMode.AllBuffered,lp._networkPlayer,lp._networkViewID,spawnPoint.position);
				lp._isInstantiated = true;
				enabled = false;
			}
			return;
		}
		if(Network.isServer)
		{
			this.enabled = false;
			return;
		}
	}
	
	private void OnPlayerConnected(NetworkPlayer player)
	{
		if(Network.isServer)
		{
			Debug.Log ("A new player (" + player + ") has connected...");
			NetworkViewID networkViewID = Network.AllocateViewID();
			_gameManagementObject.networkView.RPC("C_InitiateLocalPlayer", player ,player ,networkViewID);
		}
	}
	
	//TODO! REMOVE FROM PLAYERLIST ON ALL PEERS!
	//-> Auslagern in einen RPC auf dem Game Manager "RemovePlayerSpawn"
	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		if(Network.isServer)
		{
			PlayerObject po = _gameManagementObject.GetComponent<GameManager>().GetPlayerObject(player);
			if(po != null)
			{
				Network.Destroy (po._networkViewID);
				Network.RemoveRPCs(player,0);
				Network.DestroyPlayerObjects(player);
				_gameManagementObject.GetComponent<GameManager>().RemovePlayerObject(po);
				Debug.Log("Destroyed " + player);
			}
		}	
	}
	
	//TO BE IMPLEMENTED!
	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		
	}
}