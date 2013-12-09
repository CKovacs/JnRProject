using UnityEngine;

public class FlagHandling : MonoBehaviour
{
	public string _name;
	public Vector3 _startPosition;
	public GameManager _gameManager;

	//If a Character of the opposite team 
	//collides with the flag it gets attached
	//to the player and the script becomes 
	//disabled

	private void Start()
	{
		_startPosition = transform.position;
	}
	
	public void OnTriggerEnter(Collider collider)
	{
		//Check if player is of the opposing team
		var player = collider.gameObject.transform; //-> player
		var playerState = player.GetComponent<PlayerState>();
		if (playerState._isHoldingAFlag == false)
		{
			//enemy flag -> pick up
			if (playerState._team == Team.Blue && _name == "FlagRed" || playerState._team == Team.Red && _name == "FlagBlue")
			{
				Transform flagAttach = player.GetChild(1);
				transform.position = flagAttach.transform.position;
				transform.parent = flagAttach;
				playerState._isHoldingAFlag = true;

				enabled = false;
			}
			//own flag -> reset
			else
			{
				_gameManager.networkView.RPC("ResetFlag", RPCMode.All, player, _name=="FlagRed" ? 1 : 0);
			}
			
		}
	}
	
	public void ResetFlag()
	{
		Debug.Log("Reset Flag" + _name);
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