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
[System.Serializable]
public class Effect   
{
    public EffectType _type;
    public int _duration;         // If _time == 0       -> damage is instant, otherwise: DoT or HoT   
    public int _amount;
    public int _frequency;    // If _frequency == 0  -> isn't DoT or Hot
    public int _speed;
}