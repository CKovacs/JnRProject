using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public Transform _parentTransform;
	
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
		_parentTransform = transform.parent.transform;
		_verticalInput = 0f;
		_horizontalInput = 0f;
		_jump = false;
		//_characterController = GetComponent<CharacterController>();
		_characterController = _parentTransform.GetComponent<CharacterController>();
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


	/*
	public float _gravity = 20f;
	private bool _isGrounded = false;
	
	public float _speed = 5f;
	public float _airSpeed = 5f;
	public bool _canJump = true;
	public float _jumpHeight = 2f;
	public float _jumpMaxPushTime = 0.3f;
	private float _jumpStartTime;
	
	private Vector3 _direction;
	public bool _isLocalPlayer;
	public float _verticalInput;
	public float _horizontalInput;
	public bool _jumpButtonPressed;
	public bool _didJump;
	public bool _hasUnsyncedJump;
	public bool _isMoving;
	
	private void Awake()
	{
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = false;
	}
	
	private void FixedUpdate()
	{
		//Physics is done by the Server
		if (Network.isServer)
		{
			rigidbody.AddForce(0f, -_gravity * rigidbody.mass, 0f);
		}
	}
	
	private void Update()
	{
		HandleMovement();
	}
	
	private void HandleMovement()
	{
		Vector3 offset = Vector3.zero;
		if (_isLocalPlayer) 
		{
			//Wir sind der lokale Spieler und dürfen (zwecks Prediction) die Input Werte überprüfen
			_horizontalInput = Input.GetAxis("Horizontal");
			_verticalInput = Input.GetAxis("Vertical");
			_jumpButtonPressed = Input.GetButton("Jump");
		}
		this._direction = new Vector3(this._horizontalInput, 0f, this._verticalInput);
		this._direction.Normalize();
		//if ((this._hasUnsyncedJump || this._jumpButtonPressed) && this._isGrounded)
		//{
		//	Vector3 velocity = base.rigidbody.velocity;
		//	base.rigidbody.velocity = new Vector3(velocity.x, this.CalculateJumpVerticalSpeed(this._jumpHeight), velocity.z);
		//}
		if (this._isGrounded)
		{
			this._direction *= _speed;
			offset = _direction * Time.deltaTime;
			transform.position += offset;
		}
		_isMoving = (offset.magnitude > 0);
	}
	
	private void OnCollisionStay()
	{
		this._isGrounded = true;
	}
	
	private float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2f * jumpHeight * this._gravity);
	}
}*/
