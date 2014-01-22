using UnityEngine;
/*
public class FloatingHealthBar : MonoBehaviour {
    
    public Transform target;  // Object that this label should follow
    public Vector3 offset = Vector3.up;    // Units in world space to offset; 1 unit above object by default
    public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
    public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped
    public bool useMainCamera = true;   // Use the camera tagged MainCamera
    public Camera cameraToUse ;   // Only use this if useMainCamera is false
    Camera cam ;
    Transform thisTransform;
    Transform camTransform;
    
    void Start () 
    {
        thisTransform = transform;
        if (useMainCamera)
            cam = Camera.main;
        else
            cam = cameraToUse;
        camTransform = cam.transform;
    }
    
    
    void Update()
    {
        
        if (clampToScreen)
        {
            Vector3 relativePosition = camTransform.InverseTransformPoint(target.position);
            relativePosition.z =  Mathf.Max(relativePosition.z, 1.0f);
            thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
            thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
                                                 Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
                                                 thisTransform.position.z);
            
        }
        else
        {
            thisTransform.position = cam.WorldToViewportPoint(target.position + offset);
        }
    }
}*/

public class FloatingHealthBar : MonoBehaviour
{
    public Texture2D healthBarBackgroundTexture;
	public Texture2D healthBarTexture;
	public Transform target;
	public Camera camera;
    public Vector3 pre_offset;
    public float post_offset_x = 0;
    public float post_offset_y = 0;
    public int width = 100;
    public int height = 10;
    public int current_health = 100;
    public int sizeofborder = 4;
    
    public Transform _gameManagementObject;
    
    
    private void Start()
    {
        //offset = Vector3.zero;
    }

	private void OnGUI()
    {
        //TODO Screen Scaling..
        if(_gameManagementObject != null)
        {
            camera = _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponentInChildren<Camera>();
        }
        Vector3 wantedPos = camera.WorldToViewportPoint(target.position-pre_offset);
        int size = (width*current_health/100);
        if (size < 0)
        {
            size = 0;
        }
        if (size > width)
        {
            size = width;
        }
        
        
        GUI.DrawTexture(new Rect(Screen.width * wantedPos.x - post_offset_x - sizeofborder  , Screen.height - Screen.height * wantedPos.y  - post_offset_y - sizeofborder, width+sizeofborder*2, height+sizeofborder*2), healthBarBackgroundTexture);
        GUI.DrawTexture(new Rect(Screen.width * wantedPos.x - post_offset_x, Screen.height - Screen.height * wantedPos.y  - post_offset_y, size, height), healthBarTexture);
	}
}