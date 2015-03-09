using UnityEngine;
using System.Collections.Generic;

public class Wall : MonoBehaviour
{
	LineRenderer lr;
	float maxDisplacement = 0.15f; // In % of line segment length
	public List<Vector2> points;
	
	void Start ()
	{
		lr = GetComponent<LineRenderer> ();
	}
	void Update ()
	{
	
	}
	public void midPointBisection (int n)
	{
		for (int i=0; i<n; i++)
			midPointBisection ();
	}
	public void midPointBisection ()
	{
		if (points.Count >= 2) {
			List<Vector2> toAdd = new List<Vector2> ();
			for (int i = 0; i < points.Count - 1; i++) { // For all points except the last one
				var segment = points [i + 1] - points [i];
				var midpoint = Vector2.Lerp (points [i], points [i + 1], 0.5f);
				var theta = Mathf.Atan2 (segment.y, segment.x); // in radians
				theta += Mathf.PI / 2f; // Perpendicular, ¼ of a full circle
				var length = segment.magnitude * Random.Range (-maxDisplacement, maxDisplacement);
				var displacement = length * (new Vector2 (Mathf.Cos (theta), Mathf.Sin (theta)));
				var newPt = midpoint + displacement;
				toAdd.Add (newPt);
			}
			for (int i = 0; i < toAdd.Count; i++) {
				points.Insert (i * 2 + 1, toAdd [i]);
			}
		} else {
			Debug.Log ("Error, not enough points!");
		}
	}
	public void draw ()
	{
		lr = GetComponent<LineRenderer> ();
		lr.SetVertexCount (points.Count);
		for (int i = 0; i < points.Count; i++) {
			lr.SetPosition (i, points [i]);
		}
	}
}
