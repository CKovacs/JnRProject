using UnityEngine;
using System.Collections;

public class DynamicEffect
{
	public EffectType _effectType;
	
	public bool _isModifier;
	public bool _isStatusModifier;
	public bool _onHit;
	public bool _onDeath;
	
	public int _duration;       
    public int _frequency;  
	public int _amount;
	public int _percentage;
	
	//Housekeeping
	public float _startTime;
	public NetworkPlayer _target;
	public NetworkPlayer _source;
	public bool _isTriggered;
	public float _lastTriggerTime;
	
	enum ModifierType 
	{
		MOVEMENT_SPEED = 0,
	    ATTACK,
		ATTACKRANGE,
	    DEFENSE
	};
	
	public DynamicEffect(Effect effect)
	{
		Debug.LogError("Implement Me");
	}
	
	public DynamicEffect(Effect effect, NetworkPlayer source, NetworkPlayer target)
	{
		_startTime = Time.time;
		
		_duration = effect._duration;
		_frequency = effect._frequency;
		_amount = effect._amount;
		_percentage = effect._percentage;
		
		_effectType = effect._type;
		
		//Check if we are a modifier spell
		if(_effectType == EffectType.run || _effectType == EffectType.atk || _effectType == EffectType.def || _effectType == EffectType.range)
		{
			_isModifier = true;
		}
		else
		{
			_isModifier = false;
		}
		
		//Check if we are a status modifier
		if(_effectType == EffectType.stun || _effectType == EffectType.reflect || _effectType == EffectType.immun)
		{
			_isStatusModifier = true;
		}
		else
		{
			_isStatusModifier = false;	
		}
		
		_target = target;
		_source = source;
	}
}
