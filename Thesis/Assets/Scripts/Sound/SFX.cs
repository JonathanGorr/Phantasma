using UnityEngine;
using System.Collections;

public class SFX : MonoBehaviour {

	public static SFX Instance = null;

	public GameObject fxPrefab;

	[Header("Player")]
	public AudioClip[] playerYell;
	public AudioClip[] playerHurt;
	public AudioClip[] playerJump;
	public AudioClip[] playerDied;
	public AudioClip[] sheathes;
	public AudioClip[] slideBlocking;
	public AudioClip[] slide;
	[Header("Enemy")]
	public AudioClip[] enemyHurt;
	public AudioClip[] enemyDied;
	public AudioClip flyingSkullAttack;
	public AudioClip flyingSkullWindup;
	public AudioClip fireball;
	public AudioClip[] skeletonHurt;
	public AudioClip[] skeletonDeath;
	[Header("Weapon Sounds")]
	public AudioClip[] swings;
	public AudioClip[] slideAttackWhooshes;
	public AudioClip[] bowDraw;
	public AudioClip[] bowShoot;
	[Header("Collision")]
	public AudioClip[] block;
	public AudioClip[] weaponCollide;
	public AudioClip[] arrowImpact_Wood;
	public AudioClip[] arrowImpact_Flesh;
	[Header("Status")]
	public AudioClip heal;
	[Header("Items")]
	public AudioClip bloodPickup;
	[Header("Environment")]
	public AudioClip[] potShatters;
	public AudioClip[] woodHits;
	public AudioClip[] woodShatters;
	public AudioClip door;
	[Header("Money")]
	public AudioClip coinBounces;
	public AudioClip buy;
	public AudioClip[] coinDrops;
	public AudioClip[] coinCollects;

	public AudioSource asrc;
	[Header("UI")]
	public AudioClip[] potions;
	public AudioClip[] foods;
	public AudioClip[] armors;
	public AudioClip[] weapons;
	[Header("Jingles")]
	public AudioClip newQuest;
	public AudioClip finishQuest;
	public AudioClip failQuest;
	[Header("Buttons")]
	public AudioClip scroll;
	public AudioClip back;
	public AudioClip select;
	[Header("Debug")]
	public AudioClip debug;
	public AudioClip error;

	public AudioClip[] page_flips;

	void Awake()
	{
		if(Instance == null) Instance = this;
	}

	public void PlayUI(string command)
	{
		AudioClip clip = null;

		switch(command)
		{
			//UI
			case "debug":
			clip = debug;
			break;
			case "error":
			clip = error;
			break;
			case "scroll":
			clip = scroll;
			break;
			case "select":
			clip = select;
			break;
			case "back":
			clip = back;
			break;
			case "pause":
			clip = page_flips[Random.Range(0, page_flips.Length)];
			break;

			//Quests
			case "questUpdate":
			clip = newQuest;
			break;
			case "questCompleted":
			clip  = finishQuest;
			break;
			case "questFailed":
			clip = failQuest;
			break;

			//Items
			case "potion":
			clip = potions[Random.Range(0, potions.Length)];
			break;
			case "bread":
			clip = foods[Random.Range(0, foods.Length)];
			break;
			case "armor":
			clip = armors[Random.Range(0, armors.Length)];
			break;
			case "weapon":
			clip = weapons[Random.Range(0, weapons.Length)];
			break;

			default:
			print(command + " was not recognized");
			break;
		}

		if(clip == null) return;
		asrc.clip = clip;
		asrc.Play();
	}

	public void SetVolume(float v)
	{
		asrc.volume = v;
	}

	public void PlayFX(string command, Vector3 position)
	{
		AudioClip clip = null;
		float pitch = 1;

		switch(command)
		{
			//player
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
			case "swing_slide":
				clip = slideAttackWhooshes[Random.Range(0, slideAttackWhooshes.Length)];
				pitch = 1f;
				break;
			case "jump":
				clip = playerJump[Random.Range(0, playerJump.Length)];
				break;
			case "slide":
				clip = slide[Random.Range(0, slide.Length)];
				break;
			case "slide_blocking":
				clip = slide[Random.Range(0, slide.Length)];
				break;


			//enemies
			//skeleton
			case "hurt_skeleton":
				clip = skeletonHurt[Random.Range(0, skeletonHurt.Length)];
				break;
			case "death_skeleton":
				clip = skeletonDeath[Random.Range(0, skeletonDeath.Length)];
				break;
			case "attack_flyingSkull":
				clip = flyingSkullAttack;
				break;
			case "windup_flyingSkull":
				clip = flyingSkullWindup;
				break;
			case "fireball":
				clip = fireball;
				break;

			//arrow
			case "draw_arrow":
				clip = bowDraw[Random.Range(0, bowDraw.Length)];
				break;
			case "fire_arrow":
				clip = bowShoot[Random.Range(0, bowShoot.Length)];
				break;
			case "arrow_Enemy":
				clip = arrowImpact_Flesh[Random.Range(0, arrowImpact_Flesh.Length)];
				break;
			case "arrow_Player":
				clip = arrowImpact_Flesh[Random.Range(0, arrowImpact_Flesh.Length)];
				break;
			case "arrow_Wood":
				clip = arrowImpact_Wood[Random.Range(0, arrowImpact_Wood.Length)];
				break;
			case "arrow_Stone":
				clip = block[Random.Range(0, block.Length)];
				break;
			case "arrow_Untagged":
				clip = arrowImpact_Wood[Random.Range(0, arrowImpact_Wood.Length)];
				break;

			//environment
			case "smash_pot":
				clip = potShatters[Random.Range(0, potShatters.Length)];
				break;
			case "smash_wood":
				clip = woodShatters[Random.Range(0, woodShatters.Length)];
				break;
			case "damage_wood":
				clip = woodHits[Random.Range(0, woodHits.Length)];
				break;
			case "coin_bounce":
				clip = coinBounces;
				break;
			case "buy":
				clip = buy;
				break;
			case "door":
				clip = door;
				break;
			case "blood_pickup":
				clip = bloodPickup;
				break;
			case "coin_drop":
				clip = coinDrops[Random.Range(0, coinDrops.Length)];
				break;
			case "coin_pickup":
				clip = coinCollects[Random.Range(0, coinCollects.Length)];
				break;

			case "block":
				clip = block[Random.Range(0, block.Length)];
				break;
			case "weapons_collide":
				clip = weaponCollide[Random.Range(0, weaponCollide.Length)];
				break;
			case "sheathe":
				clip = sheathes[Random.Range(0, sheathes.Length)];
				break;
			case "heal":
				clip = heal;
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
