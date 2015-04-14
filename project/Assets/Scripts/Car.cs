using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
	//public God god;
	public Wheel wheelPrefab;
	public GameObject chassis;
	private Mesh chassisMesh;

	public List<VectorP> corners = new List<VectorP>();
	public List<Wheel> wheels = new List<Wheel>();
	public Vector2 lastPosition = Vector2.zero;
	public float idleTime = 0;

	public float fitness { get; private set; }

	public Vector2 position {
		get { return chassis.transform.position; }
		set { transform.position = value; }
	}

	private bool running = true;
	public bool Running {
		get { return running; }
		set {
			running = value;
			foreach (var component in GetComponents<WheelJoint2D>())
				component.useMotor = value;
			gameObject.SetActive(value);
		}
	}

	/// <summary>Needed for the two-point crossover technique</summary>
	/// <returns>A sequence of numeric values making up the chromosome</returns>
	public ArrayList GetChromosome() {
		var sequence = new ArrayList();
		foreach (var corner in corners) {
			sequence.Add(corner.Angle);
			sequence.Add(corner.Magnitude);
		}
		for (int i = 0; i < God.MAX_WHEELS; i++) { // Make sure every chromosome is the same size
			if (i < wheels.Count) { // We actually have a wheel
				sequence.Add(wheels[i].CornerID);
				sequence.Add(wheels[i].Radius);
			} else { // We don't have a wheel, blank values
				sequence.Add(-1);
				sequence.Add(0f);
			}
		}
		return sequence;
	}

	public void InitChromosome(ArrayList sequence) {
		int s = 0;
		for (int i = 0; i < God.NUM_VECTORS; i++) {
			var v = new VectorP();
			v.Angle = (float) sequence[s++];
			v.Magnitude = (float) sequence[s++];
			corners.Add(v);
		}
		corners.Sort((v1, v2) => v1.Angle.CompareTo(v2.Angle));
		CreateMesh();
		for (int i = 0; i < God.MAX_WHEELS; i++) {
			int cornerID = (int) sequence[s++];
			float radius = (float) sequence[s++];
			if (cornerID != -1) // We actually have a wheel
				AddWheel(cornerID, radius);
		}
		Running = false;
	}
	/// <summary>Mutates a single variable in a chromosome.</summary>
	/// <param name="k">The position in the chromosome.</param>
	/// <returns>The new random value</returns>
	public static object ChromosomeRandomValue(int k) {
		if (k * 2 < God.NUM_VECTORS) // Corners
			if (k%2 == 0)
				return Random.Range(0f, 2*Mathf.PI);
			else
				return Random.Range(0f, God.MAX_VECTOR_MAGNITUDE);
		else // Wheels
			if (k % 2 == 0)
				return Random.Range(-1, God.NUM_VECTORS);
			else
				return Random.Range(God.MIN_WHEEL_RADIUS, God.MAX_WHEEL_RADIUS);

	}

	public void InitRandom() {
		corners.Clear();

		// VECTORS
		//int numCorners = (God.MAX_VECTORS == God.MIN_VECTORS) ? God.MIN_VECTORS : Random.Range(God.MIN_VECTORS, God.MAX_VECTORS + 1);
		int numCorners = God.NUM_VECTORS;

		for (int i = 0; i < numCorners; i++) {
			var newV = new VectorP();

			if (i<4) // add one vector in each quadrant. otherwise the (0,0) is outside the car
				newV.Angle = Random.Range(Mathf.PI / 2 * i, Mathf.PI / 2 * (i+1));
			else
				newV.Angle = Random.Range(0f, Mathf.PI * 2);

			newV.Magnitude = Random.Range(0f, God.MAX_VECTOR_MAGNITUDE);
			corners.Add(newV);
		}
		corners.Sort((v1, v2) => v1.Angle.CompareTo(v2.Angle)); // Sort by angle
		CreateMesh();

		// WHEELS
		//int numWheels = (MAX_WHEELS == MIN_WHEELS) ? MIN_WHEELS : Random.Range(MIN_WHEELS, MAX_WHEELS + 1);
		//Debug.Log("Creating a random car with " + numWheels + " wheels");

		for (int i = 0; i < God.MAX_WHEELS; i++) {
			if (wheels.Count < God.MIN_WHEELS || Random.Range(0f, 1f) < God.WHEEL_FREQUENCY) {
				var cornerID = Random.Range(0, numCorners);
				var radius = Random.Range(God.MIN_WHEEL_RADIUS, God.MAX_WHEEL_RADIUS);
				AddWheel(cornerID, radius);
			}
		}

		Running = false;
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

		chassis.GetComponent<Rigidbody2D>().mass = CalcChassisWeight();
		/*Vector2[] pts = new Vector2[numVectors];// = vertices.Select(x => (Vector2) x)
		for (int i = 0; i < numVectors; i++)
			pts[i] = (Vector2) vertices[i];*/
		
		chassis.GetComponent<PolygonCollider2D>().points = corners.ConvertAll(input => input.ToVector2()).ToArray();
	}

	void AddWheel(int cornerID, float radius) {
		Wheel newW = (Wheel)Instantiate(wheelPrefab, Vector2.zero, Quaternion.identity);
		newW.transform.parent = this.transform;
		newW.car = this;
		newW.CornerID = cornerID;
		newW.Radius = radius;

		wheels.Add(newW);

		var joint = chassis.AddComponent<WheelJoint2D>();
		joint.connectedBody = newW.GetComponent<Rigidbody2D>();
		joint.connectedAnchor = Vector2.zero;
		joint.anchor = newW.Corner.ToVector2();

		var motor = joint.motor;
		var suspension = joint.suspension;
		motor.motorSpeed = God.MOTOR_SPEED;
		suspension.dampingRatio = 0.9f;
		suspension.frequency = 0.5f;

		joint.motor = motor;
		joint.useMotor = true;
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

	public float CalcFitness() {
		fitness = position.x;
		return fitness;
	}

	public bool IsIdle {
		get {
			var val = (position - lastPosition).magnitude/Time.deltaTime;
			//Debug.Log("Moving by " + val.ToString());
			return val < God.IDLE_MIN_SPEED;
		}
	}

	public bool HasStopped {
		get { return idleTime > God.IDLE_MAX_TIME; }
	}
	
	void Start ()
	{

	}
	void Update () {
		//DebugMesh();
		if (Running)
		{
			if (IsIdle)
				idleTime += Time.deltaTime;
			else
				idleTime = 0;
		}
		lastPosition = position;
	}

	void LateUpdate() {
	}
}
