using UnityEngine;
using System.Collections;

public enum DynamicEffectType
{
    instant,
    buff,
    frequent
};

public class DynamicEffect
{
	public EffectType _effectType;
	
	public bool _onHit;
	public bool _onDeath;

    public float _duration;
    public float _frequency;  
	public int _amount;
	public int _percentage;

    public DynamicEffectType _dynamicType;
	
	//Housekeeping
    public float _currentDuration;   
    public PlayerObject _target;
    public PlayerObject _source;
	public bool _isResolved = false;

    public DynamicEffect(Effect effect, PlayerObject source, PlayerObject target)
	{
        _duration = _currentDuration = effect._duration;
		_frequency = effect._frequency;
		_amount = effect._amount;
		_percentage = effect._percentage;
		_effectType = effect._type;

        _target = target;
        _source = source;

        if (_currentDuration == 0.0f)
        {
            _dynamicType = DynamicEffectType.instant;
        }
        else if (_currentDuration > 0.0f && _frequency <= 0.0f)
        {
            _dynamicType = DynamicEffectType.buff;
        }
        else if (_currentDuration > 0.0f && _frequency > 0.0f)
        {
            _dynamicType = DynamicEffectType.frequent;
        }
	}
}
