using UnityEngine;
using System.Collections;

public class Strike : MonoBehaviour
{
    public int _speed = 300;

    public void DoEffect(Vector3 newSource, Vector3 newTarget)
    {
        transform.LookAt(newTarget);
        rigidbody.AddForce(transform.forward * _speed);
    }
}
