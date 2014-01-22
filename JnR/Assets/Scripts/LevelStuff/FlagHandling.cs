using UnityEngine;

public class FlagHandling : MonoBehaviour
{
	public int _flagId;
	public GameManager _gameManager;
	public bool _isAtStart;
	public string _name;
	public Vector3 _startPosition;

	//If a Character of the opposite team 
	//collides with the flag it gets attached
	//to the player and the script becomes 
	//disabled

	private void Start()
	{
		_isAtStart = true;
		_startPosition = transform.position;
		if (_name == "FlagRed")
		{
			_flagId = FlagDescription.FLAGRED;
		}
		else
		{
			_flagId = FlagDescription.FLAGBLUE;
		}
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (Network.isServer)
		{
			//Gather Player Information
			Transform player = collider.gameObject.transform;
			var playerState = player.GetComponent<PlayerState>();
			NetworkPlayer networkPlayer = playerState._networkPlayer;
			//Only do something if the player is not holding a flag
			if (playerState._isHoldingAFlag == false)
			{
				//Only a player of the opposing team can pick up the specific flag...
				if (playerState._team == Team.Blue && _flagId == FlagDescription.FLAGRED ||
				    playerState._team == Team.Red && _flagId == FlagDescription.FLAGBLUE)
				{
					//Attach the flag to the player triggering the collider this will move the flag without the need of an extra networkview
					FlagPickUp(networkPlayer);
					playerState._isHoldingAFlag = true;
					//Sync the data for the other clients!
					_gameManager.networkView.RPC("SyncValuesForPlayer", RPCMode.Others, networkPlayer, CombatSyncValues.BOOLFLAG, 1);
					_gameManager.networkView.RPC("FlagPickUp", RPCMode.Others, networkPlayer, _flagId);
					//Update the "Player Holding Flag" information
					_gameManager.networkView.RPC("FlagCarriedByPlayer", RPCMode.All, networkPlayer, _flagId);
					//disable the trigger as long as it is attached
					_isAtStart = false;
					enabled = false;
				}
					//the flag triggered is of the same team and can be reseted
				else if (!_isAtStart)
				{
					playerState._isHoldingAFlag = false;
					_gameManager.networkView.RPC("SyncValuesForPlayer", RPCMode.Others, networkPlayer, CombatSyncValues.BOOLFLAG, 0);
					_gameManager.networkView.RPC("ResetFlag", RPCMode.All, _flagId);
					_gameManager.networkView.RPC("RemoveFlagCarriedByPlayer", RPCMode.All, _flagId);
				}
			}
		}
	}

	public void FlagPickUp(NetworkPlayer player)
	{
		Debug.Log("A flag(" + _name + ") is picked up by player(" + player + ").");
		Transform playerPrefab = _gameManager.GetPlayerObject(player)._playerPrefab;
		Transform flagAttach = playerPrefab.GetChild(1);
		transform.position = flagAttach.transform.position;
		transform.parent = flagAttach;
	}

	public void ResetFlag()
	{
		Debug.Log("A flag(" + _name + ") has been returned to it's home");
		transform.parent = null;
		transform.position = _startPosition;
		enabled = true;
		_isAtStart = true;
	}

	public void DropFlag(int flagId)
	{
		Debug.Log("A flag(" + _name + ") has been dropped at " + transform.position + ".");
		transform.parent = null;
		enabled = true;
	}
}