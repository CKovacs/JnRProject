using System;
using UnityEngine;


public class InGameHUD : MonoBehaviour
{
	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	private Vector3 _scale;

	public Transform _gameManagementObject;
	private GameManager _gameManager;
	private const int SkillBarOffset = 94;
	private const int SkillIconSize = 64;
	private readonly int _healthbarPositionLeft = (int) /*Screen.width*/_originalWidth/2 - 350;
    private readonly int _healthbarPositionTop =  (int)/*Screen.height*/_originalHeight - 35;
    private readonly int _targetHealthbarPositionLeft =  (int) /*Screen.width*/_originalWidth / 2 - 350;
    private readonly int _targetHealthbarPositionTop = (int) 35;
    private readonly int _skillBarPositionLeft =  (int) /*Screen.width*/_originalWidth/2 - 220;
    private readonly int _skillBarPositionTop = (int)/*Screen.height*/_originalHeight - 100;
	private int _healthbarHeight = 30;
	private int _healthbarLength = 600;
	private int[] _skillIconPositions;
	private Skill[] _skills;
	public GUIStyle cooldownTimerGUIStyle;
	public Texture2D currentHealthTexture;
	public GUIStyle healthBarGUIStyle;
	public Texture2D healthBarTexture;
	
	//score
	public GUIStyle scoreBlueGuiStyle;
	public Texture2D scoreBlueTexture;
	public GUIStyle scoreRedGuiStyle ;
	public Texture2D scoreRedTexture;

	public Texture2D flagSymbolBlue;
	public Texture2D flagSymbolRed;
	public Texture2D flagBlueGreen;
	public Texture2D flagBlueYellow;
	public Texture2D flagBlueRed;
	public Texture2D flagRedGreen;
	public Texture2D flagRedYellow;
	public Texture2D flagRedRed;
	//skillbar stuff
	public Texture2D iconCooldown; 

	

	public int playerHp = 100;
	public Texture2D spell1, spell2, spell3, spell4;

	// Use this for initialization
	private void Start()
	{
		_gameManager = _gameManagementObject.GetComponent<GameManager>();
		_skills = new Skill[4];
		LoadSkills();
		_skillIconPositions = GetSkillIconPositions();
	}


	// Update is called once per frame
	private void Update()
	{
		
		foreach (Skill skill in _skills)
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

		SetFlagIcon();
	}

	private void OnGUI()
	{
		if (Network.isClient)
		{
			//scaling stuff for different resolutions
			float rx = Screen.width/_originalWidth;
			float ry = Screen.height/_originalHeight;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

			//skill bar
			for (int i = 0; i < _skills.Length; i++)
			{

				GUI.DrawTexture(
					new Rect((_skillBarPositionLeft + i*SkillBarOffset), _skillBarPositionTop, SkillIconSize, SkillIconSize),
					_skills[i]._icon);
			}

			foreach (Skill skill in _skills)
			{
				if (skill._onCooldown)
				{
					GUI.Label(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize),
						"" + (int) (skill._cooldownCounter + 1), cooldownTimerGUIStyle);
					GUI.DrawTexture(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize),
						iconCooldown);
					
				}
			}

			//player health bar
			GUI.DrawTexture(new Rect(_healthbarPositionLeft, _healthbarPositionTop, _healthbarLength, _healthbarHeight),
				healthBarTexture);
			GUI.DrawTexture(
				new Rect(_healthbarPositionLeft + 3, _healthbarPositionTop + 3, GetHealthbarLength(), _healthbarHeight - 6),
				currentHealthTexture);

			//GUI.Label(new Rect(_healthbarPositionLeft, _healthbarPositionTop, _healthbarLength, _healthbarHeight), "" + 
			//	GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>()._target._playerPrefab.GetComponent<PlayerState>()._hp,
			//	healthBarGUIStyle);


			//GUI.DrawTexture(new Rect(0, 80, SkillIconSize, SkillIconSize), iconMember1);
			//GUI.DrawTexture(new Rect(0, 160, SkillIconSize, SkillIconSize), iconMember2);
			//GUI.DrawTexture(new Rect(0, 240, SkillIconSize, SkillIconSize), iconMember3);
			//GUI.DrawTexture(new Rect(0, 320, SkillIconSize, SkillIconSize), iconMember4);

