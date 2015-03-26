using System.Collections.Generic;
using UnityEngine;

/// <summary>A class to provide useful static functions.</summary>
public static class Util
{
	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}
	public enum Sense
	{
		CW,
		CCW
	}
	private static Dictionary<Direction, Vector2> vectorDirections = new Dictionary<Direction, Vector2> ();

	static Util ()
	{
		vectorDirections [Direction.Up] = Vector2.up;
		vectorDirections [Direction.Right] = Vector2.right;
		vectorDirections [Direction.Down] = -1 * Vector2.up;
		vectorDirections [Direction.Left] = -1 * Vector2.right;
	}

	/// <summary>Gets the Vector2 associated with this direction</summary>
	/// <returns>The vector.</returns>
	/// <param name="dir">The direction.</param>
	public static Vector2 GetVector (this Direction dir)
	{
		return vectorDirections [dir];
	}
	public static Vector3 GetVector3 (this Vector2 vector)
	{
		return new Vector3 (vector.x, 0, vector.y);
	}
	public static Vector2 GetVector2 (this Vector3 vector)
	{
		return new Vector2 (vector.x, vector.z);
	}
	public static Direction TurnLeft (this Direction dir)
	{
		int size = System.Enum.GetNames (typeof(Direction)).Length;
		return (Direction)(((int)dir - 1 + size) % size);
	}
	public static Direction TurnRight (this Direction dir)
	{
		int size = System.Enum.GetNames (typeof(Direction)).Length;
		return (Direction)(((int)dir + 1) % size);
	}
	public static Direction Reverse (this Direction dir)
	{
		int size = System.Enum.GetNames (typeof(Direction)).Length;
		return (Direction)(((int)dir + 2) % size);
	}
	public static Direction GetRandomDirection ()
	{
		int size = System.Enum.GetNames (typeof(Direction)).Length;
		return (Direction)Random.Range (0, size);
	}
	public static Sense Reverse (this Sense sense)
	{
		return (Sense)(((int)sense + 1) % 2);
	}
	public static Sense GetRandomSense ()
	{
		return (Sense)Random.Range (0, 2);
	}

	/// <summary>Calculates whether x is in [a,b].</summary>
	/// <returns><c>true</c> if x is in the range defined by [a,b],<c>false</c> otherwise.</returns>
	/// <param name="x">The value we're testing.</param>
	/// <param name="a">The min value</param>
	/// <param name="b">The max value.</param>
	public static bool InRange (float x, float a, float b)
	{
		return (a < x && x < b);
	}
	public static bool InRange (int x, int a, int b)
	{
		return (a < x && x < b);
	}
	/// <summary>Calculates whether a point is in the 2D box defined by min and max.</summary>
	/// <returns><c>true</c>, if pt is in the box defined by min and max, <c>false</c> otherwise.</returns>
	/// <param name="pt">The point we're testing.</param>
	/// <param name="min">Bottom-left bound.</param>
	/// <param name="max">Top-right bound.</param>
	public static bool InBox (Vector2 pt, Vector2 min, Vector2 max)
	{
		return (InRange (pt.x, min.x, max.x) && InRange (pt.y, min.y, max.y));
	}
	public static float ManhattanDistance (Vector2 from, Vector2 to)
	{
		return Mathf.Abs (to.x - from.x) + Mathf.Abs (to.y - from.y);
	}
}