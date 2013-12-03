using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Portal : MonoBehaviour 
{
    public GameObject _linkedPortal;
    public GameManager _gameManager;
    public float _coolDownPortalUse = 3;

    public void OnTriggerEnter(Collider collider) 
	{
        Debug.Log("TEST");
        if(Network.isServer)
        {

            //Gather Player Information
            Transform player = collider.gameObject.transform;
            PlayerState playerState = player.GetComponent<PlayerState>();
            NetworkPlayer networkPlayer = playerState._networkPlayer;
            float ct = Time.time;
            //Check if player is allowed to enter the portal
            if(playerState._lastPortalTime <= ct -3f)
            {
                playerState._lastPortalTime = ct;
                _gameManager.networkView.RPC("PlayerReEnabling", RPCMode.Others, networkPlayer, false ? 1 : 0);
		        Vector3 tmp = _linkedPortal.transform.position;
		        tmp.y += 1;
                tmp = tmp + collider.gameObject.transform.forward *2;
                _gameManager.networkView.RPC("SetRespawnPosition", RPCMode.Others, networkPlayer, tmp);
                player.position = tmp;
                _gameManager.networkView.RPC("PlayerReEnabling", RPCMode.Others, networkPlayer, true ? 1 : 0);
            }
        }
    }
}
