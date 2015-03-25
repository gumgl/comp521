using UnityEngine;
using System.Collections;

public abstract class Zombie : MonoBehaviour
{
	Game game;
	public Util.Direction direction;
	public Util.Sense sense;
	static public float v = 2.0f; // in squares/second
	static public float size = 1.0f;
	public float velocity;
	public float maxVelocity;
	public bool canSwitchLanes;
	public int laneID;
	public int type;
	public SpawnPoint spawnPoint = null; // Most recently visited spawn point (so that we don't turn/respawn twice)
	public GameObject visibleT;
	public GameObject invisibleT;
	bool visible = false;

	//public abstract enum State
	public enum ProximityLevel
	{
		Close,
		Visible,
		Invisible
	}

	void Update ()
	{
		// SetVisible (GetPosition ().x < 18.5f); // SetVisibility test
		UpdateVisibility ();
		SpecialPreMovement ();
		AdjustVelocity ();
		Vector2 move = GetMove ();
		Vector2 newPos = GetPosition () + move;
		MoveBy (move);
		game.HandleSpawnPoint (this);
	}
	abstract public void SpecialPreMovement ();
	public void UpdateVisibility ()
	{
		if (SurvivorCanSee (game.survivor))
			SetVisible (true);
		else
			SetVisible (false);
	}
	public void AdjustVelocity ()
	{
		float closest = Mathf.Infinity; // Set value on closest zombie
		foreach (Zombie other in game.zombies) {
			if (other != this && other.laneID == laneID) {
				var pl = CanSee (other.GetPosition ());
				var distance = Vector2.Distance (GetPosition (), other.GetPosition ());
				if (pl <= ProximityLevel.Visible && distance < closest && other.velocity < velocity) {
					if (other.sense == this.sense) {
						if (distance < 1.5f && canSwitchLanes) { // Too close, doesnt matter if behind or not. One must move.
							TryChangeLane ();
						} else if (IsBehind (other)) {
							//Debug.Log ("behind SLOWER!");
							if (!game.showingVisible)
								SetVisible (false);
							closest = distance;
							if (pl == ProximityLevel.Close) {
								velocity = maxVelocity * 0.05f + other.velocity * 0.95f; // Slow down a lot
							} else if (pl == ProximityLevel.Visible)
								velocity = maxVelocity * 0.25f + other.velocity * 0.75f; // Slow down towards them
						} else if (!game.showingVisible)
							SetVisible (true);
					} else {
						if (pl == ProximityLevel.Close) {
							TryChangeLane ();
						}
					}
				}
			}
		}
	}
	public Vector2 GetMove ()
	{
		return direction.GetVector () * velocity * Time.deltaTime;
	}
	public bool SurvivorCanSee (Survivor food)
	{
		// from zombie to survivor, check if name we hit the survivor (trickier to check if we hit this zombie)
		var delta = food.GetPosition () - this.GetPosition ();
		RaycastHit hit;
		bool collided = Physics.Raycast (this.GetPosition (), delta, out hit);
//		Debug.DrawRay (this.GetPosition (), delta, Color.cyan);
//		if (collided)
//			Debug.Log (hit.collider.gameObject.name);
		return (collided && hit.collider.gameObject.name == "Survivor");
	}
	public bool CanSeeSurvivor (Survivor food)
	{
		return CanSee (food.GetPosition ()) <= ProximityLevel.Visible;
	}
	public ProximityLevel CanSee (Vector2 pos)
	{
		Vector2 min = GetPosition () - Vector2.one * 1.5f;
		Vector2 max = GetPosition () + Vector2.one * 1.5f;
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
	public bool TryChangeLane ()
	{
		//Debug.Log ("Trying to change lane!");
		if (ChangeLaneLegal (-1)) {
			ChangeLane (-1);
			return true;
		} else if (ChangeLaneLegal (+1)) {
			ChangeLane (+1);
			return true;
		} else
			return false;
	}
	public bool ChangeLaneLegal (int delta)
	{
		int newLaneID = laneID + delta;
		if (! Util.InRange (newLaneID, 0, 2))
			return false;
		if (! game.CanSpawnZombie (GetLaneChangeMove (delta)))
			return false;
		return true;
	}
	/// <summary>Changes the zombie's lane and updates its position.</summary>
	/// <param name="delta">Change in laneID, towards the center.</param>
	public void ChangeLane (int delta)
	{
		int newLaneID = laneID + delta;
		Vector2 move = GetLaneChangeMove (delta);

		MoveBy (move);
		laneID = newLaneID;
	}
	public Vector2 GetLaneChangeMove (int delta)
	{
		Vector2 move;
		if ((delta == -1 && sense == Util.Sense.CW) || (delta == 1 && sense == Util.Sense.CCW))
			move = direction.TurnLeft ().GetVector ();
		else
			move = direction.TurnRight ().GetVector ();
		return move;
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
	public bool IsBehind (Zombie zombie)
	{
		var delta = zombie.GetPosition () - this.GetPosition ();
		var ourAngle = Mathf.Atan2 (direction.GetVector ().y, direction.GetVector ().x);
		var deltaAngle = Mathf.Atan2 (delta.y, delta.x);
		var diffAngle = Mathf.Abs (deltaAngle - ourAngle) % (2 * Mathf.PI);

		return (diffAngle * Mathf.Rad2Deg < 90);
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
	virtual public Util.Sense GetSense ()
	{
		return sense;
	}
	public void SetVisible (bool vis)
	{
		//if (vis != visible) {
		visible = vis;
		visibleT.renderer.enabled = visible;
		invisibleT.renderer.enabled = !visible;
		//}
	}
}
