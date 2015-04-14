using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
	public const int MIN_VECTORS = 8;
	public const int MAX_VECTORS = 8;
	public const int MIN_WHEELS = 1;
	public const int MAX_WHEELS = 3;
	public const float WHEEL_FREQUENCY = 0.5f;
	public const float MAX_MAGNITUDE = 2;
	public const float MIN_WHEEL_RADIUS = 0.2f;
	public const float MAX_WHEEL_RADIUS = 1f;
	public const float MOTOR_SPEED = -500;

	public Wheel wheelPrefab;
	public GameObject chassis;
	private Mesh chassisMesh;

	public List<VectorP> corners = new List<VectorP>();
	public List<Wheel> wheels = new List<Wheel>();

	public void Init() {
		
	}

	public void InitRandom() {
		corners.Clear();

		// VECTORS
		int numCorners = (MAX_VECTORS == MIN_VECTORS) ? MIN_VECTORS : Random.Range(MIN_VECTORS, MAX_VECTORS + 1);

		for (int i = 0; i < numCorners; i++) {
			var newV = new VectorP();

			if (i<4) // add one vector in each quadrant. otherwise the (0,0) is outside the car
				newV.Angle = Random.Range(Mathf.PI / 2 * i, Mathf.PI / 2 * (i+1));
			else
				newV.Angle = Random.Range(0f, Mathf.PI * 2);

			newV.Magnitude = Random.Range(0f, MAX_MAGNITUDE);
			corners.Add(newV);
		}
		//corners.Sort();
		corners.Sort((v1, v2) => v1.Angle.CompareTo(v2.Angle)); // Sort by angle
		CreateMesh();

		// WHEELS
		//int numWheels = (MAX_WHEELS == MIN_WHEELS) ? MIN_WHEELS : Random.Range(MIN_WHEELS, MAX_WHEELS + 1);
		//Debug.Log("Creating a random car with " + numWheels + " wheels");

		for (int i = 0; i < MAX_WHEELS; i++) {
			if (wheels.Count < MIN_WHEELS || Random.Range(0f, 1f) < WHEEL_FREQUENCY) {
				Wheel newW = (Wheel) Instantiate(wheelPrefab, Vector2.zero, Quaternion.identity);
				newW.transform.parent = this.transform;
				newW.car = this;
				newW.CornerID = Random.Range(0, numCorners);
				newW.radius = Random.Range(MIN_WHEEL_RADIUS, MAX_WHEEL_RADIUS);

				wheels.Add(newW);

				var joint = chassis.AddComponent<WheelJoint2D>();
				joint.connectedBody = newW.GetComponent<Rigidbody2D>();
				joint.connectedAnchor = Vector2.zero;
				joint.anchor = newW.Corner.ToVector2();

				var motor = joint.motor;
				var suspension = joint.suspension;
				motor.motorSpeed = MOTOR_SPEED;
				suspension.dampingRatio = 0.9f;
				suspension.frequency = 0.5f;

				joint.motor = motor;
				joint.useMotor = true;
			}
		}

		chassis.GetComponent<Rigidbody2D>().mass = CalcChassisWeight();

		DebugMesh();
	}

	public void CreateMesh() {
		chassisMesh = new Mesh();
		int numVectors = corners.Count;

		Vector3[] vertices = new Vector3[numVectors + 1]; // last index is for center (0,0)
		int[] triangles = new int[numVectors * 3];
		
		for (int i = 0; i < numVectors; i++) {
			vertices[i] = corners[i].ToVector2();
			triangles[i * 3 + 0] = numVectors; // center
			triangles[i * 3 + 1] = i; // current vertex
			triangles[i * 3 + 2] = (i+1) % numVectors; // next vertex
		}

		vertices[numVectors] = Vector2.zero;

		chassisMesh.vertices = vertices;
		chassisMesh.triangles = triangles;
		chassis.GetComponent<MeshFilter>().mesh = chassisMesh;

		/*Vector2[] pts = new Vector2[numVectors];// = vertices.Select(x => (Vector2) x)
		for (int i = 0; i < numVectors; i++)
			pts[i] = (Vector2) vertices[i];*/
		
		chassis.GetComponent<PolygonCollider2D>().points = corners.Select(x => x.ToVector2()).ToArray();
	}

	void DebugMesh() {
		for (int i = 0; i < corners.Count; i++) {
			Vector2 pos = chassis.transform.position;
			var p1 = corners[i].ToVector2();
			var p2 = corners[(i + 1) % corners.Count].ToVector2();
			Debug.DrawLine(pos + p1, pos + p2, Color.red);
			//Debug.DrawLine(pos , pos + p1, Color.red, Mathf.Infinity);
			Debug.Log(String.Format("Mesh Point #{0} ({1}, {2})", i, p1.ToString(), p2.ToString()));
		}
	}

	float CalcChassisWeight() {
		float total = 0f;

		for (int i = 0; i < corners.Count; i++) {
			var center = Vector2.zero;
			var curr = corners[i].ToVector2();
			var next = corners[(i+1) % corners.Count].ToVector2();

			var a = (curr - center).magnitude;
			var b = (next - curr).magnitude;
			var c = (center - next).magnitude;

			var p = (a + b + c) / 2f;

			total += Mathf.Sqrt(p*(p - a)*(p - b)*(p - c));
		}
		return total;
	}
	
	void Start ()
	{
		InitRandom();
	}
	void Update ()
	{
		DebugMesh();
	}
}
