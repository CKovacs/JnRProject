using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEffectHandler
{
    private Dictionary<PlayerObject, List<DynamicEffect>> _effectDictionary;
    private NetworkView _nv;
    private const int SIZE = 10;

    public DynamicEffectHandler(NetworkView nv, List<PlayerObject> pl)
    {
        _nv = nv;

        //Get Memory for the list
        _effectDictionary = new Dictionary<PlayerObject, List<DynamicEffect>>(SIZE);
        Debug.Log("player size " + pl.Count);
        foreach(PlayerObject po in pl)
        {
            _effectDictionary.Add(po, new List<DynamicEffect>(SIZE));   
        }
    }

    public void AddEffectsForPlayer(PlayerObject source, PlayerObject target, List<Effect> effect)
    {
        foreach (Effect e in effect)
        {
            DynamicEffect de = new DynamicEffect(e, source, target);
            _effectDictionary[target].Add(de);
        }
    }

    public void Update(float timeDelta)
    {
        bool removeMe = false;
        List<DynamicEffect> removeList = new List<DynamicEffect>();

        foreach (KeyValuePair<PlayerObject, List<DynamicEffect>> pair in _effectDictionary)
        {
            PlayerObject p = pair.Key;

            foreach (DynamicEffect e in pair.Value)
            {
                ResolveEffect(p, e, removeMe);

                e._currentDuration =- timeDelta;

                Debug.Log("Duration: " + e._currentDuration);
                //Check if the spell should be removed
                removeMe = RemoveCheck(e);

                if (removeMe)
                {
                    Debug.Log("Spell added to remove list");
                    removeList.Add(e);
                }
            }

            // Remove tyme
            foreach (DynamicEffect e in removeList)
            {
                pair.Value.Remove(e);
                RemoveEffect(p, e);
            }
        }
    }

    private bool RemoveCheck(DynamicEffect e)
    {
        if (e._currentDuration < 0)
        {
            //REMOVE THIS SPELL
            return true;
        }
        //DONT REMOVE THIS SPELL
        return false;
    }

    private void ResolveEffect(PlayerObject player, DynamicEffect e, bool wasRemoved)
    {
        //Instant
        if (e._currentDuration == 0)
        {
            if (!e._isTriggered)
            {
                // RPC call (Dynamic effect isn't a supported type, so you have to send the members)
                _nv.RPC("SC_AddEffect", RPCMode.All, player._networkPlayer, (int) e._effectType, e._amount, e._percentage);

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
                _nv.RPC("SC_AddEffect", RPCMode.All, player._networkPlayer, (int)e._effectType, e._amount, e._percentage);
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
}
