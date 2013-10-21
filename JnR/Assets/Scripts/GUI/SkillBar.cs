using UnityEngine;

public class SkillBar : MonoBehaviour
{

    public GUIStyle cooldownTimerGUIStyle;
    public GUIStyle healthBarGUIStyle;
    
    

    //skillbar stuff
    public Texture2D iconCooldown; //black texture, 50% transparent, .png
    //todo: remove when skills are loaded dynamically
    public Texture2D spell1, spell2, spell3, spell4;

    private Skill[] _skills;
    private readonly int _skillBarPositionTop = Screen.height - 100;
    private readonly int _skillBarPositionLeft = Screen.width/2 - 220;
    private const int SkillBarOffset = 94;
    private const int SkillIconSize = 64;
    private int[] _skillIconPositions;
   
    //player health bar stuff
    public Texture2D healthBarTexture;
    public Texture2D currentHealthTexture;
    private int _healthbarLength = 600;
    private int _healthbarHeight = 30;
    private int _healthbarPositionLeft = Screen.width / 2 - 350;
    private int _healthbarPositionTop = Screen.height - 35;

    public int playerHp = 100;

	// Use this for initialization
	void Start ()
	{
        _skills = new Skill[4];
	    LoadSkills();
	    _skillIconPositions = GetSkillIconPositions();
	}



    // Update is called once per frame
	void Update () 
    {
	    if (Input.GetKeyDown(KeyCode.Q))
	    {
	        StartCooldown(0);
	    }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCooldown(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCooldown(2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCooldown(3);
        }


        foreach (var skill in _skills)
        {
            if (skill._onCooldown)
            {
                if (skill._cooldownCounter > 0)
                {
                    skill._cooldownCounter -= Time.deltaTime;
                }
                else
                {
                    skill._cooldownCounter = skill._cooldown;
                    skill._onCooldown = false;
                }
            }

        }
	}

    void OnGUI()
    {
       
        //skill bar
        for (int i = 0; i < _skills.Length; i++)
        {
            GUI.DrawTexture(new Rect((_skillBarPositionLeft + i * SkillBarOffset), _skillBarPositionTop, SkillIconSize, SkillIconSize), _skills[i]._icon);
        }

        foreach (var skill in _skills)
        {
            if (skill._onCooldown)
            {
                GUI.DrawTexture(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize), iconCooldown);
                GUI.Label(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize), "" + (int)(skill._cooldownCounter + 1), cooldownTimerGUIStyle);
            }          
        }
        
        //player health bar
        GUI.DrawTexture(new Rect(_healthbarPositionLeft, _healthbarPositionTop, _healthbarLength, _healthbarHeight), healthBarTexture);
        GUI.DrawTexture(new Rect(_healthbarPositionLeft+3, _healthbarPositionTop+3, GetHealthbarLength(), _healthbarHeight-6), currentHealthTexture);
      
        GUI.Label(new Rect(_healthbarPositionLeft, _healthbarPositionTop, _healthbarLength, _healthbarHeight), ""+ playerHp, healthBarGUIStyle);

    }

    void LoadSkills()
    {
        //todo: load skills dynamically from character
        _skills[0] = new Skill
        {
            _id = 0,
            _cooldown = 1.0f,
            _cooldownCounter = 1.0f,
            _icon = spell1
        };
        _skills[1] = new Skill
        {
            _id = 1,
            _cooldown = 2.0f,
            _cooldownCounter = 2.0f,
            _icon = spell2
        };
        _skills[2] = new Skill
        {
            _id = 2,
            _cooldown = 3.0f,
            _cooldownCounter = 3.0f,
            _icon = spell3
        };
        _skills[3] = new Skill
        {
            _id = 3,
            _cooldown = 4.0f,
            _cooldownCounter = 4.0f,
            _icon = spell4
        };
    }

    private int[] GetSkillIconPositions()
    {
        var positions = new int[4];
        for (int i = 0; i < 4; i++)
        {
            positions[i] = _skillBarPositionLeft +  i*SkillBarOffset;
        }
        return positions;
    }

    void StartCooldown(int id)
    {
        _skills[id]._onCooldown = true;
    }

    int GetHealthbarLength()
    {
        var health = (_healthbarLength - 7) * playerHp/100;
        if (health < 0)
        {
            health = 0;
        }
        if (health > _healthbarLength-7)
        {
            health = _healthbarLength - 7;
        }
        return health;
    }
}
