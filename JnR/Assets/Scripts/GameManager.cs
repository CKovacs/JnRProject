using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool _isRunning;
    public List<PlayerObject> _playerList = new List<PlayerObject>();
    public int _playerListCount;
    public Transform _spawnablePlayerPrefab;
    private ServerSkillContainer _skillContainer;

    public DynamicEffectHandler _dynamicEffectHandler;

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
					AlterHealth(_playerList[i]._networkPlayer,-51);
                }
                GUILayout.Label("---", new GUILayoutOption[0]);
            }
        }
    }

    private void AlterHealth(NetworkPlayer np, int value)
    {
		Transform player = GetPlayerObject(np)._playerPrefab;
		//Change the Health on the Server Object according to the value
		player.GetComponent<PlayerState>()._hp += value;
		if(player.GetComponent<PlayerState>()._hp > PlayerStateSyncValues.MAXLIFE) 
		{
			player.GetComponent<PlayerState>()._hp = PlayerStateSyncValues.MAXLIFE;
		}
		if(player.GetComponent<PlayerState>()._hp <= 0)
		{
			Debug.Log ("A player died");
			//Replicate the Death to the player
			Death(np);
		}
		//Replicate the Health on the client peers
		 player.networkView.RPC("SyncValue", np, PlayerStateSyncValues.LIFE, player.GetComponent<PlayerState>()._hp);
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

	private void Death(NetworkPlayer player)
	{
		Debug.Log ("Player " + player + " died!");
		//Reset the lifepoints
		AlterHealth(player,PlayerStateSyncValues.MAXLIFE);
		networkView.RPC ("S_ResetPositionToSpawnpoint",RPCMode.Server,player);
	}

	[RPC]
	//Client
	private void C_Death(NetworkPlayer playerToBeDead)
	{
		//Respawn on a Spawnpoint
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
		Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
		GetPlayerObject(playerToBeDead)._playerPrefab.position = spawnPoint.position;
	}

    [RPC]
    //Server Only
    private void S_RemoteAttack(NetworkPlayer source, NetworkPlayer target)
    {
        Debug.Log("Player " + source + " attacks Player " + target + " for 10 DMG");
        AlterHealth(target,-10);
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
        foreach (var player in players.Where(player => player.name == name))
        {
            player.team = (SelectedTeam)team;
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

    private IEnumerable<PlayerMock> players;

    [RPC]
    private void AddPlayerToTeam(string name, int team)
    {
        Debug.Log("AddPlayerToTeam");
        foreach (var player in players.Where(player => player.name == name))
        {
            player.team = (SelectedTeam)team;

            networkView.RPC("SC_RemoteAddPlayerToTeam", RPCMode.All, player.name, (int)player.team);
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