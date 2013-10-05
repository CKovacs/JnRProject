using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    private Skill _skill;

    private const string RANGE = "Range";
    private const string COOLDOWN = "Cooldown";
    private const string AMOUNT = "Amount";
    private const string EFFECT = "Effect ";
    private const string EFFECTLIST = "Effect list";
    private const string EFFECTTYPE = "Effect type";
    private const string TARGET = "Target list";

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

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(EFFECTLIST);

        for (int i = 0; i < _skill._effectListSize; ++i)
        {
            EditorGUILayout.LabelField(EFFECT + (i + 1));

            Effect effect = _skill._effect[i];

            effect._type = (EffectType)EditorGUILayout.EnumPopup(EFFECTTYPE, effect._type);

            switch (effect._type)
            {
                case EffectType.damage:
                    effect._amount = EditorGUILayout.IntField(AMOUNT, effect._amount);
                    break;
            }
        }

        if (GUILayout.Button(ADDEFFECT))
        {
            //_skill._effect.Add(CreateInstance(typeof(Effect)) as Effect);
            _skill._effectListSize++;
            _skill._effect.Add(new Effect());
        }
        if (GUILayout.Button(DELEFFECT))
        {
            _skill._effectListSize--;
            _skill._effect.RemoveAt(_skill._effect.Count - 1);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(TARGET);

        if (GUILayout.Button(ADDEFFECT))
        {
            _skill._targetTypes.Add(new TargetType());
        }
        if (GUILayout.Button(DELEFFECT))
        {
            _skill._effect.RemoveAt(_skill._effect.Count - 1);
        }

        EditorGUILayout.Space();
        EditorUtility.SetDirty(_skill);
    }
}
