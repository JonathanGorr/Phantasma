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
			audio.pitch = 1f;
			audio.clip = hurt[Random.Range (0, hurt.Length)];
			audio.Play();
		}
	}

	public void DeathSound()
	{
		audio.pitch = 1f;
		audio.clip = collapse[Random.Range (0, collapse.Length)];
		audio.Play();
	}

	public void SwingSoundLight()
	{
		audio.pitch = 2f;
		audio.clip = swing[Random.Range (0, swing.Length)];
		audio.Play();
	}

	public void SwingSoundHeavy()
	{
		audio.pitch = 1f;
		audio.clip = swing[Random.Range (0, swing.Length)];
		audio.Play();
	}

	public void JumpSound()
	{
		audio.pitch = 1f;
		audio.clip = jump[Random.Range (0, jump.Length)];
		audio.Play();
	}

	public void BowDraw()
	{
		audio.pitch = 1f;
		audio.clip = bowDraw[Random.Range (0, bowDraw.Length)];
		audio.Play();
	}

	public void BowShoot()
	{
		audio.pitch = 1f;
		audio.clip = bowShoot[Random.Range (0, bowShoot.Length)];
		audio.Play();
	}
}
