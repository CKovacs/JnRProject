using UnityEngine;
using System.Collections;

public class LocalPlayer : MonoBehaviour {
	public NetworkPlayer _localPlayer;
	public NetworkViewID _localTransformViewID;
	public bool _isInstantiated;
	
	[RPC]
	private void InitPlayer(NetworkPlayer player, NetworkViewID viewID)
	{
		string text = string.Empty;
		if (Network.isServer)
		{
			text += "Server: ";
		}
		else
		{
			text += "Client: ";
		}
		text += "Received player init " + player +". ViewIDs " + viewID;
		Debug.Log(text);
		this._localPlayer = player;
		this._localTransformViewID = viewID;
	}
	private void OnGUI()
	{
		GUILayout.Label("NetworkPlayer:" + this._localPlayer.guid, new GUILayoutOption[0]);
		GUILayout.Label("NetworkViewID:" + this._localTransformViewID, new GUILayoutOption[0]);
		if (this._isInstantiated)
		{
			GUILayout.Label("Instantiated", new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label("Not Instantiated", new GUILayoutOption[0]);
		}
	}
}