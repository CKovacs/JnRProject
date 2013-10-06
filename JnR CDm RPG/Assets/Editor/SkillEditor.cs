using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    private Skill _skill;

    private const string RANGE = "Range";
    private const string COOLDOWN = "Cooldown (seconds)";
    private const string DURATION = "Duration (seconds)";
    private const string AMOUNT = "Amount";
    private const string PERCENTAGE = "Percentage";
    private const string FREQUENCY = "Frequency";

    private const string NTBIF = "Need to be in front";
    private const string NTHIB = "Need to hit in back";
    private const string CLOSEDCOMBAT = "Closed combat only";
    private const string RANGEDCOMBAT = "Ranged combat only"; 
    private const string ONDEATH = "On death";
    private const string ONHIT = "On hit";

    private const string EFFECT = "Effect ";
    private const string EFFECTLIST = "Effect list";
    private const string EFFECTTYPE = "Effect type";
    private const string TARGET = "Target list";
    private const string TARGETTYPE = "Target type";

    // Buttons
    private const string ADDEFFECT = "Add skill effect";
    private const string DELEFFECT = "Delete skill effect";
    private const string ADDTARGET = "Add skill target";
    private const string DELTARGET = "Delete skill target";

    public void Awake()
    {
        _skill = target as Skill;
    }

    public override void OnInspectorGUI()
    {
        /*
        public List<TargetType> _type;*/
        _skill._range = EditorGUILayout.FloatField(RANGE, _skill._range);
        _skill._cooldown = EditorGUILayout.IntField(COOLDOWN, _skill._cooldown);
        _skill._needToBeInFront = EditorGUILayout.Toggle(NTBIF, _skill._needToBeInFront);
        _skill._needToHitOnBack = EditorGUILayout.Toggle(NTHIB, _skill._needToHitOnBack);

        _skill._closedCombatOnly = EditorGUILayout.Toggle(CLOSEDCOMBAT, _skill._closedCombatOnly);
        _skill._rangedCombatOnly = EditorGUILayout.Toggle(RANGEDCOMBAT, _skill._rangedCombatOnly);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(EFFECTLIST);

        for (int i = 0; i < _skill._effect.Count; ++i)
        {
            EditorGUILayout.LabelField(EFFECT + (i + 1));

            Effect effect = _skill._effect[i];

            effect._type = (EffectType)EditorGUILayout.EnumPopup(EFFECTTYPE, effect._type);
            effect._duration = EditorGUILayout.IntField(DURATION, effect._duration);

            switch (effect._type)
            {
                case EffectType.damage:
                    effect._amount = EditorGUILayout.IntField(AMOUNT, effect._amount);
                    if(effect._duration > 0)
                        effect._frequency = EditorGUILayout.IntField(FREQUENCY, effect._frequency);
                    break;
                case EffectType.heal:
                    effect._amount = EditorGUILayout.IntField(AMOUNT, effect._amount);
                    if (effect._duration > 0)
                        effect._frequency = EditorGUILayout.IntField(FREQUENCY, effect._frequency);
                    break;
                case EffectType.run:
                    effect._percentage = EditorGUILayout.IntField(PERCENTAGE, effect._percentage);
                    break;
                case EffectType.atk:
                    effect._percentage = EditorGUILayout.IntField(PERCENTAGE, effect._percentage);
                    break;
                case EffectType.def:
                    effect._percentage = EditorGUILayout.IntField(PERCENTAGE, effect._percentage);
                    break;
                case EffectType.range:
                    effect._percentage = EditorGUILayout.IntField(PERCENTAGE, effect._percentage);
                    break;
            }

            effect._onDeath = EditorGUILayout.Toggle(ONDEATH, effect._onDeath);
            effect._onHit = EditorGUILayout.Toggle(ONHIT, effect._onHit);

            EditorGUILayout.Space();
        }

        if (GUILayout.Button(ADDEFFECT))
        {
            _skill._effect.Add(new Effect());
        }
        if (GUILayout.Button(DELEFFECT))
        {
            _skill._effect.RemoveAt(_skill._effect.Count - 1);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(TARGET);

        for (int i = 0; i < _skill._targetTypes.Count; ++i)
        {
            _skill._targetTypes[i] = (TargetType)EditorGUILayout.EnumPopup(TARGETTYPE, _skill._targetTypes[i]);
            EditorGUILayout.Space();
        }

        if (GUILayout.Button(ADDTARGET))
        {
            _skill._targetTypes.Add(new TargetType());
        }
        if (GUILayout.Button(DELTARGET))
        {
            _skill._targetTypes.RemoveAt(_skill._targetTypes.Count - 1);
        }

        EditorGUILayout.Space();
        EditorUtility.SetDirty(_skill);
    }
}
