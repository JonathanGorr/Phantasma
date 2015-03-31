using UnityEngine;
using System.Collections;

public class SwooshTest : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField]
	AnimationClip _animation;
#pragma warning restore 0649
	AnimationState _animationState;
	
	[SerializeField]
	int _start = 0;
	
	[SerializeField]
	int _end = 0;
	
	float _startN = 0.0f;
	float _endN = 0.0f;
	
	float _time = 0.0f;
	float _prevTime = 0.0f;
	float _prevAnimTime = 0.0f;
#pragma warning disable 0649
	[SerializeField]
	MeleeWeaponTrail _trail;
#pragma warning restore 0649
	bool _firstFrame = true;
	
	void Start()
	{
		float frames = _animation.frameRate * _animation.length;
		_startN = _start/frames;
		_endN = _end/frames;
		_animationState = animation[_animation.name];
		_trail.Emit = false;
	}
	
	void Update()
	{
		_time += _animationState.normalizedTime - _prevAnimTime;
		if (_time > 1.0f || _firstFrame)
		{
			if (!_firstFrame)
			{
				_time -= 1.0f;
			}
			_firstFrame = false;
		}
		
		if (_prevTime < _startN && _time >= _startN)
		{
			_trail.Emit = true;
		}
		else if (_prevTime < _endN && _time >= _endN)
		{
			_trail.Emit = false;
		}
		
		_prevTime = _time;
		_prevAnimTime = _animationState.normalizedTime;
	}
}
