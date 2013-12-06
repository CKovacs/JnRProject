using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEffectHandler
{
    private Dictionary<NetworkPlayer, List<DynamicEffect>> _effectDictionary;
    private const int SIZE = 10;

    public DynamicEffectHandler(List<PlayerObject> playerList)
    {
        //Get Memory for the list
        _effectDictionary = new Dictionary<NetworkPlayer, List<DynamicEffect>>(SIZE);
        Debug.Log("player size " + playerList.Count);
        foreach(PlayerObject po in playerList)
        {
            _effectDictionary.Add(po._networkPlayer, new List<DynamicEffect>(SIZE));   
        }
    }

    public void Update(float timeDelta)
    {
        bool removeMe = false;

        foreach (KeyValuePair<NetworkPlayer, List<DynamicEffect>> pair in _effectDictionary)
        {
            foreach (DynamicEffect e in pair.Value)
            {
                Resolve(pair.Key, e, removeMe);

                e._currentDuration =- timeDelta; 
                //Check if the spell should be removed
                removeMe = RemoveCheck(e);

                if (removeMe)
                {
                    Debug.Log("Removed a Spell");
                    pair.Value.Remove(e);
                }
            }
        }
    }

    private bool RemoveCheck(DynamicEffect e)
    {
        if (e._startTime < 0)
        {
            //REMOVE THIS SPELL
            return true;
        }
        //DONT REMOVE THIS SPELL
        return false;
    }

    private void Resolve(NetworkPlayer player, DynamicEffect e, bool wasRemoved)
    {
        //Instant
        if (e._currentDuration == 0)
        {
            if (!e._isTriggered)
            {
                Debug.Log("INSTANT ATTACK/HEAL OR SOMETHING..");
                e._isTriggered = true;
                e._lastTriggerTime = Time.time;
                return; //Nothing more should happen
            }
        }

        //Modifier Spells
        if (e._isModifier)
        {
            //Check if Spell is triggered or trigger it..
            if (!e._isTriggered)
            {
                Debug.Log("APPLY SOME MODIFIER!");
                e._isTriggered = true;
                e._lastTriggerTime = Time.time;
                return;
                //Nothing more should happen since we are a modifier and there is no modifier with duration equals 0
            }

            if (wasRemoved)
            {
                Debug.Log("REMOVE SOME MODIFIER!");
                return; //Nothing more should happen since we just removed a modifier
            }
        }

        //Those spells are mostly croud control
        if (e._isStatusModifier)
        {
            //Check if Spell is triggered or trigger it..
            if (!e._isTriggered)
            {
                Debug.Log("APPLY SOME STATUS MODIFIER!");
                e._isTriggered = true;
                e._lastTriggerTime = Time.time;
                return;
                //Nothing more should happen since we are a modifier and there is no modifier with duration equals 0
            }

            if (wasRemoved)
            {
                Debug.Log("REMOVE SOME STATUS MODIFIER!");
            }
        }
    }

    public void AddEffectsForPlayer(NetworkPlayer source, NetworkPlayer target, List<Effect> effect)
    {
        foreach (Effect e in effect)
        {
            Debug.Log(e._type);
            DynamicEffect de = new DynamicEffect(e, source, target);
            _effectDictionary[target].Add(de);
        }
    }
}
