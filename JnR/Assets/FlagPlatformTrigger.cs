using UnityEngine;

public class FlagPlatformTrigger : MonoBehaviour
{
	public string platformName;

	// Use this for initialization
	private void Start()
	{
	}



	public void OnTriggerEnter(Collider collider)
	{
		//Check if player is of the opposing team
		var player = collider.gameObject.transform; //-> player
		var playerState = player.GetComponent<PlayerState>();
		if (playerState._isHoldingAFlag)
		{
			if (platformName == "PlatformRed" && playerState._team == Team.Blue)
			{
				Debug.Log("Point team Blue");
			}
			if (platformName == "PlatformBlue" && playerState._team == Team.Red)
			{
				Debug.Log("Point team Red");
			}
		}
	}
}