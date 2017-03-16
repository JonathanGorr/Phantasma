using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stagger: the point at which an enemy's animation is interupted briefly due to a hit or after multiple hits

/// Stagger meter:
/// Used to determine, after how many hits, an entity will stagger.
/// Hits from weaker weapons, that have a smaller "stagger" value,
/// will not immediately stagger like hits from large weapons
/// But progressive hits over time will achieve a stagger.

/// Heavier enemies will have a higher stagger threshold;
/// will require a heavier weapon to stagger or more hits to stagger
/// </summary>

public class StaggerMeter : MonoBehaviour {

	public Animator anim;
	public Health health;
	public Entity myEntity;

	bool staggered = false;
	[SerializeField] private float stagger = 0;
	public float staggerThreshold = 5;
	private float staggerRecoverySpeed = 1;
	private float staggerRecoverDelay = 1; //how long to wait before restoring stagger

	StaggerStateBehaviour staggerState;

	float t;

	void Awake()
	{
		StartCoroutine(CoolStagger());
		staggerState = anim.GetBehaviour<StaggerStateBehaviour>();
		staggerState.onExit += OnStaggerExit;
	}

	void OnDisable()
	{
		staggerState.onExit -= OnStaggerExit;
	}

	//no longer staggered on animation finish
	void OnStaggerExit()
	{
		staggered = false;
	}

	//waits x time before recovering stamina
	IEnumerator CoolStagger()
	{
		while(true)
		{
			//wait until you can recover
			while(t > 0)
			{
				t -= Time.deltaTime;
				yield return null;
			}

			//recover if wait time (t) = 0
			t = 0;
			if(stagger > 0)
				stagger -= Time.deltaTime * staggerRecoverySpeed;
			else
				stagger = 0;
			yield return null;
		}
	}

	//adds the stagger amount of a weapon to stagger amount on hit
	public void AddStagger(float amt)
	{
		if(health.Dead) return; // don't stagger if dead
		if(staggered) return; //return if already at threshold

		//reduce amount of stagger if blocking
		if(myEntity.combatState == CombatState.Blocking) amt /= 2;

		stagger += amt;

		//stagger if at or exceeding stagger threshold
		if(stagger >= staggerThreshold)
		{
			staggered = true;
			stagger = 0; //reset stagger
			t = 0; //reset waittime
			anim.SetTrigger("Stagger");
			return;
		}

		t = staggerRecoverDelay;
	}
}
