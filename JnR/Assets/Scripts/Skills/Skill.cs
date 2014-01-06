using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject
{
	// Display stuff
	public GameObject _3dEffect;
	public Effect3DType _3dEffectType;
	public int _casttime; // If _castTime == 0   -> skill is instant, otherwise the skill has a cast time
	public List<Class> _classSpell;
	public float _cooldown;
	public float _cooldownCounter;
	public float _destroyTime;
	public List<Effect> _effects;
	public Texture2D _icon;
	public int _id;
	public bool _needToBeInFront;
	public bool _needToHitOnBack;
	public bool _onCooldown;
	public bool _penetrationProjectile = false;
	public float _range;
	public Vector3 _spellOffSetSource;
	public Vector3 _spellOffSetTarget;
	public List<TargetType> _targetTypes;

	private float _waitTime;

	public bool CheckSkillConditions(PlayerObject origin, PlayerObject target)
	{
		// Check target

		var targetType = TargetType.None;
		Team teamOrigin = origin._playerPrefab.GetComponent<PlayerState>()._team;
		Team teamTarget = target._playerPrefab.GetComponent<PlayerState>()._team;

		if (origin == target)
		{
			targetType = TargetType.Myself;
			return false;
		} /*
        else if ((teamOrigin == Team.Blue && teamTarget == Team.Blue) ||
                 (teamOrigin == Team.Red && teamTarget == Team.Red))
        {
            targetType = TargetType.Ally;
        }
        else
        {
            targetType = TargetType.Enemy;
        }

        if (_targetTypes.Find(t => t != null && t == targetType) == TargetType.None)
        {
            return false;
        }
        */
		// Check range
		if (_range <= 0)
		{
			return true;
		}

		float distance = Vector3.Distance(origin._playerPrefab.position, target._playerPrefab.position);
		Debug.Log("Distance: " + distance + "max range: " + _range);
		if (distance > _range)
		{
			return false;
		}

		// Checks if the source  is facing the target (Expensive)
		if (_needToBeInFront)
		{
			Vector3 heading = origin._playerPrefab.transform.position - target._playerPrefab.transform.position;
			float dot = Vector3.Dot(heading, origin._playerPrefab.transform.forward);

			if (dot < 0.0f)
			{
				Debug.Log("in front");
			}
			else
			{
				Debug.Log("Not in front");
				return false;
			}
		}

		return true;
	}
}