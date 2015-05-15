using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {
	
	public GameObject[] weapons; // push your prefabs
	public AudioClip[] sheath;
	private PlayerInput _input;

	[HideInInspector]
	public int currentWeapon = 0, nrWeapons;
	public float delay = 0.1f;

	public int
		up,
		right,
		down,
		left;

	private int[] list = new int[] {0, 1, 2, 3};

	void Awake() {
		_input = GetComponent<PlayerInput> ();

		//hide all but selected weapons
		SwitchWeapon(currentWeapon);
	}

	void Update () {

		//DPAD 360 Controller------------------------------------
		if(_input._controller)
		{
			if(_input._DPadVertical == 1)
			{
				currentWeapon = up;
				StartCoroutine(Sheath(delay));
			}
			else if(_input._DPadVertical == -1)
			{
				currentWeapon = down;
				StartCoroutine(Sheath(delay));
			}
			else if(_input._DPadHorizontal == -1)
			{
				currentWeapon = left;
				StartCoroutine(Sheath(delay));
			}
			else if(_input._DPadHorizontal == 1)
			{
				currentWeapon = right;
				StartCoroutine(Sheath(delay));
			}
		}

		//Press one button to cycle through--------------------------------

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

		/*
		//Mouse Scroll Wheel-----------------------------------
		if(_input._scrollWheel > 0)
		{
			currentWeapon += 1;
			StartCoroutine(Sheath(delay));
		}
		
		if(_input._scrollWheel < 0)
		{
			currentWeapon -= 1;
			StartCoroutine(Sheath(delay));
		}
		*/

		SwitchWeapon(currentWeapon);
	}
	
	public void SwitchWeapon(int index)
	{
		for (int i=0; i < weapons.Length; i++)
		{
			if (i == index)
				weapons[i].gameObject.SetActive(true);
			else 
				weapons[i].gameObject.SetActive(false);
		}
	}

	IEnumerator Sheath(float delay) {
		GetComponent<AudioSource>().clip = sheath[Random.Range (0, sheath.Length)];
		GetComponent<AudioSource>().volume = .3f;
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds (delay);
	}
	
}