using System.Collections.Generic;
using UnityEngine;

public class InputDispatcher : MonoBehaviour
{
	private const string LEFTSELECT = "LeftSelect";
	private const string RIGHTSELECT = "RightSelect";
	private const string SELFSELECT = "SelfSelect";

	// Standard attacks
	private const string AUTOHIT = "AutoHit";
	private const string LR = "LR";

	private const string SKILL13 = "Skill13";
	private const string SKILL24 = "Skill24";
	public AnimationHandle _animHandle;
	public CooldownHandler _cooldownHandle;
	private PlayerObject _currentTarget;
	public Transform _gameManagementObject;
	private GameManager _gameManager;
	private float _horizontalInput;
	private bool _isBlocking = false;
	private bool _isRunning;
	private bool _jumpInput;
	private float _lastHorizontalInput;
	private float _lastVerticalInput;
	public Movement _movementScript;
	private PlayerObject _myself;

	public Skill _skillAutohit;
	public Skill _skillBlock;
	public Skill _skillDefence;
	public Skill _skillDodgeRoll;

	public Skill _skillStandard;
	public Skill _skillUltimate;
	public Skill _skillUtility;
	private List<PlayerObject> _targetList;
	public float _targetMaxRange = 25.0f;

	private GameObject _targetRingInstance;
	public GameObject _targetRingPrefab;


	//MovementHousekeeping
	private float _verticalInput;

	private void Start()
	{
		_targetList = new List<PlayerObject>();
		_gameManager = _gameManagementObject.GetComponent<GameManager>();
		_myself =
			_currentTarget = _gameManager.GetPlayerObject(_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		_targetRingInstance = Instantiate(_targetRingPrefab) as GameObject;
		_cooldownHandle = new CooldownHandler();
	}

	private void Update()
	{
		//Network Movement Input Dispatcher
		_verticalInput = Input.GetAxis("Vertical");
		_horizontalInput = Input.GetAxis("Horizontal");
		_jumpInput = _movementScript._didJump;
		_movementScript._didJump = false;

		if (_verticalInput != _lastVerticalInput || _horizontalInput != _lastHorizontalInput || _jumpInput)
		{
			if (!Network.isServer)
			{
				if (_isRunning)
				{
					_animHandle.IdleRun(false);
				}
				else
				{
					_isRunning = true;
					_animHandle.IdleRun(true);
				}
				//Send Movement Input to Server
				networkView.RPC("S_SendUserInput", RPCMode.Server, _verticalInput, _horizontalInput, (!_jumpInput) ? 0 : 1);
			}
		}
		if (_verticalInput == 0 && _horizontalInput == 0)
		{
			if (!_isRunning)
			{
				_animHandle.IdleReady(false);
			}
			else
			{
				_isRunning = false;
				_animHandle.IdleReady(true);
			}
		}

		//Send Jump Animation if needed
		if (_jumpInput)
		{
			_animHandle.Jump(true);
		}

		_lastVerticalInput = _verticalInput;
		_lastHorizontalInput = _horizontalInput;

		UpdateTargetList();

		if (Input.GetButtonDown(LEFTSELECT) || Input.GetKeyDown(KeyCode.Q))
		{
			_currentTarget = GetTarget(false);
			//Debug.Log("GETTARGET " + _currentTarget._networkPlayer);
			var newPos = new Vector3(_currentTarget._playerPrefab.transform.position.x,
				_currentTarget._playerPrefab.transform.position.y - 1, _currentTarget._playerPrefab.transform.position.z);

			_targetRingInstance.transform.position = newPos;
			_targetRingInstance.transform.parent = _currentTarget._playerPrefab.transform;
		}

		if (Input.GetButtonDown(RIGHTSELECT) || Input.GetKeyDown(KeyCode.E))
		{
			_currentTarget = GetTarget(true);
			//Debug.Log("GETTARGET " + _currentTarget._networkPlayer);

			var newPos = new Vector3(_currentTarget._playerPrefab.transform.position.x,
				_currentTarget._playerPrefab.transform.position.y - 1, _currentTarget._playerPrefab.transform.position.z);

			_targetRingInstance.transform.position = newPos;
			_targetRingInstance.transform.parent = _currentTarget._playerPrefab.transform;
		}
		if (Input.GetButtonDown(SELFSELECT) || Input.GetKeyDown(KeyCode.C))
		{
			_currentTarget = _myself;
		}

		if ((Input.GetButtonDown(AUTOHIT) || Input.GetKeyDown(KeyCode.F)) &&
		    _skillAutohit.CheckSkillConditions(_myself, _currentTarget)
		    && !_cooldownHandle.HasCooldown(AUTOHIT))
		{
			//der verwendete Skill wird an den Server gesendet und ein Effect wird abgebildet
			//RemoteSkillUse ist "generisch"

			_animHandle.OneHandHit(true);

			Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skillAutohit);
			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_currentTarget._networkPlayer, _skillAutohit._id);

			_cooldownHandle.AddCooldown(AUTOHIT, _skillAutohit._cooldown);
		}
		/*
        if (Input.GetAxis(LR) == 1 || Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("DodgeBall");

            _gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
            _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
            _currentTarget._networkPlayer, _skillDodgeRoll._id);
        }*/
		if ((Input.GetAxis(LR) == -1 || Input.GetKeyDown(KeyCode.X)) && !_cooldownHandle.HasCooldown(LR))
		{
			Debug.Log("Block");

			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_myself._networkPlayer, _skillBlock._id);

