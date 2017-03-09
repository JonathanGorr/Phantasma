using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack state behaviour:
/// Used to interface with the attack state of any entity deriving from the player animator controller.
/// Referenced by entity derived scripts especially enemies to turn off movement and facing while in this attack state
/// </summary>

public class AnimStateBehaviour : StateMachineBehaviour {

	public delegate void OnEnter();
	public event OnEnter onEnter;

	public delegate void OnExit();
	public event OnEnter onExit;

	public bool inState = false;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		inState = true;
		if(onEnter!=null) onEnter();
	}
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		inState = false;
		if(onExit!=null) onExit();
	}
}
