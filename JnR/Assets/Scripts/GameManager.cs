using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CombatHandler _combatHandler;
    public bool _isRunning;
    public List<PlayerObject> _playerList = new List<PlayerObject>();
    public int _playerListCount;
    public Transform _spawnablePlayerPrefab;
    public GameScore _gameScore;

    private ServerSkillContainer _skillContainer;
    private IEnumerable<PlayerMock> players;

    private void Start()
    {
        _skillContainer = GetComponent<ServerSkillContainer>();
        InitPlayerList();
        _combatHandler = new CombatHandler(this.networkView);
    }

    private void Update()
    {
        if (Network.isServer)
        {
            _combatHandler.Update(Time.deltaTime);
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
            return _playerList [index];
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
        } else
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
        po._playerPrefab.GetComponent<PlayerState>()._networkPlayer = playerIdentifier;
        AddPlayerObject(po);

        if (playerIdentifier == GetComponent<LocalPlayer>()._networkPlayer)
        {
            ////////////////////
            {
                var text = string.Empty;
                if (Network.isServer)
                {
                    text += "S: ";
                } else
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
                } else
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

            // Add to combat system
            _combatHandler.AddPlayer(po);
        }
    }

    //Debug GUI for Client and Server
    private void OnGUI()
    {
        if (Network.isServer)
        {
            for (int i = 0; i < _playerList.Count; ++i)
            {
                GUILayout.Label("NetworkPlayer:" + _playerList [i]._networkPlayer, new GUILayoutOption[0]);
                GUILayout.Label("NetworkViewID:" + _playerList [i]._networkViewID, new GUILayoutOption[0]);
                GUILayout.Label("HP = " + _playerList [i]._playerPrefab.GetComponent<PlayerState>()._hp);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Hurt(51)", new GUILayoutOption[0]))
                {
                    AlterHealth(_playerList [i]._networkPlayer, -51);
                }
                if (GUILayout.Button("Team Blue", new GUILayoutOption[0]))
                {
                    _playerList [i]._playerPrefab.GetComponent<PlayerState>()._team = Team.Blue;
                    networkView.RPC("SyncValuesForPlayer", RPCMode.OthersBuffered, _playerList [i]._networkPlayer, 
                                    CombatSyncValues.TEAM,
                                    0);
                }
                if (GUILayout.Button("Team Red", new GUILayoutOption[0]))
                {
                    _playerList [i]._playerPrefab.GetComponent<PlayerState>()._team = Team.Red;
                    networkView.RPC("SyncValuesForPlayer", RPCMode.OthersBuffered, _playerList [i]._networkPlayer, 
                                    CombatSyncValues.TEAM,
                                    1);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("---", new GUILayoutOption[0]);
            }
        } else
        {
            LocalPlayer lp = GetComponent<LocalPlayer>();
            GUILayout.Label("NetworkPlayer:" + lp._networkPlayer, new GUILayoutOption[0]);
            GUILayout.Label("NetworkViewID:" + lp._networkPlayer, new GUILayoutOption[0]);
            GUILayout.Label("Healthpoints:"  + lp._playerPrefab.GetComponent<PlayerState>()._hp, new GUILayoutOption[0]);
            if(Team.Blue == lp._playerPrefab.GetComponent<PlayerState>()._team)
            {
                GUILayout.Label("Team: Blue");
            }
            else
            {
                GUILayout.Label("Team: Red");
            }
            GUILayout.Label("Redpoints:"+_gameScore._flagsCapturedTeamRed, new GUILayoutOption[0]);
            GUILayout.Label("Bluepoints:"+_gameScore._flagsCapturedTeamBlue, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal();
            GUILayout.Label("RED FLAG CARRIED BY ", new GUILayoutOption[0]);
            if(_gameScore._playerHoldingFlagRed != null)
            {
                GUILayout.Label("PLAYER " + _gameScore._playerHoldingFlagRed.GetComponent<PlayerState>()._networkPlayer, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("BLUE FLAG CARRIED BY ", new GUILayoutOption[0]);
            if(_gameScore._playerHoldingFlagBlue != null)
            {
                GUILayout.Label("PLAYER " + _gameScore._playerHoldingFlagBlue.GetComponent<PlayerState>()._networkPlayer, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
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
            if (player.GetComponent<PlayerState>()._hp > CombatSyncValues.MAXLIFE)
            {
                player.GetComponent<PlayerState>()._hp = CombatSyncValues.MAXLIFE;
            }
            if (player.GetComponent<PlayerState>()._hp <= 0)
            {
                Debug.Log("A player died...");
                //Replicate the Death to the player
                Death(np);
            }
            networkView.RPC("SyncValuesForPlayer", RPCMode.Others, np, CombatSyncValues.LIFE,
                player.GetComponent<PlayerState>()._hp);
        } else
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
            PlayerState playerState = playerPrefab.GetComponent<PlayerState>();
            Debug.Log("Player " + player + " died!");
            /*A player has died we need to do the following
             * * Trigger a respawn (this can be a coroutine)
             * * check if the dead player had a flag attached 
             * * * flag handling
             */

            
            playerState._isDead = true;
            playerPrefab.GetComponent<AnimationHandle>().Death(true);
            networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, CombatSyncValues.BOOLDEATH, playerState._isDead ? 1 : 0);
            //PlayerReEnabling(np,false);
            playerPrefab.GetComponent<InputDispatcher>().enabled = false;
            
            //networkView.RPC ("S_ResetPositionToSpawnpoint",RPCMode.Server,player);
            if (playerState._isHoldingAFlag)
            {
                if (playerState._team == Team.Blue)
                {
                    //Do something with RED flag
                    networkView.RPC("DropFlag", RPCMode.All, FlagDescription.FLAGRED);
                    networkView.RPC("FlagCarriedByPlayer",RPCMode.All,null,FlagDescription.FLAGRED);
                } else
                {
                    //Do something with BLUE flag
                    networkView.RPC("DropFlag", RPCMode.All, FlagDescription.FLAGBLUE);
                    networkView.RPC("FlagCarriedByPlayer",RPCMode.All,null,FlagDescription.FLAGBLUE);
                }

                playerState._isHoldingAFlag = false;
                networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, CombatSyncValues.BOOLFLAG, playerState._isHoldingAFlag ? 1 : 0);
            }
            StartCoroutine(Respawn(player));
        } else
        {
            Debug.LogError("Client " + Network.player + " tries to access a server function.");
        }
    }

    private IEnumerator Respawn(NetworkPlayer player)
    {
        networkView.RPC("PlayerReEnabling", RPCMode.Others, player, false ? 1 : 0);
        yield return new WaitForSeconds(2);
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        Transform spawnPoint = spawnPoints [Random.Range(0, spawnPoints.Length)].transform;
        Transform playerPrefab = GetPlayerObject(player)._playerPrefab;
        AlterHealth(player, CombatSyncValues.MAXLIFE);
        networkView.RPC("SetRespawnPosition", RPCMode.Others, player, spawnPoint.position);
        GetPlayerObject(player)._playerPrefab.position = spawnPoint.position;
        playerPrefab.GetComponent<MovementNetworkSync>().ResetState();
        yield return new WaitForSeconds(3);
        networkView.RPC("PlayerReEnabling", RPCMode.Others, player, true ? 1 : 0);
        PlayerState playerState = playerPrefab.GetComponent<PlayerState>();
        playerState._isDead = false;
        networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, CombatSyncValues.BOOLDEATH, playerState._isDead ? 1 : 0);

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
        if (flagId == FlagDescription.FLAGBLUE)
        {
            _gameScore._flagBlue.GetComponent<FlagHandling>().DropFlag(flagId);
        } else
        {
            _gameScore._flagRed.GetComponent<FlagHandling>().DropFlag(flagId);
        }
    }

    [RPC]
    private void ResetFlag(int flagId)
    {
        if (flagId == FlagDescription.FLAGBLUE)
        {
            _gameScore._flagBlue.GetComponent<FlagHandling>().ResetFlag();
        } else
        {
            _gameScore._flagRed.GetComponent<FlagHandling>().ResetFlag();
        }
    }

    [RPC]
    private void FlagPickUp(NetworkPlayer player, int flagId)
    {
        if (flagId == FlagDescription.FLAGBLUE)
        {
            _gameScore._flagBlue.GetComponent<FlagHandling>().FlagPickUp(player);
        } else
        {
            _gameScore._flagRed.GetComponent<FlagHandling>().FlagPickUp(player);
        }
    }
    
    [RPC]
    private void RemoveFlagCarriedByPlayer(int flagId)
    {
        if(flagId == FlagDescription.FLAGBLUE)
        {
            _gameScore._playerHoldingFlagBlue = null;
        }
        else
        {
            _gameScore._playerHoldingFlagRed = null;
        }
    }
    
    [RPC]
    private void FlagCarriedByPlayer(NetworkPlayer player,int flagId)
    {
        if(flagId == FlagDescription.FLAGBLUE)
        {
            if(player == null)
            {
                _gameScore._playerHoldingFlagBlue = null;
            } 
            else
            {
                _gameScore._playerHoldingFlagBlue = GetPlayerObject(player)._playerPrefab;
            }
        }
        else
        {
            if(player == null)
            {
                _gameScore._playerHoldingFlagRed = null;
            } 
            else
            {
                _gameScore._playerHoldingFlagRed = GetPlayerObject(player)._playerPrefab;
            }
        }
    }
    
    [RPC]
    private void AddPointForTeam(int teamcolor)
    {
        if(teamcolor == FlagDescription.FLAGBLUE)
        {
            _gameScore._flagsCapturedTeamBlue++;
        }
        else
        {
            _gameScore._flagsCapturedTeamRed++;
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

    /* Server skill validate
    !!!! here plz
     */

    [RPC]
    //Server Only
    private void S_RemoteSkillUse(NetworkPlayer source, NetworkPlayer target, int skillId)
    {
        Debug.Log("Player " + source.guid + " tries to casts " + skillId + " on " + target.guid);

        networkView.RPC("SC_UseSkill", RPCMode.All, source, target, skillId);

        // Skill effect
        Skill skill = _skillContainer.GetSkill(skillId);

        if (skill._3dEffectType != Effect3DType.Projectile)
        {
            _combatHandler.AddEffectsForPlayer(GetPlayerObject(source), GetPlayerObject(target), skill._effect);
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
    private void S_ApplyProjectileEffect(NetworkPlayer source, NetworkPlayer target, int skillId)
    {
        Skill skill = _skillContainer.GetSkill(skillId);

        // Effect list
    }

    [RPC]
    //Server Client
    private void SC_DoEffect(NetworkPlayer player, int type, int amount, int percentage)
    {
        PlayerObject playerObject = GetPlayerObject(player);
        EffectType effectType = (EffectType)type;

        Debug.Log("TYPE: " + effectType);

        switch (effectType)
        {
            case EffectType.life:
                {
                    PlayerState playerState = playerObject._playerPrefab.GetComponent<PlayerState>();

                    playerState._hp += amount;

                    break;
                }
            case EffectType.run:
                {
                    Movement movement = playerObject._playerPrefab.GetComponent<Movement>();

                    movement._movementEditPercentage += percentage;
                    Debug.Log("New running percentage: " + percentage);
                    break;
                }
        // Stun counter needed, because you need to keep track how many stuns are on the target
            case EffectType.stun:
                {
                    PlayerState playerState = playerObject._playerPrefab.GetComponent<PlayerState>();

                    playerState._stunCounter++;

                    if (playerState._stunCounter == 1)
                    {
                        Movement movement = playerObject._playerPrefab.GetComponent<Movement>();

                        movement._movementEditPercentage -= 100;

                        if (playerObject._networkPlayer == GetComponent<LocalPlayer>()._networkPlayer)
                        {
                            InputDispatcher inputDispatcher = playerObject._playerPrefab.GetComponent<InputDispatcher>();

                            inputDispatcher.enabled = false;
                        }
                    }

                    break;
                }
            case EffectType.def:
                {
                    PlayerState playerState = playerObject._playerPrefab.GetComponent<PlayerState>();

                    playerState._forwardBlock += percentage;

                    break;
                }
        }
    }

    [RPC]
    //Server Client
    private void SC_UndoEffect(NetworkPlayer player, int type, int amount, int percentage)
    {
        PlayerObject playerObject = GetPlayerObject(player);
        EffectType effectType = (EffectType)type;

        switch (effectType)
        {
            case EffectType.life:
                {
                    // No work needed at the moment

                    break;
                }
            case EffectType.run:
                {
                    Movement movement = playerObject._playerPrefab.GetComponent<Movement>();

                    movement._movementEditPercentage -= percentage;
                    //Debug.Log("New running percentage: " + percentage);
                    break;
                }
        // Stun counter needed, because you need to keep track how many stuns are on the target
            case EffectType.stun:
                {
                    PlayerState playerState = playerObject._playerPrefab.GetComponent<PlayerState>();

                    playerState._stunCounter--;

                    if (playerState._stunCounter == 0)
                    {
                        Movement movement = playerObject._playerPrefab.GetComponent<Movement>();

                        movement._movementEditPercentage += 100;

                        if (playerObject._networkPlayer == GetComponent<LocalPlayer>()._networkPlayer)
                        {
                            InputDispatcher inputDispatcher = playerObject._playerPrefab.GetComponent<InputDispatcher>();

                            inputDispatcher.enabled = true;
                        }
                    }

                    break;
                }
            case EffectType.def:
                {
                    PlayerState playerState = playerObject._playerPrefab.GetComponent<PlayerState>();

                    playerState._forwardBlock -= percentage;

                    break;
                }
        }
    }

    [RPC]
    private void SC_RemoteAddPlayerToTeam(string name, int team)
    {
        foreach (PlayerMock player in players.Where(player => player.name == name))
        {
            player.team = (SelectedTeam)team;
        }
    }

    [RPC]
    //Server only
    private void S_ResetPositionToSpawnpoint(NetworkPlayer source)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        Transform spawnPoint = spawnPoints [Random.Range(0, spawnPoints.Length)].transform;
        GetPlayerObject(source)._playerPrefab.position = spawnPoint.position;
    }

    [RPC]
    private void AddPlayerToTeam(string name, int team)
    {
        Debug.Log("AddPlayerToTeam");
        foreach (PlayerMock player in players.Where(player => player.name == name))
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