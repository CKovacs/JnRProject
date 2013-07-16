using UnityEngine;
using System.Collections;

public enum EffectType
{
    damage = 0,
    heal,
    stat,
    run,
    stun
};

/// <summary>
//	Effect base classs
/// </summary>
public class Effect : ScriptableObject  
{
    public EffectType _type;
    public float _time;         // If _time == 0       -> damage is instant, otherwise: DoT or HoT   
}

/// <summary>
//	For general damage effects
/// </summary>
public class DamageEffect : Effect 
{
    public float _amount;
    public float _frequency;    // If _frequency == 0  -> isn't DoT or Hot
}

public class HealEffect : Effect
{
    public float _amount;
    public float _frequency;    // If _frequency == 0  -> isn't DoT or Hot
}

/// <summary>
//	Run effect -> buff or debuff
/// </summary>
public class RunEffect : Effect
{
    public float _speed;
}