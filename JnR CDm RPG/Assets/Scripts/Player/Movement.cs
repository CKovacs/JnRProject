using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public float _gravity = 20.0f;
	private Vector3 _vecGravity = Vector3.zero;
	public float _movementSpeed = 9f;
	public float _jumpSpeed = 10f;
	public bool _canJump = true;
	private Vector3 _jumpVelocity = Vector3.zero;
	
	public bool _isLocalPlayer = true;
	public bool _isMoving = true;
	
	//Input
	public float _verticalInput = 0f;
	public float _horizontalInput = 0f;
	public bool _jump = false;
	
	//Housekeeping
	public bool _didJump = false;
	public bool _hasUnsyncedJump = false;
	
	private CharacterController _characterController;
	
	public Vector3 _velocity;
	
	private void Start()
	{
		_verticalInput = 0f;
		_horizontalInput = 0f;
		_jump = false;
		
		_characterController = GetComponent<CharacterController>();
	}
	
	private void Update()
	{
		HandleMovement();
	}
	
	private void HandleMovement()
	{
		if(_isLocalPlayer)
		{
			_verticalInput = Input.GetAxis("Vertical");
			_horizontalInput = Input.GetAxis ("Horizontal");
			_jump = Input.GetButton("Jump");
		}
		
		Vector3 offset = new Vector3(_horizontalInput,0f,_verticalInput);
		offset.Normalize();
		offset *= _movementSpeed;
		
		if((_hasUnsyncedJump || _jump) && _canJump)
		{
			_jumpVelocity.y = _jumpSpeed;
			_canJump = false;
			_hasUnsyncedJump = false;
			_didJump = true;
		}
		if(!_characterController.isGrounded)
		{
			_vecGravity += new Vector3(0.0f,-_gravity,0.0f) * Time.deltaTime;
			_canJump = false;
		}
		else
		{
			_vecGravity = Vector3.zero;
			_canJump = true;
			_jumpVelocity.y = 0;
		}
		
		offset += _vecGravity;
		offset += _jumpVelocity;
		offset *= Time.deltaTime;
		_characterController.Move (offset);
		_velocity = _characterController.velocity;
		_isMoving = offset.magnitude > 0;
	}
}