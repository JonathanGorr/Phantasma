using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour {

	public Entity myEntity;
	public WeaponController _weaponController;
	public Animator _anim;

	//anim behaviours
	AttackStateBehaviour attackState;
	BackstepStateBehaviour backstepState;
	RollStateBehaviour rollState;
	SlideAttackStateBehaviour slideAttackState;
	SpecialAttackStateBehaviour specialAttackState;
	private BlockAttackStateBehaviour blockAttackState;

	[Header("Stamina Drain")]
	public float jumpDrain = 1;
	public float rollDrain = 1;
	public float backStepDrain = 1;
	public float blockedHitDrain = 1;
	public float blockDrain = 1;

	public Slider _staminaBar;
	[SerializeField] private float stamina;
	public float maxStamina = 10;
	public float recoverDelay = 1f;
	public float recoverSpeed = 2;
	float t;

	public float MaxStamina
	{
		get { return maxStamina; }
	}

	public float CurrentStamina
	{
		get { return stamina; }
	}

	public bool Ready
	{
		get { return stamina > 0; }
	}

	void OnEnable()
	{
		_staminaBar = GameObject.Find("UI").GetComponent<UI>().MyStaminaBar;
		stamina = maxStamina;
		UpdateStaminaBar();
		StartCoroutine("RecoverStamina");

		//behaviours
		attackState = _anim.GetBehaviour<AttackStateBehaviour>();
		backstepState = _anim.GetBehaviour<BackstepStateBehaviour>();
		rollState = _anim.GetBehaviour<RollStateBehaviour>();
		slideAttackState = _anim.GetBehaviour<SlideAttackStateBehaviour>();
		specialAttackState = _anim.GetBehaviour<SpecialAttackStateBehaviour>();
		blockAttackState = _anim.GetBehaviour<BlockAttackStateBehaviour>();

		//register
		attackState.onEnter += Attack;
		backstepState.onEnter += Backstep;
		rollState.onEnter += Roll;
		slideAttackState.onEnter += SlideAttack;
		specialAttackState.onEnter += SpecialAttack;
		blockAttackState.onEnter += BlockAttack;
	}

	void OnDisable()
	{
		//unregister
		attackState.onEnter -= Attack;
		backstepState.onEnter -= Backstep;
		rollState.onEnter -= Roll;
		slideAttackState.onEnter -= SlideAttack;
		specialAttackState.onEnter -= SpecialAttack;
		blockAttackState.onEnter -= BlockAttack;
	}

	void Attack(){ 			Drain(_weaponController.weapon.regularAttackStaminaDrain); }
	void SpecialAttack(){ 	Drain(_weaponController.weapon.specialAttackStaminaDrain); }
	void Backstep(){ 		Drain(backStepDrain); }
	void Roll(){ 			Drain(rollDrain); }
	void SlideAttack(){ 	Drain(_weaponController.weapon.slideAttackStaminaDrain); }
	void BlockAttack(){ 	Drain(_weaponController.weapon.blockAttackStaminaDrain); }

	public void Drain(float cost)
	{
		if(stamina <= 0) return;
		stamina -= cost;
		t = recoverDelay;
		if(stamina <= 0) t += recoverDelay*1.5f; //punish for using ALL stamina!
		stamina = Mathf.Clamp(stamina, 0, maxStamina);
		UpdateStaminaBar();
	}

	public virtual void UpdateStaminaBar()
	{
		_staminaBar.maxValue = maxStamina;
		_staminaBar.value = stamina;
	}

	//waits x time before recovering stamina
	IEnumerator RecoverStamina()
	{
		while(true)
		{
			//wait until you can recover
			while(t > 0 && myEntity.combatState != CombatState.Blocking)
			{
				t -= Time.deltaTime;
				yield return null;
			}

			t = 0;

			//recover
			if(stamina < maxStamina && t <= 0)
			{
				stamina += Time.deltaTime * (myEntity.combatState == CombatState.Blocking ? recoverSpeed/24 : recoverSpeed);
			}

			UpdateStaminaBar();
			yield return null;
		}
	}
}
