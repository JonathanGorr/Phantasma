using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Evolution : MonoBehaviour {

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

	//this determines when the upgrade bool is true
	
	//private int blood;
	[SerializeField, HideInInspector]
	private Text bloodCounter;

	//must serialize or else reference defaults to null
	[SerializeField, HideInInspector]
	public GameObject _evoMenu;

	private LevelManager _manager;

	[HideInInspector]
	public int blood;
	
	private bool open;

	void Awake()
	{
		_evoMenu = GameObject.Find ("EvolutionMenu");
		_manager = GetComponent<LevelManager>();
		if(!_manager.inMenu && !_manager.inInitialize) bloodCounter = GameObject.Find("BloodCounter").GetComponent<Text>();
	}

	void FixedUpdate()
	{
		if(bloodCounter != null)
		{
			//assign current blood to ui
			bloodCounter.text = blood.ToString();

			//announce benchmarks
			switch(blood)
			{
			case 10:
				//print ("you have " + blood + " blood!");
				break;
			case 20:
				//print ("you have " + blood + " blood!");
				break;
			case 5:
				//print ("you have " + blood + " blood!");
				break;
			}

			if(_evoMenu != null)
			{
				//toggle window open
				if(Input.GetButtonDown("360_BackButton"))
				{
					open = !open;
					print("open/Close");
				}
				if(open)
				{
					Open();
				}
				else if(!open)
				{
					Close();
				}
			}
		}
	}

	public void AddBlood(int b)
	{
		blood += b;
	}

	public void SubtractBlood(int b)
	{
		blood -= b;
	}

	void Open()
	{
		_evoMenu.SetActive(true);
	}
	
	void Close()
	{
		_evoMenu.SetActive(false);
	}
}


