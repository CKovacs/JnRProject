﻿using UnityEngine;
using System.Collections;

//This is a Debug Player State;
//For now it is a monobehaviour and uses onSerializeNetworkView
public class PlayerState : MonoBehaviour {
	
	public int _hp = 100;
	public int _dmg = 5;
	
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			int hp = _hp;
			stream.Serialize(ref hp);
		}
		// Read data from remote client
		else
		{
			int hp = 0;
			stream.Serialize(ref hp);
			_hp = hp;
		}
	}	
}
