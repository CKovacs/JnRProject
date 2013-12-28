using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CooldownHandler
{
    private Dictionary<string, float> _cooldownDictionary;

    public CooldownHandler()
    {
        _cooldownDictionary = new Dictionary<string, float>();
    }

    public void UpdateCooldowns(float deltaTime)
    {
        Dictionary<string, float> newList = new Dictionary<string, float>();

        foreach (KeyValuePair<string, float> pair in _cooldownDictionary)
        {
            float newValue = pair.Value - deltaTime;
            //Debug.Log("Cooldown value " + newValue);
            if(newValue > 0.0f)
            {
                newList.Add(pair.Key, newValue);
            }
        }

        _cooldownDictionary = newList;
    }

    public void AddCooldown(string key, float cooldown)
    {
        _cooldownDictionary.Add(key, cooldown);
    }

    public bool HasCooldown(string cooldown)
    {
        if (_cooldownDictionary.ContainsKey(cooldown))
        {
            return true;
        }

        return false;
    }
}
