using UnityEngine;
using System.Collections;

public class Wheel : MonoBehaviour
{
	private float density = 1f;
	public Car car;
	public float radius;
	public float Radius
	{
		get { return radius; }
		set
		{
			radius = value;
			transform.localScale = Vector3.one * radius;
			GetComponent<Rigidbody2D>().mass = Weight;
		}
	}

	public int cornerID;

	public int CornerID
	{
		get {return cornerID;}
		set {
			cornerID = value;
			transform.localPosition = (Vector3) Corner.ToVector2();
		}
	}
	//public VectorP corner;

	public VectorP Corner {
		get { return car.corners[CornerID]; }
	}

	public float Weight
	{
		get
		{
			return Mathf.PI * radius * radius * density;
		}
	}
}
