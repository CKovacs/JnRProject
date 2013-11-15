using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputDispatcher : MonoBehaviour 
{
	public Transform _gameManagementObject;
	public float _targetMaxRange = 25.0f;
	public GameObject _targetRingPrefab;
	public Skill _skill;
	
	private GameManager _gameManager;
	private PlayerObject _myself, _currentTarget;
	private GameObject _targetRingInstance;

    private const string LEFTSELECT = "LeftSelect";
    private const string RIGHTSELECT = "RightSelect";
    private const string SELFSELECT = "SelfSelect";

    // Standard attacks
    private const string AUTOHIT = "AutoHit";
    private const string LR = "LR";

	void Start()
	{
		_targetList = new List<PlayerObject>();
		_gameManager = _gameManagementObject.GetComponent<GameManager>();	
		_myself = _currentTarget = _gameManager.GetPlayerObject(_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		_targetRingInstance = GameObject.Instantiate(_targetRingPrefab) as GameObject;
	}
	
	// Update is called once per frame
	void Update () 
		//TODO Add Global Cooldown
	{
		UpdateTargetList();

        if (Input.GetButtonDown(LEFTSELECT))
		{
			_currentTarget = GetTarget(false);
			Debug.Log ("GETTARGET " + _currentTarget._networkPlayer);
			
			_targetRingInstance.transform.position = _currentTarget._playerPrefab.transform.position;
			_targetRingInstance.transform.parent = _currentTarget._playerPrefab.transform;
		}
		
		if(Input.GetButtonDown(RIGHTSELECT))
		{
			_currentTarget = GetTarget(true);
			Debug.Log ("GETTARGET " + _currentTarget._networkPlayer);
			//_gameManagementObject.networkView.RPC ("RemoteAttack",RPCMode.Server,
			//	_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
			//	1);
			_targetRingInstance.transform.position = _currentTarget._playerPrefab.transform.position;
			_targetRingInstance.transform.parent = _currentTarget._playerPrefab.transform;
		}
		
		if(Input.GetButtonDown(AUTOHIT))
		{
			if(_currentTarget != _myself)
			{
				//der verwendete Skill wird an den Server gesendet und ein Effect wird abgebildet
				//RemoteSkillUse ist "generisch"
				Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);
				_gameManagementObject.networkView.RPC ("S_RemoteSkillUse",RPCMode.Server,
					_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
					_currentTarget._networkPlayer,_skill._id);
			}
		}
        if (Input.GetButtonDown(SELFSELECT))
        {
            if (_currentTarget != _myself)
            {
                Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);
                _gameManagementObject.networkView.RPC("S_RemoteAttack", RPCMode.Server,
                    _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
                    _currentTarget._networkPlayer);
            }
        }
        if (Input.GetButtonDown(SELFSELECT))
        {
            if (_currentTarget != _myself)
            {
                Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);
                _gameManagementObject.networkView.RPC("S_RemoteAttack", RPCMode.Server,
                    _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
                    _currentTarget._networkPlayer);
            }
        }
        //Debug.Log(Input.GetAxis(DODGEBALL));
        if (Input.GetAxis(LR) == 1)
        {
            Debug.Log("DodgeBall");
            if (_currentTarget != _myself)
            {
                Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);
                _gameManagementObject.networkView.RPC("S_RemoteAttack", RPCMode.Server,
                    _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
                    _currentTarget._networkPlayer);
            }
        }
        if (Input.GetAxis(LR) == -1)
        {
            Debug.Log("Shield");
            if (_currentTarget != _myself)
            {
                Effect3DBuilder.DoEffect(_myself._playerPrefab.transform, _currentTarget._playerPrefab.transform, _skill);
                _gameManagementObject.networkView.RPC("S_RemoteAttack", RPCMode.Server,
                    _gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer,
                    _currentTarget._networkPlayer);
            }
        }
		
		if(Input.GetKeyDown(KeyCode.T))
		{
            Debug.Log("Keycode.T");
			_gameManagementObject.networkView.RPC ("S_ResetPositionToSpawnpoint",RPCMode.Server,
				_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		}


        //TeamSelection
	    if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 pressed");
            _gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, "Herbert", 0);
	    }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2 pressed");
            _gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, "Herbert", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3 pressed");
            _gameManagementObject.networkView.RPC("AddPlayerToTeam", RPCMode.Server, "Herbert", 2);
        }
	}
	
	private List<PlayerObject> _targetList = null;
	
	public void UpdateTargetList()
	{
		_targetList.Clear();
		
		PlayerObject localPlayer = _gameManager.GetPlayerObject(_gameManagementObject.GetComponent<LocalPlayer>()._networkPlayer);
		foreach(PlayerObject po in _gameManager._playerList)
		{
			if(po == _currentTarget || po == localPlayer)
			{
				continue;
			}
			
			else if(Vector3.Distance(localPlayer._playerPrefab.transform.position, po._playerPrefab.transform.position) < _targetMaxRange)
			{
				_targetList.Add(po);
				//Debug.Log(po._networkPlayer);
			}
			
			//Debug.Log("NetworkPlayer == " + po._networkPlayer);
		}
	}
	
	public PlayerObject GetTarget(bool isRight)
	{
		SortedList<float,PlayerObject> sortedList = new SortedList<float,PlayerObject>();
		
		foreach(PlayerObject po in _targetList)
		{
			sortedList.Add (po._playerPrefab.transform.position.x,po);
		}
		
		float currentTargetX = _currentTarget._playerPrefab.transform.position.x;
		
		foreach(KeyValuePair<float,PlayerObject> kpo in sortedList)
		{	
			PlayerObject po = kpo.Value;
			if(isRight)
			{
				if(currentTargetX < kpo.Key)
				{
					_currentTarget = po;	
					break;
				}
			}
			else
			{
				if(currentTargetX >= kpo.Key)
				{
					_currentTarget = po;	
					break;
				}
			}
		}
		
		return _currentTarget;
	
		
		
		//float distance = 0.0f;
		//PlayerObject currentPivot = null;
		//foreach(PlayerObject po in _targetList)
		//{
		//	Vector3 relativePoint = _currentTarget._playerPrefab.transform.InverseTransformPoint(po._playerPrefab.transform.position);
		//	if(isRight && relativePoint.x > distance) 
		//	{
		//		distance = relativePoint.x;
		//		currentPivot = po;
		//	} 
		//	else if(!isRight && relativePoint.x <= distance)
		//	{
		//		distance = relativePoint.x;
		//		currentPivot = po;
		//	}
		//}
		//return currentPivot;
	}
}
