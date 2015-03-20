using UnityEngine;
using System.Collections;

public class Lane : MonoBehaviour
{
	public int width = 1;
	/// <summary>Bottom-left outside bound</summary>
	public Vector2 min;
	/// <summary>Top-right outside bound</summary>
	public Vector2 max;

	void Start ()
	{
	
	}
	void Update ()
	{
	
	}

	public bool IsInside (Vector2 pos)
	{
		return ((inRange (pos.x, min.x, max.x) // Bottom and top spaces
			&& (inRange (pos.y, min.y, min.y + 1) || inRange (pos.y, max.y - 1, max.y)))
			|| (inRange (pos.y, min.y, max.y) // Left and right spaces
			&& (inRange (pos.x, min.x, min.x + 1) || inRange (pos.x, max.x - 1, max.x))));
	}

	/// <summary>Calculates whether x is in [a,b]</summary>
	/// <returns><c>true</c> if x is in the range defined by [a,b],<c>false</c> otherwise.</returns>
	/// <param name="x">The value we're testing.</param>
	/// <param name="a">The min value</param>
	/// <param name="b">The max value.</param>
	static bool inRange (float x, float a, float b)
	{
		return (a < x && x < b);
	}
}

