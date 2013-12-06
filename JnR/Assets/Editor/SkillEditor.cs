using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    private Skill _skill;
	
	private const string ID = "Id";
    private const string DISPLAY = "Display";
    private const string ICON = "Icon";
    private const string EFFECT3D = "3D effect";
    private const string EFFECT3DTYPE = "3D effect type";
    private const string EFFECT3DOFFSETSOURCE = "3D effect source offset";
    private const string EFFECT3DOFFSETTARGET = "3D effect target offset";
    private const string DESTROYTIME = "3D effect destroy time";
    
    private const string RANGE = "Range";
    private const string RESTRICTIONS = "Restrictions";
    private const string COOLDOWN = "Cooldown (seconds)";
    private const string DURATION = "Duration (seconds)";
    private const string AMOUNT = "Amount";
    private const string PERCENTAGE = "Percentage";
    private const string FREQUENCY = "Frequency";

    private const string NTBIF = "Need to be in front";
    private const string NTHIB = "Need to hit in back";
    private const string CLASSRESTRICTION = "Class restriction";
    private const string CLASS = "Class"; 
    private const string ADDCLASS = "Add class";
    private const string DELCLASS = "Delete class"; 
    
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
		_skill._id = EditorGUILayout.IntField(ID, _skill._id);

        // 3d 2d stuff
        EditorGUILayout.LabelField(DISPLAY);
        _skill._icon = EditorGUILayout.ObjectField(ICON, _skill._icon, typeof(Texture2D), true) as Texture2D;
        _skill._3dEffect = EditorGUILayout.ObjectField(EFFECT3D, _skill._3dEffect, typeof(GameObject), true) as GameObject;
        _skill._3dEffectType = (Effect3DType)EditorGUILayout.EnumPopup(EFFECT3DTYPE, _skill._3dEffectType);
        _skill._spellOffSetSource = EditorGUILayout.Vector3Field(EFFECT3DOFFSETSOURCE, _skill._spellOffSetSource);
        _skill._spellOffSetTarget = EditorGUILayout.Vector3Field(EFFECT3DOFFSETTARGET, _skill._spellOffSetTarget);
   
        _skill._destroyTime = EditorGUILayout.FloatField(DESTROYTIME, _skill._destroyTime);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Restrictions

        EditorGUILayout.LabelField(RESTRICTIONS);

        _skill._range = EditorGUILayout.FloatField(RANGE, _skill._range);
        _skill._cooldown = EditorGUILayout.FloatField(COOLDOWN, _skill._cooldown);
        _skill._needToBeInFront = EditorGUILayout.Toggle(NTBIF, _skill._needToBeInFront);
        _skill._needToHitOnBack = EditorGUILayout.Toggle(NTHIB, _skill._needToHitOnBack);

        EditorGUILayout.LabelField(CLASSRESTRICTION);

        for (int i = 0; i < _skill._classSpell.Count; ++i)
        {
            _skill._classSpell[i] = (Class)EditorGUILayout.EnumPopup(CLASS, _skill._classSpell[i]);
            EditorGUILayout.Space();
        }

        if (GUILayout.Button(ADDCLASS))
        {
            _skill._classSpell.Add(new Class());
        }
        if (GUILayout.Button(DELCLASS))
        {
            _skill._classSpell.RemoveAt(_skill._classSpell.Count - 1);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Target

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
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(EFFECTLIST);


        // Spell effects

        for (int i = 0; i < _skill._effect.Count; ++i)
        {
            EditorGUILayout.LabelField(EFFECT + (i + 1));

            Effect effect = _skill._effect[i];

            effect._type = (EffectType)EditorGUILayout.EnumPopup(EFFECTTYPE, effect._type);
            effect._duration = EditorGUILayout.FloatField(DURATION, effect._duration);

            switch (effect._type)
            {
                case EffectType.life:
                    effect._amount = EditorGUILayout.IntField(AMOUNT, effect._amount);
                    if(effect._duration > 0)
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

        EditorUtility.SetDirty(_skill);
    }
}
