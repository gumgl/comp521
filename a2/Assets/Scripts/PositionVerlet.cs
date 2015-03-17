using UnityEngine;
using System.Collections.Generic;

public class PositionVerlet : Projectile
{
	public List<Point> points = new List<Point> ();
	public List<Edge> edges = new List<Edge> ();
	// Use this for initialization
	new void Start ()
	{
		base.Start ();
	}
	new void Update ()
	{
		base.FixedUpdate ();
	}
	public override void setup (float angle, float velocity, Vector2 pos)
	{
		resursivelyAddChildren (transform);
		Debug.Log (points.Count.ToString () + " pts & " + edges.Count.ToString () + " edges");
		transform.position = pos;
		setupPoints (angle, velocity);
		setupEdges ();
	}
	/// <summary>Set the points' locations to what they are right now.</summary>
	void setupPoints (float angle, float velocity)
	{
		var motion = getComponents (angle, velocity) * Time.fixedDeltaTime * 0.5f;
		foreach (Point point in points) { // Initialize points
			point.prevPos = (Vector2)point.transform.position;
			point.currPos = (Vector2)point.transform.position + motion;
			point.id = Point.idCount++;
			//Debug.Log ("point setup.");
			point.acceleration = Vector2.up * - gameRef.gravity * 0.5f;// * 5;
		}
	}
	/// <summary>Set the edges' originalLengths to what they are right now.</summary>
	void setupEdges ()
	{
		foreach (Edge edge in edges)
			edge.OriginalLength = (edge.V2.currPos - edge.V1.currPos).magnitude;
	}
	void resursivelyAddChildren (Transform tf)
	{
		foreach (Transform child in tf) {
			var go = child.gameObject;

			Point point = go.GetComponent<Point> ();
			if (point != null)
				points.Add (point);

			Edge edge = go.GetComponent<Edge> ();
			if (edge != null)
				edges.Add (edge);
			/*if (go is Point)
				points.Add ((Point)go);
			else if (go is Edge)
				edges.Add ((Edge)go);*/
			this.resursivelyAddChildren (child);
		}
	}
	public override void moveOnce ()
	{
		// Do nothing
		updatePoints ();
	}
	public override void updateProjectile ()
	{
		foreach (Ball ball in gameRef.GetComponentsInChildren<Ball>()) {
			ball.collision (this);
		}
		updateEdges ();
	}
	public override void collision (Wall wall)
	{
		foreach (Point point in points) {
			var from = point.prevPos;
			var to = point.currPos;
			var radius = 0;
			var projMotion = to - from;
			var projAngle = Mathf.Atan2 (projMotion.y, projMotion.x);
			var projAngleMinimized = minimize (projAngle);
			var initVelocity = projMotion.magnitude;
			for (int i = 0; i < wall.points.Count - 1; i++) { // For all points except the last one
				Vector2 collisionPt = Vector2.zero;
				var collided = Intersection (from, to, (Vector2)wall.transform.parent.position + wall.points [i], (Vector2)wall.transform.parent.position + wall.points [i + 1], ref collisionPt);
				if (collided) {
					//Debug.Log ("Point/Wall Collision! " + collisionPt);
					var obsMotion = wall.points [i + 1] - wall.points [i];
					var obsAngleMinimized = minimize (Mathf.Atan2 (obsMotion.y, obsMotion.x));
					var diff = obsAngleMinimized - projAngleMinimized;
					var obsAngleOpposite = obsAngleMinimized + Mathf.PI;
					var projAngleReflected = obsAngleOpposite + diff;
				
					var newAngle = projAngleReflected;
					var newVelocity = initVelocity * restitutionCoefficient;

					point.setup (newAngle, newVelocity, collisionPt);
					point.UpdatePoint ();
					//moveOnce ();
				}
			}
		}
	}
	public override bool offBounds (Vector2 absMin, Vector2 absMax)
	{
		return ! inBounds (absMin - Vector2.one * 5, absMax + Vector2.one * 5, points [0].currPos);
	}
	void updatePoints ()
	{
		foreach (Point point in points) {
			point.UpdatePoint ();
			var wind = Vector2.right * gameRef.windForce * Time.deltaTime;
			var airEffect = calcAir (point.currPos - point.prevPos, wind);
			//point.currPos -= airEffect;
		}
	}
	void updateEdges ()
	{
		foreach (Edge edge in edges)
			edge.UpdateEdge ();
	}
}

