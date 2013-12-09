using UnityEngine;
using System.Collections;

public class GameScore : MonoBehaviour {
	//Todo make a "win-condition" script
	public Transform 	_flagRed;
	public Transform 	_flagBlue;
	public Transform 	_playerHoldingFlagRed;
	public Transform 	_playerHoldingFlagBlue;
	
	public int 			_flagCapturedTeamBlue = 0;
	public int 			_flagCapturedTeamRed = 0;
}
