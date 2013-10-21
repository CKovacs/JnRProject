using UnityEngine;
using System.Collections;

public class Strike : MonoBehaviour
{
    public Vector3 _transform;

    public void DoEffect(Vector3 source, Vector3 newTarget)
    {
        Vector3 position = Vector3.Lerp(transform.position, newTarget, Time.deltaTime * 10.1f);
    }
}
