using UnityEngine;

public class FloatingHealthBar : MonoBehaviour
{
	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	private Vector3 _scale;

	public Transform _gameManagementObject;
	public Camera camera;
	public int current_health = 100;
	public Texture2D healthBarBackgroundTexture;
	public Texture2D healthBarTexture;
	public int height = 10;
	public float post_offset_x = 0;
	public float post_offset_y = 0;
	public Vector3 pre_offset;
	public int sizeofborder = 4;
	public Transform target;
	public int width = 100;


	private void OnGUI()
	{
		//scaling stuff for different resolutions
		float rx = Screen.width / _originalWidth;
		float ry = Screen.height / _originalHeight;

		GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));
		
		if (_gameManagementObject != null)
		{
			camera = _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponentInChildren<Camera>();
		}
		Vector3 wantedPos = camera.WorldToViewportPoint(target.position - pre_offset);
		int size = (width*current_health/100);
		if (size < 0)
		{
			size = 0;
		}
		if (size > width)
		{
			size = width;
		}


		GUI.DrawTexture(
			new Rect(_originalWidth * wantedPos.x - post_offset_x - sizeofborder,
				_originalHeight - _originalHeight * wantedPos.y - post_offset_y - sizeofborder, width + sizeofborder * 2,
				height + sizeofborder*2), healthBarBackgroundTexture);
		GUI.DrawTexture(
			new Rect(_originalWidth * wantedPos.x - post_offset_x, _originalHeight - _originalHeight * wantedPos.y - post_offset_y, size,
				height), healthBarTexture);
	}
}