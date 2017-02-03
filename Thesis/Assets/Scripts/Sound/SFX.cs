using UnityEngine;
using System.Collections;

public class SFX : MonoBehaviour {

	public GameObject fxPrefab;

	[Header("Player")]
	public AudioClip[] playerYell;
	public AudioClip[] playerHurt;
	public AudioClip[] playerJump;
	public AudioClip[] playerDied;
	public AudioClip[] sheathes;
	[Header("Enemy")]
	public AudioClip[] enemyHurt;
	public AudioClip[] enemyDied;
	[Header("Weapon Sounds")]
	public AudioClip[] swings;
	public AudioClip[] bowDraw;
	public AudioClip[] bowShoot;
	[Header("Collision")]
	public AudioClip[] shieldCollide;
	public AudioClip[] weaponCollide;

	public void PlayUI(string command)
	{
		
	}

	public void PlayFX(string command, Vector3 position)
	{
		AudioClip clip = null;
		float pitch = 1;

		switch(command)
		{
			case "player_Hurt":
				clip = playerHurt[Random.Range(0, playerHurt.Length)];
				break;
			case "enemy_Hurt":
				clip = enemyHurt[Random.Range(0, enemyHurt.Length)];
				break;
			case "player_Died":
				clip = playerDied[Random.Range(0, playerDied.Length)];
				break;
			case "enemy_Died":
				clip = enemyDied[Random.Range(0, enemyDied.Length)];
				break;
			case "swing_Heavy":
				clip = swings[Random.Range(0, swings.Length)];
				break;
			case "swing_Light":
				clip = swings[Random.Range(0, swings.Length)];
				pitch = 2f;
				break;
			case "jump":
				clip = playerJump[Random.Range(0, playerJump.Length)];
				break;
			case "bow_Draw":
				clip = bowDraw[Random.Range(0, bowDraw.Length)];
				break;
			case "bow_Shoot":
				clip = bowShoot[Random.Range(0, bowShoot.Length)];
				break;
			case "shield_hit":
				clip = shieldCollide[Random.Range(0, shieldCollide.Length)];
				break;
			case "weapon_hit":
				clip = weaponCollide[Random.Range(0, weaponCollide.Length)];
				break;
			case "sheathe":
				clip = sheathes[Random.Range(0, sheathes.Length)];
				break;
			default:
				print(command + " was not a recognized SFX command");
				break;
		}

		if(clip == null) return;

		GameObject go = Instantiate(fxPrefab, position, Quaternion.identity);
		go.name = clip.name;
		AudioSource asrc = go.GetComponent<AudioSource>();
		asrc.pitch = pitch;
		asrc.clip = clip;
		asrc.Play();
	}
}
