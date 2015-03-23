using UnityEngine;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{
	Dictionary<Util.Sense, Util.Direction> directions = new Dictionary<Util.Sense, Util.Direction> ();
	public int laneID;
	
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
		return transform.position;
	}
	public void SetPosition (Vector2 pos)
	{
		transform.position = pos;
	}
}

