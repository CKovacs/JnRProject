using UnityEngine;
using System.Collections;

public class FlagPickUp : MonoBehaviour {



	//If a Character of the opposite team 
	//collides with the flag it gets attached
	//to the player and the script becomes 
	//disabled
	public void OnTriggerEnter(Collider collider) 
	{
		//Check if player is of the opposing team
		Transform entered = collider.gameObject.transform;
		PlayerState ps = entered.GetComponent<PlayerState>();
		if(ps._isHoldingAFlag == false)
		{
			Transform flagAttach = entered.GetChild (1);
			transform.position = flagAttach.transform.position;
			transform.parent = flagAttach;
			ps._isHoldingAFlag = true;

			this.enabled = false;
		}
	}
}
