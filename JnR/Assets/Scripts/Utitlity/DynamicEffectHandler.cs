using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEffectHandler
{
    private Dictionary<NetworkPlayer, List<DynamicEffect>> _effectList;
    private const int SIZE = 10;

    public DynamicEffectHandler()
    {
        if (Network.isServer)
        {
            //Get Memory for the list
            _effectList = new Dictionary<NetworkPlayer, List<DynamicEffect>>(SIZE);
        }
    }

    public void Update(float time)
    {
        bool removeMe = false;

        foreach (KeyValuePair<NetworkPlayer, List<DynamicEffect>> pair in _effectList)
        {
            foreach (DynamicEffect e in pair.Value)
            {
                //Check if the spell should be removed
                removeMe = RemoveCheck(e);
                if (removeMe)
                {
                    Debug.Log("Removed a Spell");
                    pair.Value.Remove(e);
                }
                //Resolve the Spell with remove or not remove information
                Resolve(pair.Key, e, removeMe);
            }
        }
    }

    private bool RemoveCheck(DynamicEffect e)
    {
        float currentTime = Time.time;

        if (currentTime - e._startTime < 0)
        {
            //REMOVE THIS SPELL
            return true;
        }
        //DONT REMOVE THIS SPELL
        return false;
    }

    private void Resolve(NetworkPlayer player, DynamicEffect e, bool wasRemoved)
    {
        //Should be outside?
        float currentTime = Time.time;

        //Instant
        if (e._duration == 0)
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

    public void AddEffectsForPlayer(NetworkPlayer player, List<Effect> effect)
    {
        foreach (Effect e in effect)
        {
            DynamicEffect de = new DynamicEffect(e);
            _effectList[player].Add(de);
        }
    }
}
