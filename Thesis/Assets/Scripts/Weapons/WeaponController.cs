using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterController;

public class WeaponController : MonoBehaviour {

	public bool debug = false;
	bool switching = false;
	string currentSceneName;
	private LevelManager _manager;
	private PlayerInput _input;
	private SFX _sfx;
	private PlayerPreferences _prefs;

	public Player _player;
	public CharacterController2D _controller;
	public Animator _anim;

	//overriders
	[Header("Animation Override Controllers")]
	public AnimatorOverrideController bareFistedOverride;
	public AnimatorOverrideController swordOverride;
	public AnimatorOverrideController spearOverride;
	public AnimatorOverrideController bowOverride;

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
		_manager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
		_sfx   =  _manager.GetComponent<SFX>();
		_input =  _manager.GetComponent<PlayerInput>();
		_input.onPad += Switch;
		_prefs =  _manager.GetComponent<PlayerPreferences>();
		StartCoroutine(SwitchWeapon(Weapons.Empty));
	}

	void OnDisable()
	{
		//SceneManager.sceneLoaded -= OnSceneLoaded;
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
		//return if items not received yet
		if(currentSceneName != "Start") return;
		if(!_prefs.itemsGet && !debug) return;
		if(_manager.paused) return;
		if(switching) return;

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
		while(!_player._ready) yield return null;
		//while(_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;
		weapon = GetWeapon(w);
		while(switching)
		{
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

			_sfx.PlayFX("sheathe", _player.myTransform.position);

			//hides unmatching, enables matching
			for(int i=0;i<weapons.Length; i++)
			{
				if(i == (int)w)
					weapons[i].gameObject.SetActive(true);
				else
					weapons[i].gameObject.SetActive(false);
			}

			//wait until buttons depressed ):
			while(_input._DPadHorizontal != 0 || _input._DPadVertical != 0)
			{
				yield return null;
			}

			switching = false;
		}
	}
}
