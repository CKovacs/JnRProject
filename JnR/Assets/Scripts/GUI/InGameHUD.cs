using UnityEngine;

[RequireComponent(typeof(GUITexture))]

public class InGameHUD : MonoBehaviour
{
	private const float _originalWidth = 1920.0f;
	private const float _originalHeight = 1080.0f;
	private Vector3 _scale;

	public Transform _gameManagementObject;
	private GameManager _gameManager;
	private const int SkillBarOffset = 94;
	private const int SkillIconSize = 64;
	private readonly int _healthbarPositionLeft = Screen.width/2 - 350;
	private readonly int _healthbarPositionTop = Screen.height - 35;
	private readonly int _targetHealthbarPositionLeft = Screen.width / 2 - 350;
	private readonly int _targetHealthbarPositionTop = 35;
	private readonly int _skillBarPositionLeft = Screen.width/2 - 220;
	private readonly int _skillBarPositionTop = Screen.height - 100;
	private int _healthbarHeight = 30;
	private int _healthbarLength = 600;
	private int[] _skillIconPositions;
	private Skill[] _skills;
	public GUIStyle cooldownTimerGUIStyle;
	public Texture2D currentHealthTexture;
	public GUIStyle healthBarGUIStyle;
	public Texture2D healthBarTexture;


	//skillbar stuff
	public Texture2D iconCooldown; //black texture, 50% transparent, .png
	//todo: remove when skills are loaded dynamically
	//todo: also load dynamically
	public Texture2D iconMember1, iconMember2, iconMember3, iconMember4;

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

					GUI.DrawTexture(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize),
						iconCooldown);
					GUI.Label(new Rect(_skillIconPositions[skill._id], _skillBarPositionTop, SkillIconSize, SkillIconSize),
						"" + (int) (skill._cooldownCounter + 1), cooldownTimerGUIStyle);
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
			int j = 0;
			foreach (var player in _gameManager._playerList)
			{
				if (_gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>()._team ==
				    player._playerPrefab.GetComponent<PlayerState>()._team
				    &&
				    _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>().name !=
				    player._playerPrefab.GetComponent<PlayerState>().name)
				{

					GUI.Label(new Rect(0, 40*j, 100, 20), player._playerPrefab.GetComponent<PlayerState>().name);
					GUI.Label(new Rect(0, 40*j + 10, 100, 200),
						player._playerPrefab.GetComponent<PlayerState>()._hp + "/" +
						player._playerPrefab.GetComponent<PlayerState>()._maxHp);
					j++;
				}
			}

			//draw target info
			if (_gameManagementObject.GetComponent<LocalPlayer>()
			._playerPrefab.GetComponent<InputDispatcher>()
			._currentTarget != null &&
				_gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<InputDispatcher>()._currentTarget._networkViewID != _gameManagementObject.GetComponent<LocalPlayer>()._networkViewID )
			{
				GUI.DrawTexture(
					new Rect(_targetHealthbarPositionLeft, _targetHealthbarPositionTop, _healthbarLength, _healthbarHeight),
					healthBarTexture);
				GUI.DrawTexture(
					new Rect(_targetHealthbarPositionLeft + 3, _targetHealthbarPositionTop + 3, GetTargetHealthbarLength(),
						_healthbarHeight - 6),
					currentHealthTexture);

				GUI.Label(new Rect(_targetHealthbarPositionLeft, _targetHealthbarPositionTop, _healthbarLength, _healthbarHeight),
					"" + _gameManagementObject.GetComponent<LocalPlayer>()
				._playerPrefab.GetComponent<InputDispatcher>()
				._currentTarget._playerPrefab.GetComponent<PlayerState>()
				._hp,
					healthBarGUIStyle);
			}
			else
			{
				Debug.Log("No target selected");
			}

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

	private void StartCooldown(int id)
	{
		_skills[id]._onCooldown = true;
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
}