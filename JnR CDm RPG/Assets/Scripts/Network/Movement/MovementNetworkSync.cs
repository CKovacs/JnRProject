using UnityEngine;
using System.Collections;

public class MovementNetworkSync : MonoBehaviour 
{
	public Movement _movementScript;
	
	public double _interpolationBackTime = 0.1; //In Seconds
	public double _extrapolationLimit = 0.5; //In Seconds
	
	private bool _isMine = false;
	
	internal struct State
	{
		internal double _timeStamp;
		internal Vector3 _position;
		//internal Vector3 _velocity;
		internal Quaternion _rotation;
	}
	
	//Wir speichern uns 20 States bei einer Sendrate von 15 
	private State[] _bufferedState = new State[20];
	private State[] _localBufState = new State[20];
	private int _timeStampCount;
	private int _localTimeStampCount;
	
	private float _timeAccuracy = 0;
	private float _predictionAccuracy = 0;
	private bool _fixPredictionError = false;
	private Vector3 _newPosition = Vector3.zero;
	
	// The position vector distance to start error correction. The higher the latency the higher this
	// value should be or it constantly tries to correct errors in prediction, of course this depends
	// on the game too.
	public float _predictionThreshold = 0.25f;
	// Time difference in milliseconds where we check for error in position. If the server time value
	// of a state is too different from the local state time then the error correction comparison is
	// highly unreliable and you might try to correct more errors than there really are.
	public float _timeThreshold = 0.05f;
	
	public float _posCorrectionSpeed = 5.0F;
	public float _maxPositionErrorTime = 1.0F;
	
	float _positionErrorTime = 0.0F;
	
	//DEBUG GUI
	Rect _connInfo = new Rect( Screen.width-170, 40, 160, 50 );
	
	float _timer = Time.time + 1;
	int _msgCounter = 0;
	int _msgRate = 0;
	double _msgLatencyTotal = 0;
	double _msgLatency = 0;
	//DEBUG GUI
	
	void Start()
	{
		_movementScript = GetComponent<Movement>();
	}
	
	void SetOwnership()
	{
		Debug.Log("Setting ownership for local player");
		_isMine = true;
	}
	
	
	
	void OnGUI()
	{
		if ( _isMine )
		{
			_connInfo = GUILayout.Window( 0, _connInfo, MakeConnInfoWindow, "Local Player" );
		}
	}
	
	void MakeConnInfoWindow(int windowID)
	{
		GUILayout.Label(string.Format("{0} msg/s {1,4:f3} ms", _msgRate, _msgLatency));
		GUILayout.Label(string.Format("Time Difference : {0,3:f3}", _timeAccuracy));
		GUILayout.Label(string.Format("Prediction Difference : {0,3:f3}", _predictionAccuracy));
		if (Time.time - _timer > 0) {
			_msgRate = _msgCounter;
			_timer = Time.time + 1;
			_msgCounter = 0;
			if (_msgRate != 0) {
				_msgLatency = (_msgLatencyTotal/(double)_msgRate)*1000F;
			} else {
				_msgLatency = 0;
			}
			_msgLatencyTotal = 0;
		}
	}
	
