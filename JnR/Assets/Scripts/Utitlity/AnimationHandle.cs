using UnityEngine;
using System.Collections;

//Dieses Script entspricht nicht unseren Guidelines .. ändern!
public class AnimationHandle : MonoBehaviour
{
		public Transform _gameManagementObject;
		public Animator _animator;
		public bool _localPlayer;

		public IEnumerator OneShotAnimationHelper (string paramName)
		{
				_animator.SetBool (paramName, true);
				yield return new WaitForSeconds(0.1f);
				_animator.SetBool (paramName, false);
		
		}

		public void IdleReady (bool shouldSend)
		{
				Falses ();
				_animator.SetBool ("IdleReady", true);
				SendToServer ("IdleReady", shouldSend);
		}
	
		public void IdleRun (bool shouldSend)
		{
				IdleReady (false);
				_animator.SetBool ("IdleRun", true);
				SendToServer ("IdleRun", shouldSend);
		}
	
		public void OneHandHit (bool shouldSend)
		{
				StartCoroutine(OneShotAnimationHelper("OneHandHit"));
				//_animator.SetBool ("OneHandHit", true);
				SendToServer ("OneHandHit", shouldSend);	
		}
	
		public void ShieldBlock (bool shouldSend)
		{
				_animator.SetBool ("ShieldBlock", true);	
				SendToServer ("ShieldBlock", shouldSend);
		}
	
		public void Jump (bool shouldSend)
		{
			if (_animator.GetBool ("IdleRun")) {
					_animator.SetBool ("JumpRun", true);	
					SendToServer ("JumpRun", shouldSend);
			} else if (_animator.GetBool ("IdleReady")) {
					_animator.SetBool ("JumpStand", true);	
					SendToServer ("Jump", shouldSend);
			}
		}
	
		public void BowFire (bool shouldSend)
		{
				_animator.SetBool ("BowFire", true);
				SendToServer ("BowFire", shouldSend);
		}
	
		void Falses ()
		{
				_animator.SetBool ("IdleReady", false);
				_animator.SetBool ("IdleRun", false);
				_animator.SetBool ("OneHandHit", false);
				_animator.SetBool ("ShieldBlock", false);
				_animator.SetBool ("JumpRun", false);
				_animator.SetBool ("JumpStand", false);
				_animator.SetBool ("BowFire", false);
		}
	
		public void NetworkAnmiation (string animation)
		{
		//		Falses ();
		//
		//		//Auf String Hashes umstellen
		//		switch (animation) {
		//			case "IdleReady":
		//					IdleReady (false);
		//					break;
		//			case "IdleRun":
		//					IdleRun (false);
		//					break;
		//			case "OneHandHit":
		//					OneHandHit (false);
		//					break;
		//			case "ShieldBlock":
		//					ShieldBlock (false);
		//					break;
		//			case "Jump":
		//					Jump (false);
		//					break;
		//		}
		}
	
		void SendToServer (string animation, bool shouldSend)
		{
		//		if (_localPlayer == true) {
		//			if (!shouldSend) {// && animation == lastAnimation)
		//					return;
		//			}
		//			//lastAnimation = animation;
		//			_gameManagementObject.networkView.RPC ("S_SendAnimation", RPCMode.Server,
		//			_gameManagementObject.GetComponent<LocalPlayer> ()._networkPlayer, animation);
		//		}
		}
	
		void Update ()
		{
		
		}
}
