using UnityEngine;

//This is a Debug Player State;
//For now it is a monobehaviour and uses onSerializeNetworkView
public class PlayerState : MonoBehaviour
{
    public int _hp = 100;
	public int _dmg = 5;
	public int _movementSpeed = 1000;

    public string name;
    public SelectedTeam team;
    public Team _team;

	public bool _isHoldingAFlag;

	[RPC]
	private void SyncValue(int id, int value)
	{
		switch(id)
		{
		case PlayerStateSyncValues.LIFE:
			_hp = value;
			break;
		case PlayerStateSyncValues.DAMAGE:
			//Do something;
			break;
		}
	}
}