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
		/*return ((Util.InRange (pos.x, min.x, max.x) // Bottom and top spaces
			&& (Util.InRange (pos.y, min.y, min.y + width) || Util.InRange (pos.y, max.y - width, max.y)))
			|| (Util.InRange (pos.y, min.y, max.y) // Left and right spaces
			&& (Util.InRange (pos.x, min.x, min.x + width) || Util.InRange (pos.x, max.x - width, max.x))));*/

		// Another simpler way: check if it's in the outside box while not in inside box
		return (Util.InBox (pos, min, max) && ! Util.InBox (pos, min + Vector2.one * width, max - Vector2.one * width));
	}
}

