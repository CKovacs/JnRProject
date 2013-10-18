using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{
    public GameObject _linkedPortal;

    public void OnTriggerEnter(Collider collider) 
    {
        collider.gameObject.transform.position = _linkedPortal.transform.position + collider.gameObject.transform.forward;
    }
}
