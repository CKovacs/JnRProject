using UnityEngine;
using System.Collections;
//Dieses Script entspricht nicht unseren Guidelines .. ändern!
public class AnimationHandle : MonoBehaviour {
	public Transform _gameManagementObject;
	public Animator animator;
	bool idleStandbool = true;
 	bool idleReadybool = false;
	string lastAnimation;
	public bool _localPlayer;
	
	public void NetworkAnmiation(string animation)
	{
		//DUMB SWITCH! BITTE ÄNDER MICH...
		switch(animation)
		{
			case "IdleReady":
				IdleReady (false);
				break;
			case "IdleRun":
				IdleRun (false);
				break;
			case"IdleStand":
				IdleStand(false);
				break;
		}
	}
	
	public void IdleStand(bool shouldSend=true){
    	Falses();
    	idleStandbool = true;

    	animator.SetBool("IdleStand", true);
		SendToServer("IdleStand", shouldSend);
    }
	
	public void IdleRun(bool shouldSend=true){
		IdleReady(shouldSend);
		animator.SetBool("IdleRun", true);
		SendToServer("IdleRun", shouldSend);
	}
	
	public void IdleReady(bool shouldSend=true){
    	Falses();
    	idleReadybool = true;
    	animator.SetBool("IdleReady", true);
		SendToServer("IdleReady", shouldSend);
    }
	
	void Falses(){
    	idleStandbool = false;
    	idleReadybool = false;
		
		
    	animator.SetBool("IdleReady", false);
    	animator.SetBool("IdleStand", false);
		animator.SetBool("IdleRun", false);
    }
		
	void SendToServer(string animation, bool shouldSend)
	{
		if(_localPlayer == true)
		{
			if(!shouldSend)// && animation == lastAnimation)
			{
				return;
			}
			lastAnimation = animation;
			_gameManagementObject.networkView.RPC("S_SendAnimation", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer, animation);
		}
	}
	
	void Update()
	{
		
	}
}
