using UnityEngine;

public class FlagPickUp : MonoBehaviour
{
	//If a Character of the opposite team 
	//collides with the flag it gets attached
	//to the player and the script becomes 
	//disabled
	public void OnTriggerEnter(Collider collider)
	{
		//Check if player is of the opposing team
		Transform player = collider.gameObject.transform; //-> player
		var playerState = player.GetComponent<PlayerState>();
		if (playerState._isHoldingAFlag == false)
		{
			Transform flagAttach = player.GetChild(1);
			transform.position = flagAttach.transform.position;
			transform.parent = flagAttach;
			playerState._isHoldingAFlag = true;

			enabled = false;
		}
	}

	
}