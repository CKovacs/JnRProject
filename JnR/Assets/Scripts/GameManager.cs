using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public DynamicEffectHandler _dynamicEffectHandler;
	public bool _isRunning;
	public List<PlayerObject> _playerList = new List<PlayerObject>();
	public int _playerListCount;
	private ServerSkillContainer _skillContainer;
	public Transform _spawnablePlayerPrefab;
	private IEnumerable<PlayerMock> players;
	public GameScore _gameScore;

	private void Start()
	{
		_skillContainer = GetComponent<ServerSkillContainer>();
		InitPlayerList();
		
		if (Network.isServer)
		{
			_dynamicEffectHandler = new DynamicEffectHandler();
		}
	}

	private void Update()
	{
		if (Network.isServer)
		{
			_dynamicEffectHandler.Update(Time.deltaTime);
		}
	}

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
		if (index < _playerList.Count && index >= 0)
		{
			return _playerList[index];
		}
		return null;
	}

	public PlayerObject GetPlayerObject(NetworkPlayer p)
	{
		foreach (PlayerObject po in _playerList)
		{
			if (p == po._networkPlayer)
			{
				return po;
			}
		}
		return null;
	}

	[RPC]
	//Server and Client
	private void SC_SpawnPlayer(NetworkPlayer playerIdentifier, NetworkViewID transformViewID, Vector3 spawnPosition)
	{
		if (Network.isServer)
		{
			Debug.Log("SpawningPlayer as Server");
		}
		else
		{
			Debug.Log("SpawningPlayer as Client");
		}
		var playerPrefab = Instantiate(_spawnablePlayerPrefab, spawnPosition, Quaternion.identity) as Transform;
		var networkView = playerPrefab.GetComponent<NetworkView>();
		networkView.viewID = transformViewID;
		var po = new PlayerObject();
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
			playerPrefab.GetComponent<MovementNetworkSync>().SendMessage("SetOwnership");
			playerPrefab.GetComponentInChildren<Camera>().enabled = true;
			playerPrefab.GetComponentInChildren<AudioListener>().enabled = true;
			playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = true;
			playerPrefab.GetComponent<InputDispatcher>().enabled = true;
			playerPrefab.GetComponent<InputDispatcher>()._gameManagementObject = transform;
			playerPrefab.GetComponent<AnimationHandle>()._gameManagementObject = transform;
			playerPrefab.GetComponent<AnimationHandle>()._localPlayer = true;
			playerPrefab.GetComponent<PlayerState>()._gameManagementObject = transform;
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
			playerPrefab.GetComponent<InputDispatcher>()._gameManagementObject = transform;
			playerPrefab.GetComponent<AnimationHandle>()._gameManagementObject = transform;
			playerPrefab.GetComponent<PlayerState>()._gameManagementObject = transform;
			playerPrefab.GetComponent<AnimationHandle>()._localPlayer = false;
		}
	}

	private void OnGUI()
	{
		if (Network.isServer)
		{
			for (int i = 0; i < _playerList.Count; ++i)
			{
				GUILayout.Label("NetworkPlayer:" + _playerList[i]._networkPlayer, new GUILayoutOption[0]);
				GUILayout.Label("NetworkViewID:" + _playerList[i]._networkViewID, new GUILayoutOption[0]);
				GUILayout.Label("HP = " + _playerList[i]._playerPrefab.GetComponent<PlayerState>()._hp);
				if (GUILayout.Button("Hurt", new GUILayoutOption[0]))
				{
					AlterHealth(_playerList[i]._networkPlayer, -51);
				}
				GUILayout.Label("---", new GUILayoutOption[0]);
			}
		}
		else
		{
			//Wer hat die Flagge?
			//Wie ist der Punktestand?
		}
	}

	/*! A server-side function to change the health of a player.
	 * 
	 */

	private void AlterHealth(NetworkPlayer np, int value)
	{
		if (Network.isServer)
		{
			Transform player = GetPlayerObject(np)._playerPrefab;
			//Change the Health on the Server Object according to the value
			player.GetComponent<PlayerState>()._hp += value;
			if (player.GetComponent<PlayerState>()._hp > PlayerStateSyncValues.MAXLIFE)
			{
				player.GetComponent<PlayerState>()._hp = PlayerStateSyncValues.MAXLIFE;
			}
			if (player.GetComponent<PlayerState>()._hp <= 0)
			{
				Debug.Log("A player died...");
				//Replicate the Death to the player
				Death(np);
			}
			networkView.RPC("SyncValuesForPlayer", RPCMode.Others, np, PlayerStateSyncValues.LIFE,
				player.GetComponent<PlayerState>()._hp);
			//Replicate the Health on the client peers
			//player.networkView.RPC("SyncValue", RPCMode.Others, np, PlayerStateSyncValues.LIFE, player.GetComponent<PlayerState>()._hp);
		}
		else
		{
			Debug.LogError("Client " + Network.player + " tries to access a server function.");
		}
	}

	/*! This networkfunction syncronizes the attributes of a certain player to all peers
	 */

	[RPC]
	private void SyncValuesForPlayer(NetworkPlayer player, int id, int value)
	{
		GetPlayerObject(player)._playerPrefab.GetComponent<PlayerState>().SyncValue(id, value);
	}

	/*! A server-side function to resolve a players death
	 */

	private void Death(NetworkPlayer player)
	{
		if (Network.isServer)
		{
			Transform playerPrefab = GetPlayerObject(player)._playerPrefab;
			var playerState = playerPrefab.GetComponent<PlayerState>();
			Debug.Log("Player " + player + " died!");
			/*A player has died we need to do the following
			 * * Trigger a respawn (this can be a coroutine)
			 * * check if the dead player had a flag attached 
			 * * * flag handling
			 */

			
			playerState._isDead = true;
			playerPrefab.GetComponent<AnimationHandle>().Death(true);
			networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, PlayerStateSyncValues.BOOLDEATH, playerState._isDead ? 1 : 0);
			//PlayerReEnabling(np,false);
			playerPrefab.GetComponent<InputDispatcher>().enabled = false;
			AlterHealth(player, PlayerStateSyncValues.MAXLIFE);
			//networkView.RPC ("S_ResetPositionToSpawnpoint",RPCMode.Server,player);

			if (playerState._isHoldingAFlag)
			{
				Team t = playerState._team;
				if(t == Team.Blue)
				{
					//Do something with RED flag
					networkView.RPC ("DropFlag",RPCMode.All,0);
				} 
				else
				{
					//Do something with BLUE flag
					networkView.RPC ("DropFlag",RPCMode.All,1);
				}

				playerState._isHoldingAFlag = false;
				networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, PlayerStateSyncValues.BOOLFLAG, playerState._isHoldingAFlag ? 1 : 0);
			}
			StartCoroutine(Respawn(player));
		}
		else
		{
			Debug.LogError("Client " + Network.player + " tries to access a server function.");
		}
	}

	private IEnumerator Respawn(NetworkPlayer player)
	{
		networkView.RPC("PlayerReEnabling", RPCMode.Others, player, false ? 1 : 0);
		yield return new WaitForSeconds(2);
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
		Transform playerPrefab = GetPlayerObject(player)._playerPrefab;
		networkView.RPC("SetRespawnPosition", RPCMode.Others, player, spawnPoint.position);
		GetPlayerObject(player)._playerPrefab.position = spawnPoint.position;
		playerPrefab.GetComponent<MovementNetworkSync>().ResetState();
		yield return new WaitForSeconds(3);
		networkView.RPC("PlayerReEnabling", RPCMode.Others, player, true ? 1 : 0);
	}

	[RPC]
	/*! Disables and enables a set of components on a specific player and the corresponding peer
	 */
	private void PlayerReEnabling(NetworkPlayer player, int tralse)
	{
		if (player == GetComponent<LocalPlayer>()._networkPlayer)
		{
			Transform playerPrefab = GetPlayerObject(player)._playerPrefab;
			playerPrefab.GetComponent<InputDispatcher>().enabled = tralse == 0 ? false : true;
			playerPrefab.GetComponent<MovementNetworkSync>().enabled = tralse == 0 ? false : true;
			playerPrefab.GetComponent<Movement>().enabled = tralse == 0 ? false : true;
			playerPrefab.GetComponentInChildren<SmoothFollow>().enabled = tralse == 0 ? false : true;
		}
	}

	[RPC]
	private void DropFlag(int flagId)
	{
		if(flagId==0)
		{
		//	Debug.Log ("Dropped a flag... " + _flagRed.name);
		}
		else
		{
		//	Debug.Log ("Dropped a flag... " + _flagBlue.name);
		}
	}

	[RPC]
	private void SetRespawnPosition(NetworkPlayer player, Vector3 newPosition)
	{
		GetPlayerObject(player)._playerPrefab.position = newPosition;
	}

	[RPC]
	//Server
	private void S_SendAnimation(NetworkPlayer source, string animation)
	{
		//Send animation to other clients
		networkView.RPC("C_SendAnimation", RPCMode.Others, source, animation);
	}

	[RPC]
	//Client
	private void C_SendAnimation(NetworkPlayer source, string animation)
	{
		if (source == GetComponent<LocalPlayer>()._networkPlayer)
		{
			return;
		}
		Debug.Log("I Got It");
		GetPlayerObject(source)._playerPrefab.GetComponent<AnimationHandle>().NetworkAnmiation(animation);
	}

	[RPC]
	//Server Only
	private void S_RemoteAttack(NetworkPlayer source, NetworkPlayer target)
	{
		Debug.Log("Player " + source + " attacks Player " + target + " for 10 DMG");
		AlterHealth(target, -10);
		GetPlayerObject(target)
			._playerPrefab.networkView.RPC("SyncHealth", target,
				GetPlayerObject(target)._playerPrefab.GetComponent<PlayerState>()._hp);
	}

	[RPC]
	//Server Only
	private void S_RemoteSkillUse(NetworkPlayer source, NetworkPlayer target, int skillId)
	{
		Debug.Log("Player " + source.guid + " tries to casts " + skillId + " on " + target.guid);

		//Validate if spell is possible
		//Call RemoteSkillUseOnClient
		////// Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);

		networkView.RPC("SC_UseSkill", RPCMode.All, source, target, skillId);

		AlterHealth(target, -5);

		// Skill effect
		Skill skill = _skillContainer.GetSkill(skillId);

		if (skill._3dEffectType != Effect3DType.Projectile)
		{
			_dynamicEffectHandler.AddEffectsForPlayer(target, skill._effect);
		}
	}

	[RPC]
	//Server and Client
	private void SC_UseSkill(NetworkPlayer source, NetworkPlayer target, int skillId)
	{
		Debug.Log("Player " + source.guid + " casts " + skillId + " on " + target.guid);
		Skill skill = _skillContainer.GetSkill(skillId);

		// Graphical stuff
		if (skill._3dEffect)
		{
			Effect3DBuilder.DoEffect(GetPlayerObject(source)._playerPrefab.transform,
				GetPlayerObject(target)._playerPrefab.transform, skill);
		}
	}

	[RPC]
	//Server
	private void S_DoSkillEffect(NetworkPlayer source, NetworkPlayer target, int skillId)
	{
		Skill skill = _skillContainer.GetSkill(skillId);

		// Effect list
	}

	[RPC]
	private void SC_RemoteAddPlayerToTeam(string name, int team)
	{
		foreach (PlayerMock player in players.Where(player => player.name == name))
		{
			player.team = (SelectedTeam) team;
		}
	}

	[RPC]
	//Server only
	private void S_ResetPositionToSpawnpoint(NetworkPlayer source)
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
		GetPlayerObject(source)._playerPrefab.position = spawnPoint.position;
	}

	[RPC]
	private void AddPlayerToTeam(string name, int team)
	{
		Debug.Log("AddPlayerToTeam");
		foreach (PlayerMock player in players.Where(player => player.name == name))
		{
			player.team = (SelectedTeam) team;

			networkView.RPC("SC_RemoteAddPlayerToTeam", RPCMode.All, player.name, (int) player.team);
		}
	}


	public IEnumerable<PlayerMock> GetConnectedPlayers()
	{
		return players;
	}

	private void InitPlayerList()
	{
		players = new List<PlayerMock>
		{
			new PlayerMock
			{
				name = "Herbert",
				team = SelectedTeam.None
			},
			new PlayerMock
			{
				name = "Player2",
				team = SelectedTeam.Red
			},
			new PlayerMock
			{
				name = "Player3",
				team = SelectedTeam.None
			},
			new PlayerMock
			{
				name = "Player4",
				team = SelectedTeam.Red
			},
			new PlayerMock
			{
				name = "Player5",
				team = SelectedTeam.Blue
			},
			new PlayerMock
			{
				name = "Player6",
				team = SelectedTeam.None
			}
		};
	}
}


public class PlayerMock
{
	public string name;
	public SelectedTeam team;
}

public enum SelectedTeam
{
	Red,
	Blue,
	None
}