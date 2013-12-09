using UnityEngine;

public class FlagPlatformTrigger : MonoBehaviour
{
	public string platformName;
    public GameManager _gameManager;

	public void OnTriggerEnter(Collider collider)
	{
        if (Network.isServer)
        {
            //Gather Player Information
            Transform player = collider.gameObject.transform;
            PlayerState playerState = player.GetComponent<PlayerState>();
            NetworkPlayer networkPlayer = playerState._networkPlayer;
            //Only do something if the player is not holding a flag
            if (playerState._isHoldingAFlag == true)
    		{
    			if (platformName == "PlatformRed" && playerState._team == Team.Red)
    			{
    				Debug.Log("Point for team BLUE");
                    _gameManager.networkView.RPC("AddPointForTeam",RPCMode.All,FlagDescription.FLAGRED);
                    
    			}
    			if (platformName == "PlatformBlue" && playerState._team == Team.Blue)
    			{
                    Debug.Log("Point for team RED");
                    _gameManager.networkView.RPC("AddPointForTeam",RPCMode.All,FlagDescription.FLAGBLUE);
                }
                playerState._isHoldingAFlag = false;
                _gameManager.networkView.RPC("SyncValuesForPlayer", RPCMode.Others, networkPlayer, CombatSyncValues.BOOLFLAG, 0); 
                _gameManager.networkView.RPC("RemoveFlagCarriedByPlayer",RPCMode.All,FlagDescription.FLAGBLUE);
                _gameManager.networkView.RPC("RemoveFlagCarriedByPlayer",RPCMode.All,FlagDescription.FLAGRED);
                _gameManager.networkView.RPC("ResetFlag", RPCMode.All, FlagDescription.FLAGRED);
                _gameManager.networkView.RPC("ResetFlag", RPCMode.All, FlagDescription.FLAGBLUE);
    		}
        }
	}
}