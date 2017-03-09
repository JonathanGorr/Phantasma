using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Evolution : WaitForPlayer {

	public static Evolution Instance = null;

	/*this class stores the functionality of evolution;
	what weapons and health upgrades are unlocked


							     -------[i]------[m]
							   /
							  /
		  ---------[c]-------[f]
		/		    \		/
	[a]-------------[b]---[e]-------[h]------[L]
	|	\		    /	   \
	|	 ---------[d]------[g]	 
	|			 |		    \	 
	|			 |		  |  \
	|			 |	      |	  --------[j]------[k]
	|			 |	      |		      |        |
	0 blood----100 b------300 b-------600 b----1200-1500 b
	|		   |		  |			  |		   |
	start	   tier 1     tier 2	  tier 3   Final Tiers

	Start
	A. Shortsword

	Tier 1 (100 blood)
	C. Spear
	B. Falchion
	D. Axe

	Tier 2 (300 blood)
	F. Lance
	E. LongSword
	G. Morning Star

	Tier 3 (600 blood)
	I. Halberd
	H. Rapier
	J. Flail

	Final Tiers (1200-1500 blood)
	K. Ball and Chain (swinging)
	L. Zweihander
	M.War Scythe

	Swords
	Shortsword>Falchion>LongSword>Rapier>Zweihander

	PoleArms
	shortsword>Spear>Lance>Halberd>WarScythe
	
	Axes
	shortsword>Axe>MorningStar>Flail>BallAndChain

	//Depending on choice, assign prefab weapon to hand slot?
	*/

	public Text bloodCounter;
	public CanvasGroup _evoMenu;

	[HideInInspector]
	private int blood;

	[HideInInspector]
	public int Blood
	{
		get { return blood; }
		set { blood = value; }
	}
	
	private bool open;

	void Awake()
	{
		if(Instance == null) Instance = this;
		UpdateUI();
	}

	public void UpdateUI()
	{
		bloodCounter.text = blood.ToString();
	}

	public void AddBlood(int b)
	{
		blood += b;
		UpdateUI();
	}

	public void SubtractBlood(int b)
	{
		blood -= b;
		UpdateUI();
	}

	void Open()
	{
		_evoMenu.TurnOn();
	}
	
	void Close()
	{
		_evoMenu.TurnOff();
	}
}


