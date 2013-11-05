using UnityEngine;
using System.Collections;

//Will only be used on the client side
public class LocalPlayer : MonoBehaviour {
	public NetworkPlayer _networkPlayer;
	public NetworkViewID _networkViewID;
	public Transform _playerPrefab;
	public bool _isInstantiated;
	
	[RPC]
	private void InitiateLocalPlayer(NetworkPlayer player, NetworkViewID viewID)
	{
		string text = string.Empty;
		text += "WOWOWOWOOWOWOWOWOWOWOWOWOWOWOWWO";
		if (Network.isServer)
		{
			text += "Server:";
		}
		else
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
	}
}