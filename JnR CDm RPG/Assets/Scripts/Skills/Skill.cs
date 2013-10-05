using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Skill : ScriptableObject 
{
    public float _range;
    public int _cooldown;  
    public int _casttime;         // If _castTime == 0   -> skill is instant, otherwise the skill has a cast time
    public int _effectListSize;
    public List<Effect> _effect;
    public List<TargetType> _targetTypes;

    private const string MENUPATH = "JnR/Create/Skill";
    private const string ASSETPATH = "Prefabs/Skills";

    public bool CheckSkillConditions(Target target)
    {
        // Check range
        // Check cooldown
        // Check target

        return true;
    }

    [MenuItem(MENUPATH)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<Skill>(ASSETPATH);
    }
}
