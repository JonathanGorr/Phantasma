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
		audio.clip = shieldCollide[Random.Range (0, shieldCollide.Length)];
		audio.Play();
	}
	public void WeaponCollideSound()
	{
		audio.clip = weaponCollide[Random.Range (0, weaponCollide.Length)];
		audio.Play();
	}
}