			_cooldownHandle.AddCooldown(LR, _skillBlock._cooldown);
		}

		// Teh 4 skills
		if ((Input.GetAxis(SKILL13) == 1 || Input.GetKeyDown(KeyCode.I)) &&
		    _skillStandard.CheckSkillConditions(_myself, _currentTarget)
		    && !_cooldownHandle.HasCooldown(SKILL13 + "1"))
		{
			Debug.Log("Skill 1" + _skillStandard.name);
			_animHandle.OneHandHit(true);

			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_currentTarget._networkPlayer, _skillStandard._id);

			_cooldownHandle.AddCooldown(SKILL13 + "1", _skillStandard._cooldown);
		}
		else if ((Input.GetAxis(SKILL24) == 1 || Input.GetKeyDown(KeyCode.O)) &&
		         _skillDefence.CheckSkillConditions(_myself, _currentTarget)
		         && !_cooldownHandle.HasCooldown(SKILL24 + "1"))
		{
			Debug.Log("Skill 2" + _skillDefence.name);
			_animHandle.OneHandHit(true);

			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_currentTarget._networkPlayer, _skillDefence._id);

			_cooldownHandle.AddCooldown(SKILL24 + "1", _skillDefence._cooldown);
		}
		else if ((Input.GetAxis(SKILL13) == -1 || Input.GetKeyDown(KeyCode.K)) &&
		         _skillUtility.CheckSkillConditions(_myself, _currentTarget)
		         && !_cooldownHandle.HasCooldown(SKILL13 + "2"))
		{
			Debug.Log("Skill 3" + _skillUtility.name);
			_animHandle.OneHandHit(true);

			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_currentTarget._networkPlayer, _skillUtility._id);

			_cooldownHandle.AddCooldown(SKILL13 + "2", _skillUtility._cooldown);
		}
		else if ((Input.GetAxis(SKILL24) == -1 || Input.GetKeyDown(KeyCode.L)) &&
		         _skillUltimate.CheckSkillConditions(_myself, _currentTarget)
		         && !_cooldownHandle.HasCooldown(SKILL24 + "2"))
		{
			Debug.Log("Skill 4" + _skillUltimate.name);
			_animHandle.OneHandHit(true);

			_gameManagementObject.networkView.RPC("S_RemoteSkillUse", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
				_currentTarget._networkPlayer, _skillUltimate._id);

			_cooldownHandle.AddCooldown(SKILL24 + "2", _skillUltimate._cooldown);
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			Debug.Log("Keycode.T");
			_gameManagementObject.networkView.RPC("S_ResetPositionToSpawnpoint", RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		}


		//TeamSelection
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			_gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>().name, 0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{

			_gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>().name, 1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{

			_gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, _gameManagementObject.GetComponent<LocalPlayer>()._playerPrefab.GetComponent<PlayerState>().name, 2);
		}
		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			Debug.Log("ID: selection done");
			_gameManagementObject.networkView.RPC("S_SetTeamSelected", RPCMode.AllBuffered, _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		}
		_cooldownHandle.UpdateCooldowns(Time.deltaTime);
	}

	public void UpdateTargetList()
	{
		_targetList.Clear();

		PlayerObject localPlayer =
			_gameManager.GetPlayerObject(_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		foreach (PlayerObject po in _gameManager._playerList)
		{
			if (po == _currentTarget || po == localPlayer)
			{
			}
			if (Vector3.Distance(localPlayer._playerPrefab.transform.position, po._playerPrefab.transform.position) <
			    _targetMaxRange)
			{
				_targetList.Add(po);
				//Debug.Log(po._networkPlayer);
			}

			//Debug.Log("NetworkPlayer == " + po._networkPlayer);
		}
	}

	public PlayerObject GetTarget(bool isRight)
	{
		var sortedList = new SortedList<float, PlayerObject>();

		foreach (PlayerObject po in _targetList)
		{
			sortedList.Add(po._playerPrefab.transform.position.x, po);
		}

		float currentTargetX = _currentTarget._playerPrefab.transform.position.x;

		foreach (var kpo in sortedList)
		{
			PlayerObject po = kpo.Value;
			if (isRight)
			{
				if (currentTargetX < kpo.Key)
				{
					_currentTarget = po;
					break;
				}
			}
			else
			{
				if (currentTargetX >= kpo.Key)
				{
					_currentTarget = po;
					break;
				}
			}
		}

		return _currentTarget;
	}

	[RPC]
	private void S_SendUserInput(float vInput, float hInput, int jInput)
	{
		_movementScript._horizontalInput = hInput;
		_movementScript._verticalInput = vInput;
		if (jInput == 1)
		{
			_movementScript._hasUnsyncedJump = true;
		}
	}
}