using UnityEngine;
using System.Collections;

public class InputDispatcher : MonoBehaviour {
	public Transform _gameManagementObject;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
		{
			_gameManagementObject.networkView.RPC ("RemoteAttack",RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				1);
		}
		
		if(Input.GetKeyDown(KeyCode.T))
		{
			_gameManagementObject.networkView.RPC ("ResetPositionToSpawnpoint",RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		}
	}
}
