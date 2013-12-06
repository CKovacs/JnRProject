using UnityEngine;
using System.Collections;

public enum EffectType
{
    damage = 0,
    heal,
    lifesteal,
    run,
    // Stats
    atk,
    def,
    range,
    // Special effects
    stun, 
    reflect, 
    immun, 
    knockback, 
    blink, 
    dodgeball
};

/// <summary>
//	Effect base classs
/// </summary>
[System.Serializable]
public class Effect   
{
    public EffectType _type;
    public float _duration;         // If _time == 0       -> damage is instant, otherwise: DoT or HoT   
    public int _amount;             
    public int _frequency;    // If _frequency == 0  -> isn't DoT or Hot
    public int _percentage;          // Used for speed
    public bool _onHit;
    public bool _onDeath;
}