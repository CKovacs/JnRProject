using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ServerSkillContainer : MonoBehaviour {
	public List<Skill> _skillList;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	public Skill GetSkill(int skillid)
	{
		for(int i = 0;i<_skillList.Count;++i)
		{
			if(_skillList[i]._id == skillid)
			{
				return _skillList[i];
			}
		}
		return null;
	}
}
