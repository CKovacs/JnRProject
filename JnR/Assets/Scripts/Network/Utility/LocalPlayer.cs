using UnityEngine;
using System.Collections;

//Will only be used on the client side
public class LocalPlayer : MonoBehaviour {
	public NetworkPlayer _networkPlayer;
	public NetworkViewID _networkViewID;
	public Transform _playerPrefab;
	public bool _isInstantiated;
	
	[RPC]
	private void C_InitiateLocalPlayer(NetworkPlayer player, NetworkViewID viewID)
	{
		if(Network.isServer)
		{
			return;
		}
		string text = string.Empty;
		if(Network.isClient)
		{
			text += "Client:";
		}
		text += "InitiateLocalPlayer: player init " + player +". ViewIDs " + viewID;
		Debug.Log(text);
		_networkPlayer = player;
		_networkViewID = viewID;
	}
	
	private void OnGUI()
	{
		
		if(_isInstantiated == false)
		{
			return;	
		}
		if(Network.isServer)
		{
			enabled = false;
			return;	
		}
		GUILayout.Label("NetworkPlayer:" + this._networkPlayer.guid, new GUILayoutOption[0]);
		GUILayout.Label("NetworkViewID:" + this._networkViewID, new GUILayoutOption[0]);
		if (this._isInstantiated)
		{
			GUILayout.Label("Is instantiated!", new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label("Is not instantiated", new GUILayoutOption[0]);
		}
		GUILayout.Label ("HP: " + _playerPrefab.GetComponent<PlayerState>()._hp, new GUILayoutOption[0]);
		
		
		//BUTTON IDLESTAND
		if (GUILayout.Button("STAND", new GUILayoutOption[0]))
        {
            Debug.Log ("Animation Stand Invoke");
			_playerPrefab.GetComponent<AnimationHandle>().IdleStand();
        }
		//BUTTON IDLERUN
		if (GUILayout.Button("RUN", new GUILayoutOption[0]))
        {
            Debug.Log ("Animation Run Invoke");
			_playerPrefab.GetComponent<AnimationHandle>().IdleRun();
        }
	}
}