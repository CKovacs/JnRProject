using UnityEngine;
using System.Collections;

//Dieses Script entspricht nicht unseren Guidelines .. ändern!
public class AnimationHandle : MonoBehaviour
{
	public Transform _gameManagementObject;
	public Animator animator;
	public bool _localPlayer;
	
	public void IdleReady (bool shouldSend)
	{
		Falses ();
		animator.SetBool ("IdleReady", true);
		SendToServer("IdleReady",shouldSend);
	}
	
	public void IdleRun (bool shouldSend)
	{
		IdleReady (false);
		animator.SetBool ("IdleRun", true);
		SendToServer("IdleRun",shouldSend);
	}
	
	public void OneHandHit(bool shouldSend)
	{
		animator.SetBool ("OneHandHit",true);
		SendToServer("OneHandHit",shouldSend);	
	}
	
	public void ShieldBlock(bool shouldSend)
	{
		animator.SetBool ("ShieldBlock",true);	
		SendToServer("ShieldBlock",shouldSend);
	}
	
	public void Jump(bool shouldSend)
	{
		if(animator.GetBool ("IdleRun"))
		{
			animator.SetBool ("JumpRun",true);	
			SendToServer("JumpRun",shouldSend);
		} 
		else if(animator.GetBool ("IdleReady"))
		{
			animator.SetBool ("JumpStand",true);	
			SendToServer("Jump",shouldSend);
		}
	}
	
	public void BowFire(bool shouldSend)
	{
		animator.SetBool ("BowFire",true);
		SendToServer("BowFire",shouldSend);
	}
	
	void Falses ()
	{
		animator.SetBool ("IdleReady", false);
		animator.SetBool ("IdleRun", false);
		animator.SetBool ("OneHandHit",false);
		animator.SetBool ("ShieldBlock", false);
		animator.SetBool ("JumpRun", false);
		animator.SetBool ("JumpStand", false);
		animator.SetBool ("BowFire",false);
	}
	
	
	public void NetworkAnmiation (string animation)
	{
		//DUMB SWITCH! BITTE ÄNDER MICH...
		switch (animation) {
		case "IdleReady":
			IdleReady (false);
			break;
		case "IdleRun":
			IdleRun (false);
			break;
		case "OneHandHit":
			OneHandHit(false);
			break;
		case "ShieldBlock":
			ShieldBlock(false);
			break;
		case "Jump":
			Jump (false);
			break;
		}
	}
	
	void SendToServer (string animation, bool shouldSend)
	{
		if (_localPlayer == true) {
			if (!shouldSend) {// && animation == lastAnimation)
				return;
			}
			//lastAnimation = animation;
			_gameManagementObject.networkView.RPC ("S_SendAnimation", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer> ()._networkPlayer, animation);
		}
	}
	
	void Update()
	{
			
	}
}
