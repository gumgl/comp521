using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour
{
	public Point V1;
	public Point V2;
	public float OriginalLength; //The length of the edge when it was created
	public bool visible;

	void Start ()
	{
	
	}
	public void UpdateEdge ()
	{
		var V1V2 = V2.currPos - V1.currPos; 
		
		//Calculate the current distance
		float V1V2Length = V1V2.magnitude; 
		
		//Calculate the difference from the original length
		float Diff = V1V2Length - OriginalLength; 
		
		V1V2.Normalize ();
		
		//Push both vertices apart by half of the difference respectively 
		//so the distance between them equals the original length
		V1.currPos += V1V2 * Diff * 0.5f; 
		V2.currPos -= V1V2 * Diff * 0.5f;

		if (visible) {
			var localV1V2 = (Vector2)V2.transform.localPosition - (Vector2)V1.transform.localPosition;
			var localLength = localV1V2.magnitude;
			var localMidpoint = (Vector2)(V1.transform.localPosition + V2.transform.localPosition) / 2;
			transform.localPosition = localMidpoint;

			transform.localScale = new Vector3 (localLength, 0.2f, 1f);

			var angle = (Mathf.Atan2 (localV1V2.y, localV1V2.x) + Mathf.PI * 2) % (Mathf.PI * 2); //Mathf.Atan (V1V2.y / V1V2.x);
			var tmp = transform.localEulerAngles;
			tmp.z = angle * Mathf.Rad2Deg;
			transform.localEulerAngles = tmp;
		} else {
			transform.localScale = Vector3.zero;
		}
	}
}

