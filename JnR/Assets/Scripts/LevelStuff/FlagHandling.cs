using UnityEngine;

public class FlagHandling : MonoBehaviour
{
	public string _name;
	public Vector3 _startPosition;
	public GameManager _gameManager;
	public int _flagId;

	//If a Character of the opposite team 
	//collides with the flag it gets attached
	//to the player and the script becomes 
	//disabled

	private void Start()
	{
		_startPosition = transform.position;
		if(_name == "FlagRed")
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
		var playeeeerr = collider.gameObject.transform; //-> player
		if(Network.isClient){
			Debug.Log ("Client Owner = " + playeeeerr.networkView.owner);
			}
		if(Network.isServer)
		{
			Debug.Log ("Server Owner = " + playeeeerr.networkView.owner);
		}


		if(Network.isServer)
		{
			//Check if player is of the opposing team
			var player = collider.gameObject.transform; //-> player
			var playerState = player.GetComponent<PlayerState>();
			if (playerState._isHoldingAFlag == false)
			{
				//enemy flag -> pick up
				if (playerState._team == Team.Blue && _flagId == FlagDescription.FLAGRED || playerState._team == Team.Red && _flagId == FlagDescription.FLAGBLUE)
				{
					Transform flagAttach = player.GetChild(1);
					transform.position = flagAttach.transform.position;
					transform.parent = flagAttach;
					playerState._isHoldingAFlag = true;
	//				player.networkView.
					_gameManager.networkView.RPC("SyncValuesForPlayer", RPCMode.Others, player, CombatSyncValues.BOOLFLAG, 1); 
					_gameManager.networkView.RPC("FlagPickUp", RPCMode.Others,player,_flagId);
					enabled = false;
				}
				//own flag -> reset
				else
				{
					_gameManager.networkView.RPC("ResetFlag", RPCMode.All, _flagId);
				}
			}
		}
	}

	public void FlagPickUp(NetworkPlayer player)
	{
		//Transform playerPrefab = player;// _gameManager.GetPlayerObject(player);
		//Transform flagAttach = playerPrefab.GetChild(1);
		//transform.position = flagAttach.transform.position;
		//transform.parent = flagAttach;
	}

	public void ResetFlag()
	{
		Debug.Log("Reset Flag " + _name);
		transform.parent = null;
		transform.position = _startPosition;
		enabled = true;
	}

	public void DropFlag(int flagId)
	{
		Debug.Log("Drop flag on: " + _name);
		transform.parent = null;
		enabled = true;
	}
}