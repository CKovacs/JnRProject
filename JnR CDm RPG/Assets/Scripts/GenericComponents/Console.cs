using UnityEngine;
using System;
using System.Collections.Generic;

public class Console : MonoBehaviour {
	private struct ConsoleMessage
	{
		public readonly string message;
		public readonly string stackTrace;
		public readonly LogType type;
		public ConsoleMessage(string message, string stackTrace, LogType type)
		{
			this.message = message;
			this.stackTrace = stackTrace;
			this.type = type;
		}
	}
	private const int margin = 20;
	public static readonly Version version = new Version(1, 0);
	public KeyCode toggleKey = KeyCode.Tab;
	private List<global::Console.ConsoleMessage> entries = new List<global::Console.ConsoleMessage>();
	private Vector2 scrollPos;
	public bool show;
	public bool collapse;
	private Rect windowRect = new Rect(20f, (float)(Screen.height / 2), (float)(Screen.width - 40), (float)(Screen.height / 2 - 40));
	private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
	private void OnEnable()
	{
		Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
	}
	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}
	private void Update()
	{
		if (Input.GetKeyDown(this.toggleKey))
		{
			this.show = !this.show;
		}
	}
	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		this.windowRect = GUILayout.Window(123456, this.windowRect, new GUI.WindowFunction(this.ConsoleWindow), "Console", new GUILayoutOption[0]);
	}
	void ConsoleWindow (int windowID)
	{
		scrollPos = GUILayout.BeginScrollView(scrollPos);
			
			// Go through each logged entry
			for (int i = 0; i < entries.Count; i++) {
				ConsoleMessage entry = entries[i];
 
				// If this message is the same as the last one and the collapse feature is chosen, skip it
				if (collapse && i > 0 && entry.message == entries[i - 1].message) {
					continue;
				}
 
				// Change the text colour according to the log type
				switch (entry.type) {
					case LogType.Error:
					case LogType.Exception:
						GUI.contentColor = Color.red;
						break;
 
					case LogType.Warning:
						GUI.contentColor = Color.yellow;
						break;
 
					default:
						GUI.contentColor = Color.white;
						break;
				}
 
				GUILayout.Label(entry.message);
			}
 
			GUI.contentColor = Color.white;
 
		GUILayout.EndScrollView();
 
		GUILayout.BeginHorizontal();
 
			// Clear button
			if (GUILayout.Button(clearLabel)) {
				entries.Clear();
			}
			
			// Collapse toggle
			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
 
		GUILayout.EndHorizontal();
 
		// Set the window to be draggable by the top title bar
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		global::Console.ConsoleMessage item = new global::Console.ConsoleMessage(message, stackTrace, type);
		this.entries.Add(item);
	}
}