using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Skill : ScriptableObject 
{
    public float _range;
    public float _cooldown;  
    public float _casttime;         // If _castTime == 0   -> skill is instant, otherwise the skill has a cast time
    public List<Effect> _effect;
    public List<TargetType> _type;

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
