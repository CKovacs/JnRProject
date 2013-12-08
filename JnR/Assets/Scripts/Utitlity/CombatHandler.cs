using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatHandler
{
    private Dictionary<PlayerObject, List<DynamicEffect>> _effectDictionary;
    private NetworkView _nv;
    private const int SIZE = 10;

    public CombatHandler(NetworkView nv, List<PlayerObject> pl)
    {
        _nv = nv;

        //Get Memory for the list
        _effectDictionary = new Dictionary<PlayerObject, List<DynamicEffect>>(SIZE);
        Debug.Log("player size " + pl.Count);
        foreach (PlayerObject po in pl)
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
        List<DynamicEffect> removeList = new List<DynamicEffect>();

        foreach (KeyValuePair<PlayerObject, List<DynamicEffect>> pair in _effectDictionary)
        {
            PlayerObject player = pair.Key;

            foreach (DynamicEffect e in pair.Value)
            {
                // Resolve the effect, if needed
                if (!e._isResolved)
                {
                    ResolveEffect(player, e);
                }

                e._currentDuration = -timeDelta;

                Debug.Log("Duration: " + e._currentDuration);

                //Check if the spell should be removed
                if (RemoveCheck(e))
                {
                    Debug.Log("Spell added to remove list");
                    removeList.Add(e);
                }
            }

            // Remove tyme
            foreach (DynamicEffect e in removeList)
            {
                pair.Value.Remove(e);
                //RemoveEffect(p, e);
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

    private void ResolveEffect(PlayerObject player, DynamicEffect effect)
    {
        // Instant
        if (effect._dynamicType == DynamicEffectType.instant || effect._dynamicType == DynamicEffectType.buff)
        {
            // RPC call (Dynamic effect isn't a supported type, so you have to send the members)
            _nv.RPC("SC_DoEffect", RPCMode.All, player._networkPlayer, (int)effect._effectType, effect._amount, effect._percentage);

            effect._isResolved = true;
            return; //Nothing more should happen
        }
        // Frequently effect
        else if (effect._dynamicType == DynamicEffectType.frequent)
        {
            if(effect._currentDuration > (effect._duration - effect._frequency))
            {
                // RPC call (Dynamic effect isn't a supported type, so you have to send the members)
                _nv.RPC("SC_DoEffect", RPCMode.All, player._networkPlayer, (int)effect._effectType, effect._amount, effect._percentage);

                if (effect._currentDuration <= 0.0f)
                {
                    effect._isResolved = true;
                }
                else 
                {
                    // Change the duration for the next tick
                    effect._duration = effect._duration - effect._frequency;
                }
            }

            return; //Nothing more should happen
        }
    }
}
