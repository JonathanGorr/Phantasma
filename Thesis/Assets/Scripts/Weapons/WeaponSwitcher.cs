using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {
	
	public GameObject[] weapons; // push your prefabs
	public AudioClip[] sheath;
	private LevelManager _manager;
	private PlayerInput _input;

	[HideInInspector]
	public int currentWeapon = 0, nrWeapons;
	public float delay = 0.1f;
	private int[] list = {0,1,2,3};
	
	public bool weaponGet, inMenu;

	public int
		up,
		right,
		down,
		left;

	void Awake() {
		_input = GetComponent<PlayerInput> ();
		_manager = GetComponent<LevelManager>();
		nrWeapons = weapons.Length;
		SwitchWeapon(currentWeapon); // Set default weapon
	}

	void Update () {

		if(_input.DPadVertical == 1)
		{
			currentWeapon = up;
			StartCoroutine(Sheath(delay));
		}
		else if(_input.DPadVertical == -1)
		{
			currentWeapon = down;
			StartCoroutine(Sheath(delay));
		}
		else if(_input.DPadHorizontal == -1)
		{
			currentWeapon = left;
			StartCoroutine(Sheath(delay));
		}
		else if(_input.DPadHorizontal == 1)
		{
			currentWeapon = right;
			StartCoroutine(Sheath(delay));
		}

		if (currentWeapon > list.Length - 1) {
			currentWeapon = 0;
		}
		else if(currentWeapon < 0)
		{
			currentWeapon = list.Length - 1;
		}

		if(_input._cycleWep)
		{
			currentWeapon += 1;
		}
		
		if(_input._scrollWheel > 0)
		{
			currentWeapon += 1;
		}
		
		if(_input._scrollWheel < 0)
		{
			currentWeapon -= 1;
		}

		SwitchWeapon(currentWeapon);

		/*
		//keys
		for (int i=1; i <= nrWeapons; i++)
		{ 
			//number key correlates to weapon
			if (Input.GetKeyDown("" + i)) {
				currentWeapon = i-1;
				
				SwitchWeapon(currentWeapon);
				StartCoroutine(Sheath(delay));
			}
		}
		*/
	}
	
	public void SwitchWeapon(int index)
	{
		for (int i=0; i < nrWeapons; i++)
		{
			if (i == index) {
				weapons[i].gameObject.SetActive(true);
			} else { 
				weapons[i].gameObject.SetActive(false);
			}
		}
	}

	IEnumerator Sheath(float delay) {
		GetComponent<AudioSource>().clip = sheath[Random.Range (0, sheath.Length)];
		GetComponent<AudioSource>().volume = .3f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (delay);
	}
	
}