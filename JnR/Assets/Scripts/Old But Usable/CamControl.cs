using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour
{
    public float _speed = 0.17f;
    public float _jumpHigh = 160.0f;
    public Camera _cam;
    public Camera _miniCam;
    public float _gravity;// = -20.0f;
	public Vector3 _camOffset;
    public Vector3 _miniCamOffset;
    private float _SmoothValue = 3.0f;
    private bool _canJump = false;
    private float _lastY;
	private Vector3 _lastPos;
	bool irgendwas;
    bool overviewMode;

    int _xDir = -1;
    int _yDir = -1;
    bool _horizontalVerticalSwitch = true;

    public float _jumpTimer;
    public float _buttonTimer;

    // Use this for initialization
    void Start()
    {
        _lastY = transform.position.y;
        Physics.gravity = new Vector3(0, _gravity, 0);
        _camOffset = new Vector3(0.0f, 13.0f, 16.0f);
        _miniCamOffset = new Vector3(0.0f, 50.0f, 1.0f);
		_lastPos = this.transform.position;
		irgendwas = false;
        overviewMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Example());
    }

    void OnCollisionEnter(Collision collision)
    {
        _canJump = true;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        _canJump = false;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        _canJump = true;
    }

    IEnumerator Example()
    {
        if (_buttonTimer > 0)
        {
            _buttonTimer -= Time.deltaTime; //in sec	
        }

        if (_buttonTimer < 0)
        {
            _buttonTimer = 0;
        }

        if (!overviewMode)
        {

            if (irgendwas)
            {
                animation.Play("run");
                irgendwas = true;
            }
            else if (!irgendwas)
            {
                animation.CrossFade("idle");
                irgendwas = false;
            }

            Vector3 newPos;

            if (_horizontalVerticalSwitch)
            {
                newPos = new Vector3(transform.position.x + Input.GetAxis("Horizontal") * _xDir, transform.position.y, transform.position.z + Input.GetAxis("Vertical") * _yDir);
            }
            else
            {
                newPos = new Vector3(transform.position.x + Input.GetAxis("Vertical") * _xDir, transform.position.y, transform.position.z + Input.GetAxis("Horizontal") * _yDir);
            }

            MoveObject(this.transform, this.transform.position, newPos, _speed);

            float damping = 12;

            Vector3 desiredPosition = transform.position + _camOffset;
            Vector3 position = Vector3.Lerp(_cam.transform.position, desiredPosition, Time.deltaTime * damping);
            _cam.transform.position = position;
            _cam.transform.LookAt(this.transform.position);

            Vector3 desiredPosition2 = transform.position;
            desiredPosition2.y = 0;
            desiredPosition2 += _miniCamOffset;

            Vector3 position2 = Vector3.Lerp(_miniCam.transform.position, desiredPosition2, Time.deltaTime * 100);
            _miniCam.transform.position = position2;
            _miniCam.transform.LookAt(this.transform.position);

            if (_jumpTimer > 0)
            {
                _jumpTimer -= Time.deltaTime; //in sec	
            }

            if (_jumpTimer < 0)
            {
                _jumpTimer = 0;
            }

            if ((Input.GetKey(KeyCode.I) || Input.GetButton("AB")) && _canJump && _jumpTimer == 0)
            {
                rigidbody.AddForce(new Vector3(0.0f, _jumpHigh, 0.0f));
                _jumpTimer = 0.4f;
            }

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                irgendwas = true;
            }
            else
            {
                irgendwas = false;
            }

            if (Input.GetKey(KeyCode.P) && _buttonTimer == 0)
            {
                _camOffset.y = 30.0f;
                _camOffset.z = 1.0f;
                desiredPosition = transform.position + _camOffset;
                //position = Vector3.Lerp(_cam.transform.position, desiredPosition, Time.deltaTime * damping);
                _cam.transform.position = desiredPosition;
                _cam.transform.LookAt(this.transform.position);
                _buttonTimer = 0.4f;
                overviewMode = true;
            }

            _lastPos = transform.position;
            _lastY = _lastPos.y;
        }
        else
        {
            Vector3 newPos = new Vector3(_cam.transform.position.x + Input.GetAxis("Horizontal") * _xDir, _cam.transform.position.y, _cam.transform.position.z + Input.GetAxis("Vertical") * _yDir);

            _cam.transform.position = newPos;

            if (Input.GetKey(KeyCode.P) && _buttonTimer == 0)
            {
                _camOffset.y = 13.0f;
                _camOffset.z = 16.0f;
                Vector3 desiredPosition = transform.position + _camOffset;
                //position = Vector3.Lerp(_cam.transform.position, desiredPosition, Time.deltaTime * damping);
                _cam.transform.position = desiredPosition;
                _cam.transform.LookAt(this.transform.position);
                _buttonTimer = 0.4f;

                overviewMode = false;
            }
        }
        yield return null;
    }

    void MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        float rate = 1.0f / time;
        float i = 0.0f;
        if (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			thisTransform.LookAt(endPos);	
        }
    }
}