using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{
    public GameObject _linkedPortal;

    public void OnTriggerEnter(Collider collider) 
	{
		Vector3 tmp = _linkedPortal.transform.position;
		tmp.y += 1;
        collider.gameObject.transform.position = tmp + collider.gameObject.transform.forward *2;
    }
}
