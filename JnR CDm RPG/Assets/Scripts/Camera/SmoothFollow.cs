using System;
using UnityEngine;
public class SmoothFollow : MonoBehaviour
{
	public Camera _cam;
	public Transform _target;
	public float _height = 8f;
	public float _distance = 10f;
	public float _damping = 12f;
	private void Awake()
	{
	}
	private void LateUpdate()
	{
		Vector3 position = this._target.transform.position;
		position.y += this._height;
		position.z -= this._distance;
		this._cam.transform.position = position;
		this._cam.transform.LookAt(this._target.transform.position);
	}
}
