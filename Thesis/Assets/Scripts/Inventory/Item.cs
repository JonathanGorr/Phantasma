
public enum ItemType { consumable, armor, weapon, ammo, spell, magic, quest, key }
public enum ItemRarity { common, uncommon, rare, legendary }

[System.Serializable]
public class Item //blood, money etc
{
	public int id;
	public string title;
	public string description;
	public int value;
	public int count;
	public string slug;
	public int[] stats;
	public string rarity;
	public bool stackable;
	public bool slots;

	public Item(int id, string title, string description, int value, int count, bool slots, int[] stats, bool stackable, string rarity, string slug)
	{
		this.id = id;
		this.title = title;
		this.value = value;
		this.count = count;
		this.slots = slots;
		this.description = description;
		this.stats = stats;
		this.stackable = stackable;
		this.rarity = rarity;
		this.slug = slug;
	}

	public Item()
	{
		this.id = -1;
	}
}
