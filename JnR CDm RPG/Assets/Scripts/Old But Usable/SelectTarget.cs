using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectTarget : MonoBehaviour
{
    //public Camera _cam;
    public List<Transform> _visibleTargets;
	public bool changed;
        
    // Use this for initialization
    void Start()
    {
		changed = false;
    }
}
