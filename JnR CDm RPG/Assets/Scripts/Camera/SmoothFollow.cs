using UnityEngine;
using System.Collections;

/**
 * Author: Nikolaus Pöttler
 * Date: 13.06.13
 **/

public class SmoothFollow : MonoBehaviour
{
    //Camera Specific
    public Transform _target;           //The position of the target
    public float _height = 8;           //The offset in the y-axis
    public float _distance = 10;        //The offset in the z-axis
    public float _damping = 12;

    void Awake()
    {
        /*if (networkView.isMine != true)
        {
            enabled = false;
        }*/
    }

    void Start()
    {
        if (_target == null)
        {
            Debug.LogError("There is no target linked");
            return;
        }
    }


    void LateUpdate()
    {
        //adjusting the Camera
        Vector3 desiredPosition = _target.transform.position;

        desiredPosition.y += _height;
        desiredPosition.z -= _distance;
        //Vector3 position = Vector3.Lerp(transform.position, desiredPosition, _damping * Time.deltaTime);
        transform.position = desiredPosition;
        transform.LookAt(_target.transform.position);
    }
}
