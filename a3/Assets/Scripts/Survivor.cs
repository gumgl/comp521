using UnityEngine;
using System.Collections.Generic;

public class Survivor : MonoBehaviour
{
	public Game game;
	Util.Direction direction;
	List<GameObject> targets;
	//List<Vector2> currentPath = new List<Vector2> ();
	GameObject target = null;
	NavMeshAgent agent;
	float velocity;

	void Start ()
	{
		if (game.GetSense () == Util.Sense.CW)
			targets = game.visitPointsCW;
		else
			targets = game.visitPointsCCW;

		agent = GetComponent<NavMeshAgent> ();
		agent.speed = game.v * 1.5f;
		//agent.SetDestination (new Vector2 (0.5f, 10.5f).GetVector3 ());
		//GeneratePath ();
	}
	void Update ()
	{
		if (targets.Count > 0 &&
			(target == null || target != null && Vector2.Distance (GetPosition (), target.transform.position.GetVector2 ()) < 0.5f))
			ToNextTarget ();
	}

	void ToNextTarget ()
	{
		target = targets [0];
		targets.RemoveAt (0);
		agent.SetDestination (target.transform.position);
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

