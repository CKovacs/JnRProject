using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
	public int _dmg = 5;
	public int _forwardBlock = 0;
	public Transform _gameManagementObject;
	public int _maxHp = 100;
	public int _hp = 100;
	public bool _isDead = false;
	public bool _isHoldingAFlag;
	public float _lastPortalTime;
	public int _movementSpeed = 1000;
	public NetworkPlayer _networkPlayer;
	public int _stunCounter = 0;
	public Team _team;
	public string name;
	public bool _teamSelected;
	public bool _spellsSelected;
	public IEnumerable<Skill> Skills;
	//public PlayerObject _target;

	public void SyncValue(int id, int value)
	{
		switch (id)
		{
			case CombatSyncValues.LIFE:
				_hp = value;
				break;
			case CombatSyncValues.DAMAGE:
				//Do something;
				break;
			case CombatSyncValues.BOOLDEATH:
				_isDead = value > 0;
				break;
			case CombatSyncValues.BOOLFLAG:
				_isHoldingAFlag = value > 0;
				break;
			case CombatSyncValues.TEAM:
				if (value == 0)
				{
					_team = Team.Blue;
				}
				else
				{
					_team = Team.Red;
				}
				break;
		}
	}
}