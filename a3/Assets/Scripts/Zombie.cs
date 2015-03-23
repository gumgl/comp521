using UnityEngine;
using System.Collections;

public abstract class Zombie : MonoBehaviour
{
	Game game;
	public Util.Direction direction;
	public Util.Sense sense;
	static public float v = 1.0f; // in squares/second
	static public float size = 1.0f;
	public float maxVelocity;
	public int laneID;
	public int type;
	public SpawnPoint spawnPoint = null; // Most recently visited spawn point (so that we don't turn/respawn twice)

	//public abstract enum State
	public enum ProximityLevel
	{
		Close,
		Visible,
		Invisible
	}

	void Update ()
	{
		if (game != null)
			game.HandleSpawnPoint (this);
		Vector2 move = GetMove ();
		Vector2 newPos = GetPosition () + move;

	}
	public Vector2 GetMove ()
	{
		return direction.GetVector () * maxVelocity * Time.deltaTime;
	}

	public bool CanSeeSurvivor (Survivor food)
	{
		return CanSee (food.GetPosition ()) <= ProximityLevel.Visible;
	}

	public ProximityLevel CanSee (Vector2 pos)
	{
		Vector2 min = GetPosition () - Vector2.one;
		Vector2 max = GetPosition () + Vector2.one;
		if (Util.InBox (pos, min, max))
			return ProximityLevel.Close;
		else {
			int forwardExtra = 6; // Can see further in front of him
			switch (direction) {
			case Util.Direction.Up:
				max.y += forwardExtra;
				break;
			case Util.Direction.Right:
				max.x += forwardExtra;
				break;
			case Util.Direction.Down:
				min.y -= forwardExtra;
				break;
			case Util.Direction.Left:
				min.x -= forwardExtra;
				break;
			}
			if (Util.InBox (pos, min, max))
				return ProximityLevel.Visible;
			else
				return ProximityLevel.Invisible;
		}
	}
	/// <summary>Changes the zombie's lane and updates its position.</summary>
	/// <param name="delta">Change in laneID, towards the center.</param>
	public void ChangeLane (int delta)
	{
		int newLaneID = laneID + delta;
		if (Util.InRange (newLaneID, 0, 2)) {
			Vector2 move;
			if ((delta == -1 && sense == Util.Sense.CW) || (delta == 1 && sense == Util.Sense.CCW))
				move = direction.TurnLeft ().GetVector ();
			else
				move = direction.TurnRight ().GetVector ();

			MoveBy (move);
			laneID = newLaneID;
		}
	}
	public void TurnRight ()
	{
		direction = direction.TurnRight ();
	}
	public void TurnLeft ()
	{
		direction = direction.TurnLeft ();
	}
	public void MoveBy (Vector2 delta)
	{
		Vector2 pos = GetPosition ();
		pos += delta;
		SetPosition (pos);
	}
	/// <summary>Resets position and direction to the current spawnPoint<summary>
	public void ZeroOnSpawnPoint ()
	{
		if (spawnPoint != null) {
			SetPosition (spawnPoint.GetPosition ());
			SetDirection (spawnPoint.GetDirection (sense));
			laneID = spawnPoint.laneID;
		}
	}
	public Util.Direction GetDirection ()
	{
		return direction;
	}
	public void SetDirection (Util.Direction dir)
	{
		direction = dir;
	}
	public Vector2 GetPosition ()
	{
		return transform.position;
	}
	public void SetPosition (Vector2 pos)
	{
		transform.position = pos;
	}
	public int GetLaneID ()
	{
		return laneID;
	}
	public float GetSize ()
	{
		return size;
	}
	public float GetRadius ()
	{
		return GetSize () / 2;
	}
	public void SetGame (Game gameRef)
	{
		game = gameRef;
	}
}
