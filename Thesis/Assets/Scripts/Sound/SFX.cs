using UnityEngine;
using System.Collections;

public class SFX : MonoBehaviour {
	
	public AudioClip[] swing;
	public AudioClip[] yell;
	public AudioClip[] hurt;
	public AudioClip[] jump;
	public AudioClip[] collapse;
	public AudioClip[] bowDraw;
	public AudioClip[] bowShoot;

	private Health _health;

	void Awake()
	{
		_health = GetComponent<Health> ();
	}

	void Update()
	{
		if(_health.playerHurt || _health.enemyHurt)
		{
			GetComponent<AudioSource>().pitch = 1f;
			GetComponent<AudioSource>().clip = hurt[Random.Range (0, hurt.Length)];
			GetComponent<AudioSource>().Play();
		}
	}

	public void DeathSound()
	{
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().clip = collapse[Random.Range (0, collapse.Length)];
		GetComponent<AudioSource>().Play();
	}

	public void SwingSoundLight()
	{
		GetComponent<AudioSource>().pitch = 2f;
		GetComponent<AudioSource>().clip = swing[Random.Range (0, swing.Length)];
		GetComponent<AudioSource>().Play();
	}

	public void SwingSoundHeavy()
	{
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().clip = swing[Random.Range (0, swing.Length)];
		GetComponent<AudioSource>().Play();
	}

	public void JumpSound()
	{
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().clip = jump[Random.Range (0, jump.Length)];
		GetComponent<AudioSource>().Play();
	}

	public void BowDraw()
	{
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().clip = bowDraw[Random.Range (0, bowDraw.Length)];
		GetComponent<AudioSource>().Play();
	}

	public void BowShoot()
	{
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().clip = bowShoot[Random.Range (0, bowShoot.Length)];
		GetComponent<AudioSource>().Play();
	}
}
