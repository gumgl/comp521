using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public List<Lane> lanes = new List<Lane> ();
	/// <summary># of zombies initially spawned</summary>
	public int n;
	/// <summary>percentage of respawn (1-p = percentage of simply moving along)</summary>
	public float p;
	/// <summary>percentage of hard zombies (1-r = percentage of easy zombies)summary>
	public float r;
	public List<Zombie> zombies = new List<Zombie> ();
	public Zombie[] prefabs = new Zombie[4];
	public List<SpawnPoint> spawnPoints = new List<SpawnPoint> ();

	void Start ()
	{
		AddSpawnPoints ();
		SpawnInitialZombies ();

	}
	void AddSpawnPoints ()
	{
		for (int i=0; i<lanes.Count; i++) {
			Lane lane = lanes [i];
			float xMin = lane.min.x + lane.width / 2;
			float xMax = lane.max.x - lane.width / 2;
			float yMin = lane.min.y + lane.width / 2;
			float yMax = lane.max.y - lane.width / 2;
			var sp1 = new SpawnPoint (); // Bottom-left
			sp1.SetPosition (new Vector2 (xMin, yMin));
			sp1.SetDirection (Util.Sense.CW, Util.Direction.Up);
			sp1.SetDirection (Util.Sense.CCW, Util.Direction.Right);
			sp1.laneID = i;
			spawnPoints.Add (sp1);
			var sp2 = new SpawnPoint (); // Top-left
			sp2.SetPosition (new Vector2 (xMin, yMax));
			sp2.SetDirection (Util.Sense.CW, Util.Direction.Right);
			sp2.SetDirection (Util.Sense.CCW, Util.Direction.Down);
			sp2.laneID = i;
			spawnPoints.Add (sp2);
			var sp3 = new SpawnPoint (); // Bottom-right
			sp3.SetPosition (new Vector2 (xMax, yMin));
			sp3.SetDirection (Util.Sense.CW, Util.Direction.Left);
			sp3.SetDirection (Util.Sense.CCW, Util.Direction.Up);
			sp3.laneID = i;
			spawnPoints.Add (sp3);
			var sp4 = new SpawnPoint (); // Top-right
			sp4.SetPosition (new Vector2 (xMax, yMax));
			sp4.SetDirection (Util.Sense.CW, Util.Direction.Down);
			sp4.SetDirection (Util.Sense.CCW, Util.Direction.Left);
			sp4.laneID = i;
			spawnPoints.Add (sp4);
		}
	}
	void SpawnInitialZombies ()
	{
		Util.Sense sense = Util.GetRandomSense ();
		for (int i=0; i<n; i++) { // Spawn zombies
			int type, laneID, side;
			Util.Direction dir;
			Vector2 pos;
			do { // Until we find an unoccupied spot
				type = Random.Range (0, 1);
				laneID = Random.Range (0, 2);
				side = Random.Range (0, 2);
				Lane lane = lanes [laneID];
				float x, y;
				switch (side) {
				case 0: // Left
					x = Random.Range (lane.min.x, lane.min.x + lane.width);
					y = Random.Range (lane.min.y, lane.max.y);
					if (sense == Util.Sense.CW)
						dir = Util.Direction.Up;
					else
						dir = Util.Direction.Down;
					break;
				case 1: // Bottom
					x = Random.Range (lane.min.x, lane.max.x);
					y = Random.Range (lane.min.y, lane.min.y + lane.width);
					if (sense == Util.Sense.CW)
						dir = Util.Direction.Left;
					else
						dir = Util.Direction.Right;
					break;
				case 2: // Top
				default: // So that the compiler knows we always set x & y
					x = Random.Range (lane.min.x, lane.max.x);
					y = Random.Range (lane.max.y - lane.width, lane.max.y);
					if (sense == Util.Sense.CW)
						dir = Util.Direction.Right;
					else
						dir = Util.Direction.Left;
					break;
				}
				
				pos = new Vector2 (x, y);
				
			} while (!CanSpawnZombie(pos));
			
			var recruit = SpawnZombie (type, pos);
			recruit.SetDirection (dir);
			recruit.sense = sense;
			recruit.laneID = laneID;
		}
	}
	void Update ()
	{
	
	}
	public Zombie SpawnZombie (int type, Vector2 pos)
	{
		Zombie recruit = (Zombie)Instantiate (prefabs [type], Vector3.zero, Quaternion.identity);
		recruit.transform.parent = this.transform;
		recruit.SetGame (this);
		recruit.type = type;
		recruit.SetPosition (pos);
		zombies.Add (recruit);
		return recruit;
	}
	public void DeSpawnZombie (Zombie zombie)
	{
		zombies.Remove (zombie);
		Destroy (zombie.gameObject);
	}
	public void HandleSpawnPoint (Zombie zombie)
	{
		var pts = GetSpawnPoints ();
		foreach (SpawnPoint pt in pts) { // Search all the spawn points
			if (pt != zombie.spawnPoint // If this is not the last seen spawn point
				&& Vector2.Distance (pt.GetPosition (), zombie.GetPosition ()) < 0.1 * zombie.GetSize ()) { // If we are on a spawn point
				if (Random.Range (0f, 1f) < p) { // Respawing
					int tries = pts.Count * 2; // To avoid potential infinite loop when all spawn points are occupied (rare case)
					SpawnPoint newPt;
					do {
						newPt = pts [Random.Range (0, pts.Count - 1)];
						tries --;
					} while (!CanSpawnZombie(newPt.GetPosition ()) && tries > 0);
					int type;
					if (Random.Range (0f, 1f) < r)
						type = Random.Range (2, 3);
					else
						type = Random.Range (0, 1);

					if (type == zombie.type)
						zombie.spawnPoint = newPt;
					else { // Only spawn a new zombie if type changed
						Zombie newZombie = SpawnZombie (type, newPt.GetPosition ());
						newZombie.sense = zombie.sense;
						DeSpawnZombie (zombie);
						zombie = newZombie;
					}
					zombie.spawnPoint = newPt;
				}
				// Either way, we still set its position & direction
				zombie.ZeroOnSpawnPoint ();
				break;
			}
		}
	}
	public List<Lane> GetLanes ()
	{
		return lanes;
	}
	public List<SpawnPoint> GetSpawnPoints ()
	{
		return spawnPoints;
	}
	public bool CanSpawnZombie (Vector2 pos)
	{
		foreach (Zombie zombie in zombies) {
			if (Vector2.Distance (pos, zombie.GetPosition ()) < zombie.GetRadius () * 2)
				return false;
		}
		return true;
	}
}

