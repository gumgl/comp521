using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour
{
	#region References
	public Game gameRef;
	#endregion
	public float restitutionCoefficient;
	public float airResistanceFactor; // in % of velocity^2 relative to air

	public void Start ()
	{
	}
	public void FixedUpdate ()
	{
		moveOnce ();
		updateProjectile ();
		collision (gameRef.leftWall);
		collision (gameRef.rightWall);

	}
	public abstract void setup (float angle, float velocity, Vector2 pos);
	public abstract void moveOnce ();
	public abstract void updateProjectile ();
	public abstract void collision (Wall wall);
	public abstract bool offBounds (Vector2 absMin, Vector2 absMax);
	public void disappear ()
	{
		transform.localScale = Vector3.zero;
	}

	protected Vector2 calcAir (Vector2 velocity, Vector2 wind)
	{
		var relativeVelocity = velocity - wind;

		float slowdownFactor = airResistanceFactor * relativeVelocity.sqrMagnitude;

		return relativeVelocity * airResistanceFactor;
	}
	/// <summary>
	/// Computes i, the intersection point of lines ab and cd.
	/// Taken from http://stackoverflow.com/a/1968345
	/// </summary>
	/// <returns><c>true</c>, if i was found, <c>false</c> otherwise.</returns>
	/// <param name="a">The first point of ab.</param>
	/// <param name="b">The second point of ab.</param>
	/// <param name="c">The first point of cd.</param>
	/// <param name="d">The second point of cd.</param>
	/// <param name="i">The intersection point.</param>
	static public bool Intersection (Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 i)
	{
		Vector2 s1 = b - a;
		Vector2 s2 = d - c;
		
		var s = (-s1.y * (a.x - c.x) + s1.x * (a.y - c.y)) / (-s2.x * s1.y + s1.x * s2.y);
		var t = (s2.x * (a.y - c.y) - s2.y * (a.x - c.x)) / (-s2.x * s1.y + s1.x * s2.y);
		
		if (s >= 0 && s <= 1 && t >= 0 && t <= 1) { // Collision detected
			i.x = a.x + (t * s1.x);
			i.y = a.y + (t * s1.y);
			return true;
		} else
			return false; // No collision
	}
	/// <summary>
	/// Maps angles -> 0..PI
	/// </summary>
	/// <param name="a">The angle in radians</param>
	static public float minimize (float a)
	{
		return (a + Mathf.PI) % Mathf.PI;
	}
	static public Vector2 further (Vector2 motion, float distance)
	{
		return motion.normalized * distance;
	}
	static public Vector2 getComponents (float angle, float magnitude)
	{
		return new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * magnitude;
	}
	/// <summary>Checks if a vector is within the bounds of a rectangle a to b</summary>
	/// <returns><c>true</c>, if x is in bounds, <c>false</c> otherwise.</returns>
	/// <param name="min">Bottom-left corner.</param>
	/// <param name="max">Top-right corner.</param>
	/// <param name="v">The coordinate.</param>
	static public bool inBounds (Vector2 min, Vector2 max, Vector2 v)
	{
		return (v.x > min.x && v.x < max.x && v.y > min.y && v.y < max.y);
	}
}

