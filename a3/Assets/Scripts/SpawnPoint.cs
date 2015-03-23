using UnityEngine;
using System.Collections.Generic;

public class SpawnPoint
{
	Dictionary<Util.Sense, Util.Direction> directions = new Dictionary<Util.Sense, Util.Direction> ();
	public int laneID;
	Vector2 position;
	
	public Util.Direction GetDirection (Util.Sense sense)
	{
		return directions [sense];
	}
	public void SetDirection (Util.Sense sense, Util.Direction direction)
	{
		directions [sense] = direction;
	}
	public Vector2 GetPosition ()
	{
		return position;
	}
	public void SetPosition (Vector2 pos)
	{
		position = pos;
	}
}

