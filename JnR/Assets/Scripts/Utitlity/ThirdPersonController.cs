using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour {
	private Transform _parentTransform;
	private CharacterController _characterController;
	public float _gravity = 5.0f;
	public float _movementSpeed = 5.0f;
	public float _jumpSpeed = 50.0f;
	public float _fallSpeed = 30.0f;
	

	
	private Vector3 _airVelocity = Vector3.zero;
	private bool _canJump = true;
	public bool _isLocalPlayer = false;
	public bool _isMoving = false;
	
	public float _verticalInput;
	public float _horizontalInput;
	public bool _jumpButtonPressed = false;
	public bool _didJump;
	public bool _hasUnsyncedJump;
		
	// Use this for initialization
	void Start () {
		_horizontalInput = 0.0f;
		_verticalInput = 0.0f;
		_jumpButtonPressed = false;
		_parentTransform = transform.parent;
		_characterController = _parentTransform.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		HandleMovementInput();
	}
	
	void HandleMovementInput()
	{
		if(_isLocalPlayer)
		{
			_horizontalInput = Input.GetAxis ("Horizontal");
			_verticalInput = Input.GetAxis ("Vertical");
			_jumpButtonPressed = Input.GetButtonDown ("Jump");
		}
		
		Vector3 _movementDirection = new Vector3(_horizontalInput, 0, _verticalInput);
		_movementDirection.Normalize();
		if(_horizontalInput != 0 || _verticalInput != 0) 
		{
			Rotating(_horizontalInput,_verticalInput);
		}
		Vector3 _movementOffset = _movementDirection * _movementSpeed;
		
		if((_hasUnsyncedJump || _jumpButtonPressed) && _canJump)
		{
			_airVelocity.y = _jumpSpeed;
			_canJump = false;
			_hasUnsyncedJump = false;
			_didJump = true;
		}
		else if(_airVelocity.y > -_fallSpeed)
		{
			_airVelocity.y -= _gravity * Time.deltaTime;
		}
		
		if(_characterController.isGrounded && _airVelocity.y < 0)
		{
			_airVelocity.y = 0;
			_canJump = true;
		}
		
		_movementOffset += _airVelocity;
		_movementOffset *= Time.deltaTime;
		_characterController.Move (_movementOffset);
		_isMoving = (_movementOffset.magnitude > 0);
	}
		
		
	void Rotating (float horizontal, float vertical)
    {
        // Create a new vector of the horizontal and vertical inputs.
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        
        // Create a rotation based on this new vector assuming that up is the global y axis.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        
        // Create a rotation that is an increment closer to the target rotation from the player's rotation.
        Quaternion newRotation = Quaternion.Lerp(_parentTransform.rotation, targetRotation, 15 * Time.deltaTime);
        
        // Change the players rotation to this new 
		_parentTransform.rotation = newRotation;
    }
}
