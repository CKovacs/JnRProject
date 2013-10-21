using System;
using UnityEngine;

public class MovementNetworkRB : MonoBehaviour
{
	public MovementRB _movementScript;
	private float _verticalInput;
	private float _horizontalInput;
	private float _lastVerticalInput;
	private float _lastHorizontalInput;
	private bool _jumpInput;
	
	private void Update()
	{
		this._verticalInput = Input.GetAxis("Vertical");
		this._horizontalInput = Input.GetAxis("Horizontal");
		this._jumpInput = Input.GetButton("Jump");
		if (this._verticalInput != this._lastVerticalInput || this._horizontalInput != this._lastHorizontalInput || this._jumpInput)
		{
			if (!Network.isServer)
			{
				networkView.RPC("SendUserInput", RPCMode.Server,_verticalInput,_horizontalInput,(!this._jumpInput) ? 0 : 1);
			}
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
