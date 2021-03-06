using System;
using UnityEngine;

public class MovementNetwork : MonoBehaviour
{
	public Movement _movementScript;
	private float _verticalInput;
	private float _horizontalInput;
	private float _lastVerticalInput;
	private float _lastHorizontalInput;
	private bool _jumpInput;
	
	private void Update()
	{
		this._verticalInput = Input.GetAxis("Vertical");
		this._horizontalInput = Input.GetAxis("Horizontal");
		this._jumpInput = this._movementScript._didJump;
		this._movementScript._didJump = false;
		if (this._verticalInput != this._lastVerticalInput || this._horizontalInput != this._lastHorizontalInput || this._jumpInput)
		{
			if (!Network.isServer)
			{
				GetComponent<AnimationHandle>().IdleRun (true);
				networkView.RPC("SendUserInput", RPCMode.Server,_verticalInput,_horizontalInput,(!this._jumpInput) ? 0 : 1);
			}
		}
		if(this._verticalInput == 0 && this._horizontalInput == 0)
		{
			GetComponent<AnimationHandle>().IdleReady (true);		
		}
		_lastVerticalInput = _verticalInput;
		_lastHorizontalInput = _horizontalInput;
	}
	
	[RPC]
	private void SendUserInput(float vInput, float hInput, int jInput)
	{
		_movementScript._horizontalInput = hInput;
		_movementScript._verticalInput = vInput;
		if (jInput == 1)
		{
			this._movementScript._hasUnsyncedJump = true;
		}
	}
}
