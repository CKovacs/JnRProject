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
    public List<Effect> _effect;
    public Texture2D _icon;
    public int _id;
    public bool _needToBeInFront;
    public bool _needToHitOnBack;
    public bool _onCooldown;
    public float _range;
    public Vector3 _spellOffSetSource;
    public Vector3 _spellOffSetTarget;
    public List<TargetType> _targetTypes;

    private float _waitTime;

    public bool CheckSkillConditions(Target origin, Target target, float deltaTime)
    {
        // Check target

        var targetType = TargetType.None;

        if (origin._id == target._id)
        {
            targetType = TargetType.Myself;
        }
        else if ((origin._team == Team.Blue && target._team == Team.Blue) ||
                 (origin._team == Team.Red && target._team == Team.Red))
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

        // Check cooldown
        if (_waitTime != 0)
        {
            _waitTime = + deltaTime;

            if (_waitTime < _cooldown)
            {
                return false;
            }
        }


        // Check range
        float distance = Vector3.Distance(origin._3dData.transform.position, target._3dData.transform.position);

        if (distance > _range)
        {
            return false;
        }

        return true;
    }
}