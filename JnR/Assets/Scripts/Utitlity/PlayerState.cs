using UnityEngine;

//This is a Debug Player State;
//For now it is a monobehaviour and uses onSerializeNetworkView
public class PlayerState : MonoBehaviour
{
	public int _dmg = 5;
	public Transform _gameManagementObject;
	public int _hp = 100;
	public bool _isDead = false;
	public bool _isHoldingAFlag;
	public int _movementSpeed = 1000;
    public int _stunCounter = 0;
    public int _forwardBlock = 0;
	public Team _team;

	public string name;
//	public SelectedTeam team;

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
		}
	}
}