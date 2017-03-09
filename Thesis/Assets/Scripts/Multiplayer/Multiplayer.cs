using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Multiplayer:
/// Used to spawn multiplayer avatars, and keep track of them for their derive scripts
/// </summary>

public class Multiplayer : MonoBehaviour {

	public static Multiplayer Instance = null;

	//This is used to give subsequent players alternative appearances( to distinguish )
	[System.Serializable]
	public class Avatar
	{
		[SerializeField] private int id = 0;
		[SerializeField] string name = "Player";
		[SerializeField] Color color = Color.white;
		Player player;

		//constructor
		public Avatar(int i, string n, Color c, Player p)
		{
			id = i;
			name = n;
			color = c;
			player = p;
		}

		public int ID
		{
			get { return id; }
		}
		public string Name
		{
			get { return name; }
		}
		public Color Color 
		{
			get { return color; }
		}
		public Player Player
		{
			get { return player; }
		}
	}

	public GameObject playerPrefab;
	public List<Avatar> players = new List<Avatar>();

	[SerializeField] private int playerCount = 0;
	public int PlayerCount()
	{
		return playerCount;
	}

	public Player GetPlayer(int id)
	{
		print("player got");
		for(int i=0;i<players.Count;i++)
		{
			if(players[i].ID == id) return players[i].Player;
		}

		print("Player doesn't exist");
		return null;
	}

	//customization
	[Header("Color")]
	public Color[] torsoPalette;
	public Color[] pantsPalette;
	public Color[] shoesPalette;
	public Color[] armorPalette;

	void Awake()
	{
		if(Instance == null) Instance = this;
	}

	void Update()
	{
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.F))
		{
			CreatePlayer();
		}
#endif
	}

	void OrderListByIndex() //sort player list by index
	{
		
	}

	//assign random colors
	void AssignColors(PlayerInfo pi)
	{
		//torso
		Color t = torsoPalette[Random.Range(0, torsoPalette.Length)];
		for(int i = 0;i<pi.torso.Length;i++)
		{
			pi.torso[i].color = t;
		}

		//pants
		Color p = pantsPalette[Random.Range(0, pantsPalette.Length)];
		for(int i = 0;i<pi.pants.Length;i++)
		{
			pi.pants[i].color = p;
		}

		//shoes
		Color s = shoesPalette[Random.Range(0, shoesPalette.Length)];
		for(int i = 0;i<pi.shoes.Length;i++)
		{
			pi.shoes[i].color = s;
		}

		//armor
		Color a = armorPalette[Random.Range(0, armorPalette.Length)];
		for(int i = 0;i<pi.armor.Length;i++)
		{
			pi.armor[i].color = a;
		}
	}

	//custom assign colors
	void AssignColors(PlayerInfo pi, Color t, Color p, Color s, Color a)
	{
		//torso
		for(int i = 0;i<pi.torso.Length;i++)
		{
			pi.torso[i].color = t;
		}

		//pants
		for(int i = 0;i<pi.pants.Length;i++)
		{
			pi.pants[i].color = p;
		}

		//shoes
		for(int i = 0;i<pi.shoes.Length;i++)
		{
			pi.shoes[i].color = s;
		}

		//armor
		for(int i = 0;i<pi.armor.Length;i++)
		{
			pi.armor[i].color = a;
		}
	}

	//automatically creates random character
	public Multiplayer.Avatar CreatePlayer()
	{
		GameObject go = Instantiate(playerPrefab, GameObject.FindWithTag("Respawn").transform.position, Quaternion.identity);
		DontDestroyOnLoad(go);
		Player p = go.GetComponent<Player>();

		int id = players.Count + 1;
		p.gameObject.name = "Player " + id;
		//assign passed things
		p.title = name; //TODO: replace with custom name when that's set up

		Avatar a = new Avatar(id, name, torsoPalette[Random.Range(0, torsoPalette.Length)], p);
		AssignColors(p.GetComponent<PlayerInfo>());
		//add to list and sort
		players.Add(a);
		OrderListByIndex();
		playerCount = players.Count;
		return a;
	}

	//creates custom character
	public Multiplayer.Avatar CreatePlayer(int id, string name, Color color)
	{
		GameObject go = Instantiate(playerPrefab);
		DontDestroyOnLoad(go);
		Player p = go.GetComponent<Player>();

		p.gameObject.name = "Player " + id;
		//assign passed things
		p.title = name; //TODO: replace with custom name when that's set up

		Avatar a = new Avatar(id, name, color, p);
		//add to list and sort
		players.Add(a);
		OrderListByIndex();
		playerCount = players.Count;
		return a;
	}
}
