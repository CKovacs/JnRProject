using UnityEngine;
using System.Collections;

public class GameScore : MonoBehaviour
{
    public Transform _flagRed;
    public Transform _flagBlue;
    public Transform _playerHoldingFlagRed;
    public Transform _playerHoldingFlagBlue;
    public int _flagsCapturedTeamBlue = 0;
    public int _flagsCapturedTeamRed = 0;
}
