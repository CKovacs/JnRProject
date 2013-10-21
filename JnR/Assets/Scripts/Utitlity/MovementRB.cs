using UnityEngine;
using System.Collections;
 
public class MovementRB : MonoBehaviour {
 
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool grounded = false;
	
	public bool _isLocalPlayer = true;
 
 	public Rigidbody _rigidBody;
	public Collider _collider;
	
	//Input
	public float _verticalInput = 0f;
	public float _horizontalInput = 0f;
	public bool _jump = false;
	public bool _hasUnsyncedJump = false;
 
	void Awake () {
	    _rigidBody.freezeRotation = true;
	    _rigidBody.useGravity = false;
	}
 
	void FixedUpdate () {
		if(_isLocalPlayer)
		{
			_verticalInput = Input.GetAxis("Vertical");
			_horizontalInput = Input.GetAxis ("Horizontal");
			_jump = Input.GetButton("Jump");
		}
		
		
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(_horizontalInput, 0, _verticalInput);
		targetVelocity.Normalize();
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = _rigidBody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        _rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
 		if (grounded) {
	        // Jump
	        if (canJump && (_hasUnsyncedJump || _jump)) {
	            _rigidBody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				_hasUnsyncedJump = false;
	        }
	    }
 
	    // We apply gravity manually for more tuning control
	    _rigidBody.AddForce(new Vector3 (0, -gravity * _rigidBody.mass, 0));
 
	    grounded = false;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}
 
	float CalculateJumpVerticalSpeed () {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}