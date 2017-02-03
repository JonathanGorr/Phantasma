using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordShield : Weapon {

	public override void OnEnable()
	{
		base.OnEnable();
	}

	public override void Update()
	{
		base.Update();
		//set blocking bool
		if(_entity.combatState == CombatState.Blocking) 
		{
			_entity._anim.SetBool("Blocking", true);
		}
		else 
			_entity._anim.SetBool("Blocking", false);

		//Blocking and Strong Attack
		if(_entity._controller.isGrounded)
		{
			if(_entity.combatState == CombatState.Blocking)
			{
				_entity.SetSpeed (_entity.blockSpeed);
			}
			else if(_entity.combatState == CombatState.Attacking)
			{
				_entity.SetSpeed (_entity.blockSpeed);
			}
			else
			{
				_entity.SetSpeed (_entity.walkSpeed);
			}
		}
	}
}
