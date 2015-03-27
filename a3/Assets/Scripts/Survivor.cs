using UnityEngine;
using System.Collections.Generic;

public class Survivor : MonoBehaviour
{
	static int visionUpdateRate = 10; // Update the visibility of zombies every X frames
	static int pathRecalcRate = 10; // Recalculate the path every X frames
	static float pathCheckDensity = 0.5f; // Check points at every X intervals between path corners
	public Game game;
	float velocity;
	Util.Direction direction;
	List<Zombie> visibleZombies = new List<Zombie> ();
	List<GameObject> targets;
	//List<Vector2> currentPath = new List<Vector2> ();
	GameObject currTarget = null;
	GameObject previousTarget = null;
	NavMeshAgent agent;


	void Start ()
	{
		if (game.GetSense () == Util.Sense.CW)
			targets = game.visitPointsCW;
		else
			targets = game.visitPointsCCW;

		agent = GetComponent<NavMeshAgent> ();
		agent.ResetPath ();
		velocity = game.v * 1.5f;
		agent.speed = velocity;
		//agent.SetDestination (new Vector2 (0.5f, 10.5f).GetVector3 ());
		//GeneratePath ();
	}
	void Update ()
	{
		DebugPath ();
		CheckDeath ();
		AvoidanceNow ();
		if (!agent.hasPath)
			ToNextTarget ();
		else if (agent.hasPath && currTarget != null && Vector2.Distance (GetPosition (), currTarget.transform.position.GetVector2 ()) < 0.5f) {
			previousTarget = currTarget;
			previousTarget.renderer.enabled = false;
			currTarget = null; // fetch next target in list
			agent.ResetPath ();
			if (targets.Count == 0)
				game.Win ();
			ToNextTarget ();
		}
		if (Time.frameCount % visionUpdateRate == 0)
			UpdateVision ();
	}
	void UpdateVision ()
	{
		visibleZombies.Clear ();
		foreach (Zombie zombie in game.zombies) {
			if (zombie.SurvivorCanSee (this)) {
				zombie.SetVisible (true);
				visibleZombies.Add (zombie);
			} else {
				zombie.SetVisible (false);
			}
		}
		if (!ValidatePath (agent.path, 5f)) {
			// Invalidate path
			Debug.Log ("Oh ho...");
			agent.ResetPath ();
//			if (previousTarget != null)
//				TryPathTo (previousTarget);
		}
	}
	void ToNextTarget ()
	{
		if (targets.Count > 0) {
			if (currTarget == null) {
				currTarget = targets [0];
				targets.RemoveAt (0);
			}
			TryPathTo (currTarget);
			//agent.SetDestination (target.transform.position);
		}
	}
	bool TryPathTo (GameObject target)
	{
		NavMeshPath candidate = new NavMeshPath ();
		agent.CalculatePath (target.transform.position, candidate);
		if (ValidatePath (candidate, 5f)) {
			agent.path = candidate;
			return true;
		} else
			return false;
	}
	void CheckDeath ()
	{
		if (!ValidatePoint (GetPosition (), 0f)) {
			game.Loss ();
		}
	}
	void AvoidanceNow ()
	{
		var pos = GetPosition ();
		var time = 2f; // predict 2 seconds ahead
		if (!agent.hasPath) { // If not moving
			foreach (Zombie zombie in visibleZombies) {
				if (zombie != null) { // In case they're despawned by the game, at spawn points
					var box = zombie.GetDeathBox ();
					var prediction = zombie.GetDirection ().GetVector () * zombie.velocity * time;
					box.min += prediction;
					box.max += prediction;
					if (Util.InBox (pos, box)) {
						var move = agent.speed * Time.deltaTime * zombie.GetDirection ().GetVector ().GetVector3 ();
						agent.Move (move);
						Debug.Log ("collision detected! moving by " + move.ToString ());
					}
				}
			}
		}
	}
	/// <summary>Makes sure that we're not visible in the path</summary>
	bool ValidatePath (NavMeshPath path, float maxDistance)
	{
		float time = 0f;
		int size = path.corners.Length;
		//if (size > 0 && ! ValidatePoint (path.corners [0], time))
		//	return false;
		for (int i=1; i<size; i++) {
			var from = path.corners [i - 1];
			var to = path.corners [i];
			var diff = to - from;
			var dist = diff.magnitude;
			var distSoFar = 0f;
			var localTime = time;
			while (distSoFar < dist) {
				var point = Vector2.MoveTowards (path.corners [i - 1].GetVector2 (), path.corners [i].GetVector2 (), distSoFar);
				if (! ValidatePoint (point, localTime))
					return false;
					
				localTime += pathCheckDensity / agent.speed;
				distSoFar += pathCheckDensity;
			}
			time += dist / agent.speed;
		}
		return true;
	}
	bool ValidatePoint (Vector2 pt, float time)
	{
		//time = 0f;
		foreach (Zombie zombie in visibleZombies) {
			if (zombie != null) { // In case they're despawned by the game, at spawn points
				var box = zombie.GetDeathBox ();
				var prediction = zombie.GetDirection ().GetVector () * zombie.velocity * time;
				box.min += prediction;
				box.max += prediction;
				if (Util.InBox (pt, box)) {
					return false;
				}
			}
		}
		return true;
	}
	void DebugPath ()
	{
		int size = agent.path.corners.Length;
		for (int i=0; i<size-1; i++)
			Debug.DrawLine (agent.path.corners [i], agent.path.corners [i + 1], Color.blue);
	}
	/*void GeneratePath ()
	{
		Dictionary<Vector2, int> dist = new Dictionary<Vector2, int> (); // Cost
		Dictionary<Vector2, Vector2> prev = new Dictionary<Vector2, Vector2> (); // Came from
		
		List<Vector2> frontier = new List<Vector2> ();
		Dictionary<Vector2, int> priorities = new Dictionary<Vector2, int> (); // Came from
		
		Vector2 source = GetPosition ();
		dist [source] = 0;
		prev [source] = null;
		frontier.Add (source);
		
		while (frontier.Count > 0) {
			
			Vector2 current = null;
			for (int i=0; i<frontier.Count; i++) { // Pick Vector2 with least priority value
				Vector2 possibleU = frontier [i];
				if (current == null || priorities [possibleU] < priorities [current]) {
					current = possibleU;
				}
			}
			frontier.Remove (current);
			if (current == target)
				break; // First time we find the target, we exit as it is the shortest path
			
			foreach (KeyValuePair<Hex.Direction, Vector2> kv in current.getNeighbours()) {
				Vector2 neighbour = kv.Value;
				int newDist = dist [current] + 1;
				if (((neighbour.canWalkThrough (this) || neighbour == target)) &&
					(dist.ContainsKey (neighbour) == false || newDist < dist [neighbour])) {
					dist [neighbour] = newDist;
					
					frontier.Add (neighbour);
					priorities [neighbour] = newDist + Util.ManhattanDistance (target.pos, neighbour.pos); // heuristic
					prev [neighbour] = current;
				}
			}
		}
		if (prev.ContainsKey (target)) { // If reachable
			currentPath = new List<Vector2> ();
			Vector2 curr = target;
			
			while (curr != null) { // Add path by backtracking
				currentPath.Add (curr);
				curr = prev [curr];
			}
			currentPath.Reverse ();
			currentPathIndex = 0;
			MoveToNextVector2 ();
		}
	}*/

	public Vector2 GetPosition ()
	{
		return transform.position.GetVector2 ();
	}
	public void SetPosition (Vector2 pos)
	{
		transform.position = pos.GetVector3 ();
	}
}