	IEnumerator MonitorLocalMovement()
	{
		while (true)
		{
			yield return new WaitForSeconds(1/15);
			
			for( int i = _localBufState.Length - 1; i >= 1; i-- )
			{
				_localBufState[i] = _localBufState[i-1];
			}
			
			State state;
			state._timeStamp = Network.time;
			state._position = transform.position;
			state._rotation = transform.rotation;
			_localBufState[0] = state;
			
			_localTimeStampCount = Mathf.Min(_localTimeStampCount + 1, _localBufState.Length);
			
			//
			// Check if the client side prediction has an error
			//
			
			// Find the local buffered state which is closest to network state in time
			int j = 0;
			bool match = false;
			for( j = 0; j < _localTimeStampCount - 1; j++ )
			{
				if( _bufferedState[0]._timeStamp <= _localBufState[j]._timeStamp &&
					_localBufState[j]._timeStamp - _bufferedState[0]._timeStamp <= _timeThreshold )
				{
					_timeAccuracy = Mathf.Abs((float)_localBufState[j]._timeStamp -(float)_bufferedState[0]._timeStamp);
					_predictionAccuracy = (Vector3.Distance(_localBufState[j]._position,_bufferedState[0]._position));
					match = true;
					break;
				}
			}
			if (!match)
			{ 
				//Debug.Log("No match!");
			}
			//If prediction is off, move back toward last known good location.
			else if (_predictionAccuracy > _predictionThreshold)
			{
				// Find how far we travelled since the prediction failed
				Vector3 localMovement = _localBufState[j]._position - _localBufState[0]._position;
				
				// "Erase" old values in the local buffer
				_localTimeStampCount = 1;

				// New position which we need to converge to in the update loop				
				_newPosition = _bufferedState[0]._position + localMovement;

				// Trigger the new position convergence routine				
				_fixPredictionError = true;
			}
			else
			{
				_fixPredictionError = false;
			}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			//Vector3 vel = rigidbody.velocity;

			stream.Serialize(ref pos);
			//stream.Serialize(ref vel);
			stream.Serialize(ref rot);
		}
		// Read data from remote client
		else
		{
			//DEBUG GUI
			_msgCounter++;
			_msgLatencyTotal += (Network.time-info.timestamp );
			//DEBUG GUI
			
			Vector3 pos = Vector3.zero;
			//Vector3 vel = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			stream.Serialize(ref pos);
			//stream.Serialize(ref vel);
			stream.Serialize(ref rot);
			
			// Shift the buffer sideways, deleting state 20
			for (int i=_bufferedState.Length-1;i>=1;i--)
			{
				_bufferedState[i] = _bufferedState[i-1];
			}
			
			// Record current state in slot 0
			State state;
			state._timeStamp = info.timestamp;
			state._position = pos;	
			//state._velocity = vel;
			state._rotation = rot;
			_bufferedState[0] = state;
			
			// Update used slot count, however never exceed the buffer size
			// Slots aren't actually freed so this just makes sure the buffer is
			// filled up and that uninitalized slots aren't used.
			_timeStampCount = Mathf.Min(_timeStampCount + 1, _bufferedState.Length);

			// Check if states are in order, if it is inconsistent you could reshuffel or 
			// drop the out-of-order state. Nothing is done here
			for (int i=0;i<_timeStampCount-1;i++)
			{
				if (_bufferedState[i]._timeStamp < _bufferedState[i+1]._timeStamp)
					Debug.Log("State inconsistent");
			}	
		}
	}
	
