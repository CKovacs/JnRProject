using UnityEngine;
using System.Collections;

public class SkillTest : MonoBehaviour 
{
    public Transform _source;
    public Transform _destination;
    public Skill _skill;

	void LateUpdate () 
    {
        Effect3DBuilder.DoEffect(_source, _destination, _skill);
	}
}
