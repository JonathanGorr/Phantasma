using UnityEngine;
using System.Collections;

public class WeaponSFX : MonoBehaviour {

	//sounds
	//public AudioClip[] hit;
	public AudioClip[] shieldCollide;
	public AudioClip[] weaponCollide;
	public float delay = 0.1f;

	public void ShieldCollideSound()
	{
		GetComponent<AudioSource>().clip = shieldCollide[Random.Range (0, shieldCollide.Length)];
		GetComponent<AudioSource>().Play();
	}
	public void WeaponCollideSound()
	{
		GetComponent<AudioSource>().clip = weaponCollide[Random.Range (0, weaponCollide.Length)];
		GetComponent<AudioSource>().Play();
	}
}
