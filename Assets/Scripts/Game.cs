using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public bool defaultWind;
	public bool defaultAngle;
	public bool defaultVelocity;
	public float gravity;
	public int maxX;
	public int maxY;
	public float maxWind;
	public Wall leftWall;
	public Wall rightWall;
	public Cannon leftCannon;
	public Cannon rightCannon;
	public List<Projectile> projectiles = new List<Projectile> ();
	public float windForce;
	public Text windText;
	public GameObject windArrow;

	void Start ()
	{
		//Debug.Log ("Q1=" + Mathf.Atan2 (1, 1).ToString () + ", Q2=" + Mathf.Atan2 (1, -1).ToString () + ", Q3=" + Mathf.Atan2 (-1, -1).ToString () + ", Q1=" + Mathf.Atan2 (-1, 1).ToString ());
		BuildWalls ();
		leftCannon.changeAngle ();
		rightCannon.changeAngle ();
		PlaceCannon (leftCannon, leftWall, 2f / 3);
		PlaceCannon (rightCannon, rightWall, 2f / 3);
		changeWind ();
		InvokeRepeating ("changeWind", 0f, 0.5f);
	}
	void BuildWalls ()
	{
		leftWall.midPointBisection (4);
		leftWall.draw ();
		rightWall.midPointBisection (4);
		rightWall.draw ();
	}
	/// <summary>
	/// Places the cannon at given heights.
	/// </summary>
	/// <param name="cannon">Cannon.</param>
	/// <param name="wall">Wall.</param>
	/// <param name="height">Height in % of the total wall length starting from bottom.</param>
	void PlaceCannon (Cannon cannon, Wall wall, float height)
	{
		var maxDistance = wall.getLength () * (1 - height);
		var distance = 0f;
		for (int i = 0; i < wall.points.Count - 1; i++) { // For all points except the last one
			var segment = wall.points [i + 1] - wall.points [i];
			var segmentLength = segment.magnitude;
			if (distance + segmentLength > maxDistance) { // Place cannon on this segment
				var segmentDistance = maxDistance - distance;
				var segmentPortion = segmentDistance / segmentLength;
				var segmentPoint = wall.points [i] + (segment * segmentPortion);
				var segmentAngle = Mathf.Atan2 (segment.y, segment.x);
				var stickOutAngle = (cannon.facing == Cannon.Direction.Left) ? segmentAngle - Mathf.PI / 2 : segmentAngle + Mathf.PI / 2;
				var stickOut = Projectile.getComponents (stickOutAngle, 3f);
				var newPos = (Vector2)wall.transform.parent.transform.position + segmentPoint + stickOut;
				cannon.transform.position = newPos;
				break;
			} else // Move on to next segment
				distance += segmentLength;
		}
	}
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			projectiles.Add (leftCannon.shoot ());
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			projectiles.Add (rightCannon.shoot ());
		}
		var minAbs = (Vector2)transform.position + new Vector2 (-maxX, -maxY);
		var maxAbs = (Vector2)transform.position + new Vector2 (maxX, maxY);
		for (int i = projectiles.Count - 1; i >= 0; i--) { // Iterating backwards so indexes are not broken
			if (projectiles [i].offBounds (minAbs, maxAbs)) {
				Destroy (projectiles [i].gameObject);
				projectiles.RemoveAt (i);
				Debug.Log ("Removed a projectile!");
			}
		}
	}
	void changeWind ()
	{
		windForce = defaultWind ? 0 : Random.Range (-maxWind, maxWind);
		windText.text = windForce.ToString ();
		var tmp = windArrow.transform.localScale;
		tmp.x = windForce / maxWind;
		windArrow.transform.localScale = tmp;
	}
	bool offBoundsCheck (GameObject go)
	{
		var where = go.transform.localPosition;
		// Add a 10% buffer so they really disappear off-screen
		if (Mathf.Abs (where.x) > maxX || where.y < -maxY * 1.1f) {
			Destroy (go);
			Debug.Log ("Removed an object!");
			return true;
		} else
			return false;
	}
}
