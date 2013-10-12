using UnityEngine;
using System.Collections;

public class ChangeShader : MonoBehaviour
{
	public Transform _target;
	public Material _transparentMat;
	
	private Transform _lastHit = null;
	private Material _savedMat;
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit selectField;
		
		Debug.DrawRay (Camera.main.transform.position, _target.position - Camera.main.transform.position);
		
		if (Physics.Raycast (Camera.main.transform.position, _target.position - Camera.main.transform.position, out selectField)) 
		{	
			if(selectField.transform != _target && selectField.transform != _lastHit)
			{	
				_lastHit = selectField.transform;
				_savedMat = _lastHit.renderer.material;
				
				_lastHit.renderer.material = _transparentMat;
			}
			else if(selectField.transform == _target && _lastHit != null)
			{
				_lastHit.renderer.material = _savedMat;
				_lastHit = null;
			}
		}
	}
}