			//team members
			//int j = 0;
			//foreach (var player in _gameManager._playerList)
			//{
			//	if (_gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>()._team ==
			//		player._playerPrefab.GetComponent<PlayerState>()._team
			//		&&
			//		_gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>().name !=
			//		player._playerPrefab.GetComponent<PlayerState>().name)
			//	{

			//		GUI.Label(new Rect(0, 40*j, 100, 20), player._playerPrefab.GetComponent<PlayerState>().name);
			//		GUI.Label(new Rect(0, 40*j + 10, 100, 200),
			//			player._playerPrefab.GetComponent<PlayerState>()._hp + "/" +
			//			player._playerPrefab.GetComponent<PlayerState>()._maxHp);
			//		j++;
			//	}
			//}

			//draw target info
			//if (_gameManagementObject.GetComponent<LocalPlayer>()
			//._playerPrefab.GetComponent<InputDispatcher>()
			//._currentTarget != null &&
			//	_gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<InputDispatcher>()._currentTarget._networkViewID != _gameManagementObject.GetComponent<LocalPlayer>()._networkViewID )
			//{
			//	GUI.DrawTexture(
			//		new Rect(_targetHealthbarPositionLeft, _targetHealthbarPositionTop, _healthbarLength, _healthbarHeight),
			//		healthBarTexture);
			//	GUI.DrawTexture(
			//		new Rect(_targetHealthbarPositionLeft + 3, _targetHealthbarPositionTop + 3, GetTargetHealthbarLength(),
			//			_healthbarHeight - 6),
			//		currentHealthTexture);

			//	GUI.Label(new Rect(_targetHealthbarPositionLeft, _targetHealthbarPositionTop, _healthbarLength, _healthbarHeight),
			//		"" + _gameManagementObject.GetComponent<LocalPlayer>()
			//	._playerPrefab.GetComponent<InputDispatcher>()
			//	._currentTarget._playerPrefab.GetComponent<PlayerState>()
			//	._hp,
			//		healthBarGUIStyle);
			//}
			//else
			//{
			//	//Debug.Log("No target selected");
			//}


			//Score
			int center = Convert.ToInt32(_originalWidth/2);
			GUI.Label(new Rect(center - 160, 5, 60, 60), flagSymbolBlue);
			GUI.Box(new Rect(center - 100, 10, 90, 50), _gameManager._gameScore._flagsCapturedTeamBlue.ToString(), scoreBlueGuiStyle);

			GUI.Label(new Rect(center + 200, 5, 60, 60), flagSymbolRed);
			GUI.Box(new Rect(center + 100, 10, 90, 50), _gameManager._gameScore._flagsCapturedTeamRed.ToString(), scoreRedGuiStyle);
		}
	}

	private void LoadSkills()
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
			positions[i] = _skillBarPositionLeft + i*SkillBarOffset;
		}
		return positions;
	}

	public void StartCooldown(int id, float time)
	{
		_skills[id]._onCooldown = true;
		_skills[id]._cooldown = _skills[id]._cooldownCounter = time;
	}

	private int GetHealthbarLength()
	{
		int hp = _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>()._hp;
		int health = (_healthbarLength - 7)*hp/100;
		if (health < 0)
		{
			health = 0;
		}
		if (health > _healthbarLength - 7)
		{
			health = _healthbarLength - 7;
		}
		return health;
	}

	private int GetTargetHealthbarLength()
	{
		int hp = _gameManagementObject.GetComponent<LocalPlayer>()
			._playerPrefab.GetComponent<InputDispatcher>()
			._currentTarget._playerPrefab.GetComponent<PlayerState>()
			._hp;
		//_gameManagementObject.GetComponent<LocalPlayer>()
			//	._playerPrefab.GetComponent<PlayerState>()
			//	._target._playerPrefab.GetComponent<PlayerState>()
			//	._hp;

		Debug.Log("target-hp: " + hp);
		int health = (_healthbarLength - 7) * hp / 100;
		if (health < 0)
		{
			health = 0;
		}
		if (health > _healthbarLength - 7)
		{
			health = _healthbarLength - 7;
		}
		return health;
	}

	private void SetFlagIcon()
	{
		if (_gameManager._gameScore._playerHoldingFlagBlue == null)
		{
			flagSymbolBlue = flagBlueGreen;
		}
		else
		{
			flagSymbolBlue = flagBlueRed;
		}

		if (_gameManager._gameScore._playerHoldingFlagRed == null)
		{
			flagSymbolRed = flagRedGreen;
		}
		else
		{
			flagSymbolRed = flagRedRed;
		}
	}
}