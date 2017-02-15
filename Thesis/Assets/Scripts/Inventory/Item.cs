using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

	public enum ItemType { consumable, armor, weapon, ammo, spell, magic, quest, key }
	public enum ItemRarity { common, uncommon, rare, legendary }
	public int ID { get; set; }
	public string SpriteID { get; set; } //TODO: cannot serialize this, need to reference sprite somehow else
	public string ConsumeSound { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public int Value { get; set; }
	public int Power { get; set; }
	public int Defense { get; set; }
	public int Vitality { get; set; }
	public bool Stackable { get; set; }
	public ItemRarity Rarity { get; set; }
	public string Slug { get; set; }
	public Sprite Sprite { get; set; }
	public ItemType Type { get; set; }
	public bool Slots { get; set; }

	public Item(int id, string title, string description, int value, bool slots, int power, int defense, int vitality, bool stackable, string rarity, string slug )
	{
		this.ID = id;
		this.Title = title;
		this.Value = value;
		this.Slots = slots;
		this.Description = description;
		this.Defense = defense;
		this.Power = power;
		this.Stackable = stackable;
		this.Vitality = vitality;
		this.Rarity = (ItemRarity)System.Enum.Parse(typeof (ItemRarity), rarity);
		this.Slug = slug;
		this.Sprite = Resources.Load<Sprite>("Sprites/Items/" + slug);
	}

	public Item()
	{
		this.ID = -1;
	}

	public override string ToString()
	{
		return Description;
	}
}
