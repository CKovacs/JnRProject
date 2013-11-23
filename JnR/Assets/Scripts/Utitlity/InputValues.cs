using UnityEngine;
using System.Collections;

public class InputValues : MonoBehaviour {
	private AnimationHandle animHandle;
	// Use this for initialization
	void Start () {
		animHandle = GetComponent<AnimationHandle>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			animHandle.IdleRun(true);
		} 
		else
		{
			animHandle.IdleReady(true);	
		}
		
		if(Input.GetKey (KeyCode.R))
		{
			animHandle.BowFire(true);
		}
		
		if(Input.GetKey (KeyCode.F))
		{
			animHandle.OneHandHit(true);
		}
		
		if(Input.GetKey (KeyCode.B))
		{
			animHandle.ShieldBlock(true);
		}
		
		if(Input.GetKey(KeyCode.Space))
		{
			animHandle.Jump (true);	
		}
	}
}
