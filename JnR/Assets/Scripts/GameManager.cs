using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public List<PlayerObject> _playerList = new List<PlayerObject>();
	public int _playerListCount;
	public Transform _spawnablePlayerPrefab;
	//BUFF / DEBUFF SPELL PROTOTYPE
	public Dictionary<int,List<DynamicEffect>> _effectList;
	int size = 10;
	//Game is Running?
	public bool _isRunning;
	
	private void Start()
	{
		if(Network.isServer)
		{
			//Get Memory for the list.

			_effectList = new Dictionary<int, List<DynamicEffect>>(size);
			for(int i = 0 ; i < size; ++i)
			{
				_effectList.Add(i,new List<DynamicEffect>(50));
			}
		}
	}
	
	private void Update()
	{
		if(Network.isServer)
		{
			bool removeMe = false;
			for(int i = 0;i<size;++i)
			{
				foreach(DynamicEffect e in _effectList[i])
				{
					//Check if the spell should be removed
					removeMe = RemoveCheck(e);
					if(removeMe)
					{
						Debug.Log("Removed a Spell");
						_effectList[i].Remove(e);
					}
					Resolve(i,e,removeMe);
				}
			}
		}
	}	
	
	private bool RemoveCheck(DynamicEffect e)
	{
		float currentTime = Time.time;
		if(currentTime - e._startTime < 0)
		{
			//REMOVE THIS SPELL
			return true;
		}
		//DONT REMOVE THIS SPELL
		return false;
	}
	
	private void Resolve(int player,DynamicEffect e,bool wasRemoved)
	{
		float currentTime = Time.time;
		
		//Instant
		if(e._duration == 0)
		{
			if(!e._isTriggered)
			{
				Debug.Log("INSTANT ATTACK/HEAL OR SOMETHING..");
				e._isTriggered = true;
				e._lastTriggerTime = Time.time;
				return; //Nothing more should happen
			}
		}
		
		//Modifier Spells
		if(e._isModifier)
		{
			//Check if Spell is triggered or trigger it..
			if(!e._isTriggered)
			{
				Debug.Log ("APPLY SOME MODIFIER!");	
				e._isTriggered = true;
				e._lastTriggerTime = Time.time;
				return; //Nothing more should happen since we are a modifier and there is no modifier with duration equals 0
			}
			
			if(wasRemoved)
			{
				Debug.Log ("REMOVE SOME MODIFIER!");
				return; //Nothing more should happen since we just removed a modifier
			}
		}
		
		//Those spells are mostly croud control
		if(e._isStatusModifier) 
		{
			//Check if Spell is triggered or trigger it..
			if(!e._isTriggered)
			{
				Debug.Log ("APPLY SOME STATUS MODIFIER!");	
				e._isTriggered = true;
				e._lastTriggerTime = Time.time;
				return; //Nothing more should happen since we are a modifier and there is no modifier with duration equals 0
			}
			
			if(wasRemoved)
			{
				Debug.Log ("REMOVE SOME STATUS MODIFIER!");	
				return; //Nothing more should happen since we just removed a modifier
			}	
		}
	}
	
	private void AddEffectForPlayer(int player, DynamicEffect effect)
	{
		//Make a copy for the gc?
		_effectList[player].Add (effect);
	}
	
	private void AddEffectForPlayer(int player, DynamicEffect[] effect)
	{
		foreach(DynamicEffect e in effect)
		{
			_effectList[player].Add (e);	
		}
	}
	// END BUFF / DEBUFF SPELL PROTOTYPE
	
	
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
		//foreach (NetworkView nv in playerPrefab.GetComponentsInChildren<NetworkView>())
		//{
		//	nv.viewID = transformViewID;	
		//}
		//
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
			
			playerPrefab.GetComponent<Movement>().enabled = true;
			playerPrefab.GetComponent<Movement>()._isLocalPlayer = true;
			playerPrefab.GetComponent<MovementNetwork>().enabled = true;
			playerPrefab.GetComponent<MovementNetworkSync>().SendMessage("SetOwnership");
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
			playerPrefab.GetComponent<Movement>().enabled = true;
			playerPrefab.GetComponent<Movement>()._isLocalPlayer = false;
			playerPrefab.GetComponent<MovementNetwork>().enabled = false;

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
	
	private void RemoveHP(NetworkPlayer np)
	{
		GetPlayerObject(np)._playerPrefab.GetComponent<PlayerState>()._hp -= 10;
	}
	
	[RPC]
	private void RemoteAttack(NetworkPlayer source, NetworkPlayer target)
	{
		Debug.Log ("Player " + source + " attacks Player " + target + " for 10 DMG");
		RemoveHP (target);
		GetPlayerObject(target)._playerPrefab.networkView.RPC("SyncHealth",target,GetPlayerObject(target)._playerPrefab.GetComponent<PlayerState>()._hp);
	}
	
	[RPC]
	private void ResetPositionToSpawnpoint(NetworkPlayer source)
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
		GetPlayerObject(source)._playerPrefab.position = spawnPoint.position;		
	}
}
