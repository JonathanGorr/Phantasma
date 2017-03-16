using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterController;

public class WeaponController : MonoBehaviour {

	public bool debug = false;
	bool switching = false;
	string currentSceneName;

	public Player _player;
	public CharacterController2D _controller;
	public Animator _anim;

	//overriders
	[Header("Animation Override Controllers")]
	public AnimatorOverrideController bareFistedOverride;
	public AnimatorOverrideController swordOverride;
	public AnimatorOverrideController spearOverride;
	public AnimatorOverrideController bowOverride;

	//called whenever the weapon is switched
	public delegate void WeaponSwitched(Weapon last, Weapon next);
	public event WeaponSwitched onWeaponSwitch;

	public Weapon[] weapons; // push your prefabs

	[HideInInspector] public Weapon weapon;
	private int nrWeapons;
	public float delay = 0.1f;

	void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		currentSceneName = SceneManager.GetActiveScene().name;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode m)
	{
		currentSceneName = scene.name;
	}

	void Start()
	{
		PlayerInput.onPad += Switch;
		StartCoroutine(SwitchWeapon(Weapons.Empty));
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public bool IsWeapon(Weapons w)//returns the weapon enum of the current Weapon
	{
		if(w == weapon.weapon) return true;
		else return false;
	}

	public Weapon GetWeapon(Weapons w)
	{
		for(int i=0;i<weapons.Length;i++)
		{
			if(w == weapons[i].weapon) return weapons[i];
		}
		return null;
	}

	void Switch(PlayerInput.PADDirection direction)
	{
		//in demo scene, no restrictions on switching
		if(currentSceneName != "Demo")
		{
			if(currentSceneName != "Start") return;
			if(PlayerPrefs.GetInt("ItemsGet") == 0) return;
			if(PauseMenu.paused) return;
		}
		if(switching) return;
		if(PlayerInput.Instance.L1Down) { return; }

		switch(direction)
		{
			case PlayerInput.PADDirection.left:
				if(!IsWeapon(Weapons.Empty)) StartCoroutine(SwitchWeapon(Weapons.Empty));
			break;
			case PlayerInput.PADDirection.right:
				if(!IsWeapon(Weapons.SwordShield)) StartCoroutine(SwitchWeapon(Weapons.SwordShield));
			break;
			case PlayerInput.PADDirection.up:
				if(!IsWeapon(Weapons.Bow)) StartCoroutine(SwitchWeapon(Weapons.Bow));
			break;
			case PlayerInput.PADDirection.down:
				if(!IsWeapon(Weapons.Spear)) StartCoroutine(SwitchWeapon(Weapons.Spear));
			break;
		}
	}

	IEnumerator SwitchWeapon(Weapons w)
	{
		switching = true;
		while(_player.combatState == CombatState.Attacking) yield return null;
		while(!_player.ready) yield return null;

		SFX.Instance.PlayFX("sheathe", _player.myTransform.position);

		//hides unmatching, enables matching
		for(int i=0;i<weapons.Length; i++)
		{
			if(i == (int)w)
			{
				weapons[i].gameObject.SetActive(true);
				if(onWeaponSwitch != null) onWeaponSwitch(weapon, weapons[i].GetComponent<Weapon>());
			}
			else
				weapons[i].gameObject.SetActive(false);
		}

		//assign the correct animation override controller
		switch(w)
		{
		case Weapons.Empty:
			_anim.runtimeAnimatorController = bareFistedOverride;
			break;
		case Weapons.SwordShield:
			_anim.runtimeAnimatorController = swordOverride;
			break;
		case Weapons.Spear:
			_anim.runtimeAnimatorController = spearOverride;
			break;
		case Weapons.Bow:
			_anim.runtimeAnimatorController = bowOverride;
			break;
		}

		weapon = GetWeapon(w);

		//wait until buttons depressed ):
		while(PlayerInput._pad)
		{
			yield return null;
		}

		switching = false;
	}
}
