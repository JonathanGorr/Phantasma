using System.Collections;
using UnityEngine;

[System.Serializable]
public class Location {

	/// <summary>
	/// Location:
	/// This is the location specific to a location objective
	///	It can either be an exact location (v2) or simply a zone (graveyard, castle, etc)
	/// </summary>

	public Vector2 worldCoord;
	public enum ZoneType { None, House, Graveyard, Camp, Castle, BossRoom }
	public ZoneType zone;

	public Vector2 WorldCoord
	{
		get { return worldCoord; }
	}

	public ZoneType Zone
	{
		get { return zone; }
	}

	//the position you want the player to go to
	public Location(Vector2 coord)
	{
		this.worldCoord = coord;
		zone = ZoneType.None;
	}

	public Location(ZoneType z)
	{
		worldCoord = Vector2.zero;
		this.zone = z;
	}
}
