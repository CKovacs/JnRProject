using UnityEngine;
using System.Collections;

/**
 * Author: Nikolaus Pöttler
 * Date: 13.06.13
 * This Script is ready for Networking
 * TODO: 
 * * Add Walk / Run Speed
 * * Add Animation Handle
 * * (Add Handling for Rotation?)
 **/

//http://docs.unity3d.com/Documentation/ScriptReference/RequireComponent.html 
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Movement : MonoBehaviour
{
    public const string MULTIPLAYERMANAGER = "MultiplayerManager";

    //Physic Specific
    public float _gravity = 20;
    public float _speed = 5;
    public float _airSpeed = 5;

    //Jump Specific
    public bool _canJump = true;        //Is Jumping allowed?
    public float _jumpHeight = 2;
    public float _jumpMaxPushTime = 0.3f; //in MS
    private float _jumpStartTime;
    private bool _isGrounded;
    private Vector3 targetVelocity;
    private Vector3 velocity;

    //http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Awake.html
    //http://answers.unity3d.com/questions/10189/what-is-the-general-use-of-awake-start-update-fixe.html
    void Awake()
    {
        GameObject multiplayerManager;
        multiplayerManager = GameObject.Find(MULTIPLAYERMANAGER);
        if (multiplayerManager == null)
        {
            rigidbody.freezeRotation = true;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
			_isGrounded = false;
			
            //networkView.RPC("UpdateMovement", RPCMode.OthersBuffered,
            //              transform.position, transform.rotation);
        }
        else
        {
            if (networkView.isMine == true)
            {
                rigidbody.freezeRotation = true;
                rigidbody.useGravity = false;
            	rigidbody.isKinematic = false;
                _isGrounded = false;

                //networkView.RPC("UpdateMovement", RPCMode.OthersBuffered,
                //                transform.position, transform.rotation);
            }
            else
            {
                rigidbody.freezeRotation = true;
                rigidbody.useGravity = false;
            	rigidbody.isKinematic = false;
				_isGrounded = false;
           }
        }
    }


    //http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.FixedUpdate.html
    void FixedUpdate()
    {
        //As of http://answers.unity3d.com/questions/270552/not-sure-if-this-should-go-in-update-or-fixedupdat.html
        //and http://answers.unity3d.com/questions/275016/updatefixedupdate-motion-stutter-not-another-novic.html
        //everything has moved to Update expect some gravity stuff.
        Debug.Log("test");
        //Gravity is applied per hand - this way each character could have different gravity values
        rigidbody.AddForce(0, -_gravity * rigidbody.mass, 0);
    }

    void Update()
    {
        velocity = rigidbody.velocity;

        if (_isGrounded == true)
        {
            targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity *= _speed;
            transform.position += targetVelocity * Time.deltaTime;

            if (_canJump && Input.GetButton("Jump"))
            {
                rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(_jumpHeight), velocity.z);
                targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity *= _airSpeed;
                transform.position += targetVelocity * Time.deltaTime;
                _jumpStartTime = Time.time;
            }
        }
        else
        {
            if (Input.GetButton("Jump") && (Time.time - _jumpStartTime) < _jumpMaxPushTime)
            {
                rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(_jumpHeight), velocity.z);
            }
            targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity *= _airSpeed;
            transform.position += targetVelocity * Time.deltaTime;
        }

        _isGrounded = false;

        /*if(!_networkOff)
        {
            networkView.RPC("UpdateMovement", RPCMode.OthersBuffered,
                            transform.position, transform.rotation);
        }
        else
        {
            UpdateMovement (transform.position, transform.rotation);
        }*/
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {

    }

    //http://docs.unity3d.com/Documentation/ScriptReference/Collider.OnCollisionStay.html
    void OnCollisionStay()
    {
        _isGrounded = true;
    }

    float CalculateJumpVerticalSpeed(float jumpHeight)
    {
        return Mathf.Sqrt(2 * jumpHeight * _gravity);
    }

    [RPC]
    void UpdateMovement(Vector3 newPosition, Quaternion newRotation)
    {
        /*transform.position = newPosition;
        transform.rotation = newRotation;*/
    }
}
