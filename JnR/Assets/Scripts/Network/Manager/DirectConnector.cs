using UnityEngine;
using System.Collections;

public class DirectConnector : MonoBehaviour {	
	public void Connect()
	{
		Network.Connect("127.0.0.1", 25000);
	}
	
	public void StartServer()
	{
		Network.InitializeServer(10, 25000, false);
	}
	
	private void OnConnectedToServer()
	{
		Debug.Log ("Dissable MsgQueue");
		Network.isMessageQueueRunning = false;
		PlayerPrefs.SetString("connectIP", Network.connections[0].ipAddress);
		PlayerPrefs.SetInt("connectPort", Network.connections[0].port);
	}
	
}
