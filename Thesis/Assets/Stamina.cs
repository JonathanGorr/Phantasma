using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour {

	private UI _ui;
	public Slider _staminaBar;
	[SerializeField] private float stamina;
	public float maxStamina = 10;
	public float recoverDelay = 1f;
	public float recoverSpeed = 2;
	float t;

	public float MaxStamina
	{
		get { return maxStamina; }
	}

	public float CurrentStamina
	{
		get { return stamina; }
	}

	public bool Ready
	{
		get { return stamina > 0; }
	}

	void OnEnable()
	{
		if(gameObject.CompareTag("Player"))
		{
			_staminaBar = GameObject.Find("UI").GetComponent<UI>().MyStaminaBar;
		}
		stamina = maxStamina;
		UpdateStaminaBar();
		StartCoroutine("RecoverStamina");
	}

	public void UseStamina(float cost)
	{
		if(stamina <= 0) return;
		stamina -= cost;
		t = recoverDelay;
		if(stamina <= 0) t += recoverDelay*1.5f; //double delay if used ALL stamina!
		stamina = Mathf.Clamp(stamina, 0, maxStamina);
		UpdateStaminaBar();
	}

	public virtual void UpdateStaminaBar()
	{
		_staminaBar.maxValue = maxStamina;
		_staminaBar.value = stamina;
	}

	//waits x time before recovering stamina
	IEnumerator RecoverStamina()
	{
		while(true)
		{
			//wait until you can recover
			while(t > 0)
			{
				t -= Time.deltaTime;
				yield return null;
			}

			t = 0;

			//recover
			if(stamina < maxStamina && t <= 0)
			{
				stamina += Time.deltaTime * recoverSpeed;
			}

			UpdateStaminaBar();
			yield return null;
		}
	}
}
