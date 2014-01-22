using UnityEngine;

public class FloatingHealthBar : MonoBehaviour
{
	public Texture2D healthBarTexture;
	public Transform target;
	public Camera camera;

	private void Start()
	{
		camera = FindObjectOfType<Camera>();
	}
	private void Update()
	{

		Vector3 wantedPos = Camera.main.WorldToScreenPoint(target.position);

		transform.position = wantedPos;

	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(transform.position.x, transform.position.y, 30, 5),
				healthBarTexture);

		
	}
}