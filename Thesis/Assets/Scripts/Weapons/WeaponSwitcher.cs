using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {
	
	public GameObject[] weapons; // push your prefabs
	public AudioClip[] sheath;
	private LevelManager _manager;

	[HideInInspector]
	public int currentWeapon = 0, nrWeapons;
	public float delay = 0.1f;
	
	public bool weaponGet;

	public bool inMenu;

	public int
		up,
		right,
		down,
		left;

	void Awake() {
		_manager = GetComponent<LevelManager>();
		nrWeapons = weapons.Length;
		SwitchWeapon(currentWeapon); // Set default gun
	}

	void Update () {
		if(weaponGet && !inMenu)
		{
			if(!_manager.paused)
			{
				//up
				if(Input.GetAxis ("360_VerticalDPAD") == 1)
				{
					currentWeapon = up;
					SwitchWeapon(currentWeapon);
					StartCoroutine(Sheath(delay));
				}
				else if(Input.GetAxis ("360_VerticalDPAD") == -1)
				{
					currentWeapon = down;
					SwitchWeapon(currentWeapon);
					StartCoroutine(Sheath(delay));
				}
				else if(Input.GetAxis ("360_HorizontalDPAD") == -1)
				{
					currentWeapon = left;
					SwitchWeapon(currentWeapon);
					StartCoroutine(Sheath(delay));
				}
				else if(Input.GetAxis ("360_HorizontalDPAD") == 1)
				{
					currentWeapon = right;
					SwitchWeapon(currentWeapon);
					StartCoroutine(Sheath(delay));
				}

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
			}
		}
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