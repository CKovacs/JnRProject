using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public List<PlayerObject> _playerList = new List<PlayerObject>();
	public int _playerListCount;
	public Transform _spawnablePlayerPrefab;
	
	public void AddPlayerObject(PlayerObject po)
	{
		_playerList.Add(po);
		_playerListCount++;
	}
	
	public void RemovePlayerObject(PlayerObject po)
	{
		_playerList.Remove(po);
		_playerListCount--;
	}
	
	public PlayerObject GetPlayerObjectAt(int index)
	{
		if(index < _playerList.Count && index >= 0)
		{
			return _playerList[index];
		}
		return null;
	}
	
	public PlayerObject GetPlayerObject(NetworkPlayer p)
	{
		foreach(PlayerObject po in _playerList)
		{
			if (p == po._networkPlayer)
			{
				return po;
			}
		}
		return null;
	}
	
	[RPC]
	private void SpawnPlayer(NetworkPlayer playerIdentifier, NetworkViewID transformViewID, Vector3 spawnPosition)
	{
		if(Network.isServer) 
		{
			Debug.Log("SpawningPlayer as Server");
		}
		else
		{
			Debug.Log("SpawningPlayer as Client");
		}
		Transform playerPrefab = Instantiate(_spawnablePlayerPrefab, spawnPosition, Quaternion.identity) as Transform;
		NetworkView networkView = playerPrefab.GetComponent<NetworkView>();
		networkView.viewID = transformViewID;
		foreach (NetworkView nv in playerPrefab.GetComponentsInChildren<NetworkView>())
		{
			nv.viewID = transformViewID;	
		}
		
		PlayerObject po = new PlayerObject();
		po._networkPlayer = playerIdentifier;
		po._networkViewID = transformViewID;
		po._playerPrefab = playerPrefab;
		AddPlayerObject(po);
	
		if (playerIdentifier == GetComponent<LocalPlayer>()._networkPlayer)
		{
			////////////////////
			{
				string text = string.Empty;
				if (Network.isServer)
				{
					text += "S: ";
				}
				else
				{
					text += "C: ";
				}
				text = text + "Spawning the local player!";
				Debug.Log(text);
			}
			////////////////////
			//GameManager Local Player (only on Client)
			GetComponent<LocalPlayer>()._playerPrefab = playerPrefab;
			
			playerPrefab.GetComponentInChildren<Movement>().enabled = true;
			playerPrefab.GetComponentInChildren<Movement>()._isLocalPlayer = true;
			playerPrefab.GetComponentInChildren<MovementNetwork>().enabled = true;
			playerPrefab.GetComponentInChildren<MovementNetworkSync>().SendMessage("SetOwnership");
			playerPrefab.GetComponentInChildren<Camera>().enabled = true;
			playerPrefab.GetComponentInChildren<AudioListener>().enabled = true;
			playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = true;
			playerPrefab.GetComponentInChildren<InputDispatcher>().enabled = true;
			playerPrefab.GetComponentInChildren<InputDispatcher>()._gameManagementObject = this.transform;
			
			//playerPrefab.GetComponentInChildren<Movement>().enabled = true;
			//playerPrefab.GetComponentInChildren<Movement>()._isLocalPlayer = true;
			//playerPrefab.GetComponentInChildren<MovementNetwork>().enabled = true;
			//playerPrefab.GetComponentInChildren<MovementNetworkSync_TSP>().SendMessage("SetOwnership");
			//playerPrefab.GetComponentInChildren<Camera>().enabled = true;
			//playerPrefab.GetComponentInChildren<AudioListener>().enabled = true;
			//playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = true;
			//playerPrefab.GetComponentInChildren<InputDispatcher>().enabled = true;
			//playerPrefab.GetComponentInChildren<InputDispatcher>()._gameManagementObject = this.transform;
			return;
		}
		if (Network.isServer)
		{
			////////////////////
			{
				string text = string.Empty;
				if (Network.isServer)
				{
					text += "S: ";
				}
				else
				{
					text += "C: ";
				}
				text = text + "Spawning a remote player!";
				Debug.Log(text);
			}
			////////////////////
			playerPrefab.GetComponentInChildren<Camera>().enabled = false;
			playerPrefab.GetComponentInChildren<AudioListener>().enabled = false;
			playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = false;
			playerPrefab.GetComponentInChildren<Movement>().enabled = true;
			playerPrefab.GetComponentInChildren<Movement>()._isLocalPlayer = false;
			playerPrefab.GetComponentInChildren<MovementNetwork>().enabled = false;

			//playerPrefab.GetComponentInChildren<Camera>().enabled = false;
			//playerPrefab.GetComponentInChildren<AudioListener>().enabled = false;
			//playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = false;
			//playerPrefab.GetComponentInChildren<Movement>().enabled = true;
			//playerPrefab.GetComponentInChildren<Movement>()._isLocalPlayer = false;
			//playerPrefab.GetComponentInChildren<MovementNetwork>().enabled = false;
		}
	}
	
	private void OnGUI()
	{
		if(Network.isServer)
		{
			for(int i = 0;i<_playerList.Count;++i)
			{
				GUILayout.Label("NetworkPlayer:" + _playerList[i]._networkPlayer, new GUILayoutOption[0]);
				GUILayout.Label("NetworkViewID:" + _playerList[i]._networkViewID, new GUILayoutOption[0]);
				GUILayout.Label("HP = " + _playerList[i]._playerPrefab.GetComponent<PlayerState>()._hp);
				if (GUILayout.Button ("Hurt", new GUILayoutOption[0])) {
					RemoveHP (i);
				}
				GUILayout.Label("---", new GUILayoutOption[0]);
			}
		}
	}
	
	private void RemoveHP(int i)
	{
		GetPlayerObjectAt(i)._playerPrefab.GetComponent<PlayerState>()._hp -= 10;
	}
	
	[RPC]
	private void RemoteAttack(NetworkPlayer source, int target)
	{
		Debug.Log ("Player " + source + " attacks Player " + target + " for 10 DMG");
		RemoveHP (target);
	}
	
	[RPC]
	private void ResetPositionToSpawnpoint(NetworkPlayer source)
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
		GetPlayerObject(source)._playerPrefab.position = spawnPoint.position;		
	}
}
