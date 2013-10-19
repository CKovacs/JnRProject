using UnityEngine;
using System.Collections;

public class TargetListUpdate : MonoBehaviour
{
    private SelectTarget _selectTargetScript;

    void Start() 
    {
        _selectTargetScript = transform.parent.GetComponent<SelectTarget>();
    }

    void OnBecameVisible()
    {
        _selectTargetScript._visibleTargets.Add(transform);
		_selectTargetScript.changed = true;
        Debug.Log("Add " + name);
    }

    void OnBecameInvisible()
    {
        _selectTargetScript._visibleTargets.Remove(transform);
		_selectTargetScript.changed = true;
        Debug.Log("Remove " + name);
    }
}
