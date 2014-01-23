using UnityEngine;

public class StartScreen : MonoBehaviour
{

	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	private Vector3 _scale;
	public Texture2D startScreenTexture;
	public GUIStyle tutorialGuiStyle;
	public GUIStyle gameGuiStyle;

	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void OnGUI()
	{

		float rx = Screen.width / _originalWidth;
		float ry = Screen.height / _originalHeight;
		GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

		GUI.DrawTexture(new Rect(0,0, _originalWidth, _originalHeight), startScreenTexture);

		if (GUI.Button(new Rect(_originalWidth / 2 - 200, 500, 400, 90), "Network Game", gameGuiStyle))
		{
			Application.LoadLevel("Menu_Test_Scene");
		}

		if (GUI.Button(new Rect(_originalWidth / 2 - 200, 600, 400, 90), "Tutorial", tutorialGuiStyle))
		{
			Application.LoadLevel("Tutorial");
		}
	}
}