using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill : ScriptableObject 
{
    // Display stuff
    public Texture2D _icon;
    public GameObject _3dEffect;
    public Effect3DType _3dEffectType;

    public float _range;
    public float _cooldown;  
    public int _casttime;         // If _castTime == 0   -> skill is instant, otherwise the skill has a cast time
    public bool _needToBeInFront;
    public bool _needToHitOnBack;
    public bool _closedCombatOnly;
    public bool _rangedCombatOnly;
    public Class _classSpell;
    public List<Effect> _effect;
    public List<TargetType> _targetTypes;

    private float _waitTime = 0;

    public bool CheckSkillConditions(Target origin, Target target, float deltaTime)
    {
        // Check target
        
        TargetType targetType = TargetType.None;

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
            _waitTime =+ deltaTime;

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
