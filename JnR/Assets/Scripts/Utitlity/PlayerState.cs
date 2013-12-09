using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public NetworkPlayer _networkPlayer;
    public int _dmg = 5;
    public Transform _gameManagementObject;
    public int _hp = 100;
    public bool _isDead = false;
    public bool _isHoldingAFlag;
    public int _movementSpeed = 1000;
    public int _stunCounter = 0;
    public int _forwardBlock = 0;
    public string name;
    public Team _team;

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