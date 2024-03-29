using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public List<Lane> lanes = new List<Lane> ();
	/// <summary># of zombies initially spawned.</summary>
	public int n;
	/// <summary>percentage of respawn (1-p = percentage of simply moving along).</summary>
	public float p;
	/// <summary>percentage of hard zombies (1-r = percentage of easy zombies).</summary>
	public float r;
	/// <summary>Velocity used throughout the game.</summary>
	public float v;

	public List<Zombie> zombies = new List<Zombie> ();
	public Zombie[] prefabs = new Zombie[4];
	public List<SpawnPoint> spawnPoints = new List<SpawnPoint> ();
	public Survivor survivor;
	public bool showingVisible = true;
	public List<GameObject> visitPointsCW = new List<GameObject> ();
	public List<GameObject> visitPointsCCW = new List<GameObject> ();
	public UnityEngine.UI.Text winText;
	public UnityEngine.UI.Text lossText;
	public float maxTime;
	int gameStatus = 0; // 0 ongoing, 1 loss, 2 win
	Util.Sense sense;


	void Start ()
	{
		AddSpawnPoints ();
		SpawnInitialZombies ();
		winText.enabled = false;
		lossText.enabled = false;
	}
	void AddSpawnPoints ()
	{
		for (int i=0; i<lanes.Count; i++) {
			Lane lane = lanes [i];
			float xMin = lane.min.x + lane.width / 2f;
			float xMax = lane.max.x - lane.width / 2f;
			float yMin = lane.min.y + lane.width / 2f;
			float yMax = lane.max.y - lane.width / 2f;
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
		sense = Util.GetRandomSense ();
		for (int i=0; i<n; i++) { // Spawn zombies
			int type, laneID, side;
			Util.Direction dir;
			Vector2 pos;
			do { // Until we find an unoccupied spot
				type = Random.Range (0, 2);
				laneID = Random.Range (0, 3);
				side = Random.Range (0, 3);
				Lane lane = lanes [laneID];
				float x, y, half = lane.width / 2f;
				switch (side) {
				case 0: // Left
					x = lane.min.x + half;
					y = Random.Range (lane.min.y + half, lane.max.y - half);
					if (sense == Util.Sense.CW)
						dir = Util.Direction.Up;
					else
						dir = Util.Direction.Down;
					break;
				case 1: // Bottom
					x = Random.Range (lane.min.x + half, lane.max.x - half);
					y = lane.min.y + half;
					if (sense == Util.Sense.CW)
						dir = Util.Direction.Left;
					else
						dir = Util.Direction.Right;
					break;
				case 2: // Top
				default: // So that the compiler knows we always set x & y
					x = Random.Range (lane.min.x + half, lane.max.x - half);
					y = lane.max.y - half;
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
	public Zombie SpawnZombie (int type, Vector2 pos)
	{
		//Debug.Log ("Spawning a zombie at " + pos.ToString ());
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
		//Debug.Log ("Despawning the zombie at " + zombie.GetPosition ().ToString ());
		zombies.Remove (zombie);
		Destroy (zombie.gameObject);
	}
	public void HandleSpawnPoint (Zombie zombie)
	{
		var pts = GetSpawnPoints ();
		foreach (SpawnPoint pt in pts) { // Search all the spawn points
			if (pt != zombie.spawnPoint // If this is not the last seen spawn point
				&& Vector2.Distance (pt.GetPosition (), zombie.GetPosition ()) < zombie.velocity * Time.deltaTime * 1.5f) { // If we are on a spawn point
				//Debug.Log ("Zombie encounters a spawn point)");
				if (Random.Range (0f, 1f) < p) { // Respawing
					int tries = pts.Count * 2; // To avoid potential infinite loop when all spawn points are occupied (rare case)
					SpawnPoint newPt;
					do {
						newPt = pts [Random.Range (0, pts.Count)];
						tries --;
					} while (!CanSpawnZombie(newPt.GetPosition ()) && tries > 0);
					int type;
					if (Random.Range (0f, 1f) < r)
						type = Random.Range (2, 4);
					else
						type = Random.Range (0, 2);

					if (type != zombie.type) { // Only spawn a new zombie if type changed
						Zombie newZombie = SpawnZombie (type, newPt.GetPosition ());
						newZombie.sense = sense;
						DeSpawnZombie (zombie);
						zombie = newZombie;
					}
					zombie.spawnPoint = newPt;
				} else
					zombie.spawnPoint = pt;
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
	public Util.Sense GetSense ()
	{
		return sense;
	}
	void Update ()
	{
		if (gameStatus == 0 && Time.time > maxTime * v * 3)
			Loss ();
	}
	public void Win ()
	{
		Debug.Log ("WIN");
		gameStatus = 2;
		winText.enabled = true;
		EndGame ();
	}
	public void Loss ()
	{
		Debug.Log ("LOSS");
		gameStatus = 1;
		lossText.enabled = true;
	}

	void EndGame ()
	{
		foreach (Zombie zombie in zombies)
			zombie.renderer.enabled = false;
		survivor.renderer.enabled = false;
		Debug.Log ("Game duration (s): " + Time.time.ToString () + " = v * " + (Time.time / v).ToString ());
	}
}