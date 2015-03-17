using UnityEngine;
using System.Collections;

public class Ball : Projectile
{
	public float initVelocity;
	public float initAngle; // in radians
	public Vector2 initPos;
	public Vector2 prevPos;
	public Vector2 currPos;
	public float initTime;

	new void Start ()
	{
		base.Start ();
	}

	new void FixedUpdate ()
	{
		base.FixedUpdate ();
	}
	public override void setup (float angle, float velocity, Vector2 pos)
	{
		initVelocity = velocity;
		initAngle = angle;
		initPos = pos;
		currPos = pos;
		transform.position = pos;
		initTime = Time.time;
	}
	public override void moveOnce ()
	{
		var t = Time.time - initTime;
		var dt = Time.deltaTime;
		prevPos = currPos;
		
		var parabola = calcParabola (t, initVelocity, initAngle) - calcParabola (t - dt, initVelocity, initAngle);
		var wind = Vector2.right * gameRef.windForce * dt;
		var airEffect = calcAir (parabola, wind);

		currPos = currPos + parabola + wind;// - airEffect;
	}
	public override void updateProjectile ()
	{
		transform.position = currPos;
	}
	public void collision (PositionVerlet pv)
	{
		var from = prevPos;
		var to = currPos;
		//var radius = getSize () / 4;
		var projMotion = to - from;
		var projAngle = Mathf.Atan2 (projMotion.y, projMotion.x);
		//to += further (projMotion, radius); // extend range to detect when tip of projectile collides
		var projAngleMinimized = minimize (projAngle);
		
		foreach (Edge edge in pv.edges) { // For all edges
			Vector2 collisionPt = Vector2.zero;
			var collided = Intersection (from, to, edge.V1.currPos, edge.V2.currPos, ref collisionPt);
			if (collided) {
				Debug.Log ("Ball/PositionVerlet Collision!" + collisionPt);
				/*var obsMotion = edge.V2.currPos - edge.V1.currPos;
				var obsAngleMinimized = minimize (Mathf.Atan2 (obsMotion.y, obsMotion.x));
				var diff = obsAngleMinimized - projAngleMinimized;
				var obsAngleOpposite = obsAngleMinimized + Mathf.PI;
				var projAngleReflected = obsAngleOpposite + diff;
				
				var newAngle = projAngleReflected;
				var newVelocity = initVelocity * restitutionCoefficient;
				var newVector = getComponents (newAngle, newVelocity);
				var newPos = collisionPt + further (newVector, radius); // Move it away since collision was detected early
				
				setup (newAngle, newVelocity, collisionPt);*/
				edge.V1.moveBy (projMotion);
				edge.V2.moveBy (projMotion);

				//disappear ();
				//moveOnce ();
				//break;
			}
		}
	}
	public override void collision (Wall wall)
	{
		var from = prevPos;
		var to = currPos;
		var radius = getSize () / 4;
		var projMotion = to - from;
		var projAngle = Mathf.Atan2 (projMotion.y, projMotion.x);
		to += further (projMotion, radius); // extend range to detect when tip of projectile collides
		var projAngleMinimized = minimize (projAngle);
		
		for (int i = 0; i < wall.points.Count - 1; i++) { // For all points except the last one
			Vector2 collisionPt = Vector2.zero;
			var collided = Intersection (from, to, (Vector2)wall.transform.parent.position + wall.points [i], (Vector2)wall.transform.parent.position + wall.points [i + 1], ref collisionPt);
			if (collided) {
				//Debug.Log ("Ball/Wall Collision!" + collisionPt);
				var obsMotion = wall.points [i + 1] - wall.points [i];
				var obsAngleMinimized = minimize (Mathf.Atan2 (obsMotion.y, obsMotion.x));
				var diff = obsAngleMinimized - projAngleMinimized;
				var obsAngleOpposite = obsAngleMinimized + Mathf.PI;
				var projAngleReflected = obsAngleOpposite + diff;
				
				var newAngle = projAngleReflected;
				var newVelocity = initVelocity * restitutionCoefficient;
				var newVector = getComponents (newAngle, newVelocity);
				var newPos = collisionPt + further (newVector, radius); // Move it away since collision was detected early
				
				setup (newAngle, newVelocity, collisionPt);
				//disappear ();
				moveOnce ();
				break;
			}
		}
	}
	public override bool offBounds (Vector2 absMin, Vector2 absMax)
	{
		return ! inBounds (absMin, absMax, currPos);
	}
	public float getSize ()
	{
		return transform.localScale.magnitude;
	}
	/// <summary>
	/// Calculates the parabola of a projectile.
	/// </summary>
	/// <returns>The displacement of a projectile in a parabola.</returns>
	/// <param name="t">Time since launch.</param>
	/// <param name="v">Velocity.</param>
	/// <param name="theta">Angle of launch.</param>
	public Vector2 calcParabola (float t, float v, float theta)
	{
		float dx = t * v * Mathf.Cos (theta);
		float dy = t * v * Mathf.Sin (theta) - 1f / 2 * gameRef.gravity * Mathf.Pow (t, 2);
		return new Vector2 (dx, dy);
	}
}