	// We have a window of interpolationBackTime where we basically play 
	// By having interpolationBackTime the average ping, you will usually use interpolation.
	// And only if no more data arrives we will use extra polation
	void Update () {
		// This is the target playback time of the rigid body
		double currentTime = Network.time;
		double interpolationTime = currentTime - _interpolationBackTime;
			
		if( _isMine && !_fixPredictionError )
		{
			_positionErrorTime = 0.0F;
		}
		
		if(_isMine && _fixPredictionError)
		{
			_positionErrorTime += Time.deltaTime;
			if(_positionErrorTime > _maxPositionErrorTime)
			{
				transform.position = _newPosition;	
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, _newPosition,Time.deltaTime * _posCorrectionSpeed);	
			}
		}
		else if(_isMine && !_movementScript._isMoving)
		{
			transform.position = Vector3.Lerp (_bufferedState[0]._position, transform.position, 0.95f);
		}
		else if(Network.isClient && !_isMine)
		{
			// Use interpolation if the target playback time is present in the buffer
			if (_bufferedState[0]._timeStamp > interpolationTime)
			{
				// Go through buffer and find correct state to play back
				for (int i=0;i<_timeStampCount;i++)
				{
					if (_bufferedState[i]._timeStamp <= interpolationTime || i == _timeStampCount-1)
					{
						// The state one slot newer (<100ms) than the best playback state
						State rhs = _bufferedState[Mathf.Max(i-1, 0)];
						// The best playback state (closest to 100 ms old (default time))
						State lhs = _bufferedState[i];
						
						// Use the time between the two slots to determine if interpolation is necessary
						double length = rhs._timeStamp - lhs._timeStamp;
						float t = 0.0F;
						// As the time difference gets closer to 100 ms t gets closer to 1 in 
						// which case rhs is only used
						// Example:
						// Time is 10.000, so sampleTime is 9.900 
						// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
						// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
						if (length > 0.0001){
							t = (float)((interpolationTime - lhs._timeStamp) / length);
						}
						//	Debug.Log(t);
						// if t=0 => lhs is used directly
						transform.localPosition = Vector3.Lerp(lhs._position, rhs._position, t);
						transform.localRotation = Quaternion.Slerp(lhs._rotation, rhs._rotation, t);
						return;
					}
				}
			}
			// Use extrapolation
			else
			{
				State latest = _bufferedState[0];
				
				transform.localPosition = latest._position;
				transform.localRotation = latest._rotation;
				//State latest = _bufferedState[0];
				//
				//float extrapolationLength = (float)(interpolationTime - latest._timeStamp);
				//// Don't extrapolation for more than 500 ms, you would need to do that carefully
				//if (extrapolationLength < _extrapolationLimit)
				//{
				//	transform.position = latest._position;// + latest._velocity * extrapolationLength;
				//	transform.rotation = latest._rotation;
				//
				//	//rigidbody.velocity = latest._velocity;
				//}
			}
		}	
	}
}	
		
		/*
			// Use interpolation if the target playback time is present in the buffer
			if (_bufferedState[0]._timeStamp > interpolationTime)
			{
				// Go through buffer and find correct state to play back
				for (int i=0;i<_timeStampCount;i++)
				{
					if (_bufferedState[i]._timeStamp <= interpolationTime || i == _timeStampCount-1)
					{
						// The state one slot newer (<100ms) than the best playback state
						State rhs = _bufferedState[Mathf.Max(i-1, 0)];
						// The best playback state (closest to 100 ms old (default time))
						State lhs = _bufferedState[i];
						
						// Use the time between the two slots to determine if interpolation is necessary
						double length = rhs._timeStamp - lhs._timeStamp;
						float t = 0.0F;
						// As the time difference gets closer to 100 ms t gets closer to 1 in 
						// which case rhs is only used
						// Example:
						// Time is 10.000, so sampleTime is 9.900 
						// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
						// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
						if (length > 0.0001){
							t = (float)((interpolationTime - lhs._timeStamp) / length);
						}
						//	Debug.Log(t);
						// if t=0 => lhs is used directly
						transform.localPosition = Vector3.Lerp(lhs._position, rhs._position, t);
						transform.localRotation = Quaternion.Slerp(lhs._rotation, rhs._rotation, t);
						return;
					}
				}
			}
			// Use extrapolation
			else
			{
				State latest = _bufferedState[0];
				
				float extrapolationLength = (float)(interpolationTime - latest._timeStamp);
				// Don't extrapolation for more than 500 ms, you would need to do that carefully
				if (extrapolationLength < _extrapolationLimit)
				{
					transform.position = latest._position;// + latest._velocity * extrapolationLength;
					transform.rotation = latest._rotation;
				
					//rigidbody.velocity = latest._velocity;
				}
			}
		//}
		//else /*ismine
		//{
		//	State latest = _bufferedState[0];
		//	transform.position = Vector3.Lerp (transform.position,latest._position,0.9f);
		//	transform.rotation = Quaternion.Lerp (transform.rotation,latest._rotation,0.9f);
		//}
	}
}*/
