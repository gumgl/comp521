using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
	private const int MIN_VECTORS = 8;
	private const int MAX_VECTORS = 8;
	private const float MAX_MAGNITUDE = 3;
	public GameObject body;
	private Mesh bodyMesh;

	private List<VectorP> corners = new List<VectorP>();

	public void Init() {
		
	}

	public void InitRandom() {
		corners.Clear();
		int numVectors = (MAX_VECTORS == MIN_VECTORS) ? MIN_VECTORS : Random.Range(MIN_VECTORS, MAX_VECTORS + 1);

		for (int i = 0; i < numVectors; i++) {
			VectorP newV = new VectorP();
			newV.angle = Random.Range(0f, Mathf.PI * 2);
			newV.magnitude = Random.Range(0f, MAX_MAGNITUDE);
			corners.Add(newV);
		}
		//corners.Sort();
		corners.Sort((v1, v2) => v1.angle.CompareTo(v2.angle)); // Sort by angle

		CreateMesh();

		DebugMesh();
	}

	public void CreateMesh() {
		bodyMesh = new Mesh();
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

		bodyMesh.vertices = vertices;
		bodyMesh.triangles = triangles;
		body.GetComponent<MeshFilter>().mesh = bodyMesh;
	}

	void DebugMesh() {
		for (int i = 0; i < corners.Count; i++) {
			Vector2 pos = body.transform.position;
			var p1 = corners[i].ToVector2();
			var p2 = corners[(i + 1)%corners.Count].ToVector2();
			Debug.DrawLine(pos + p1, pos + p2, Color.red, Mathf.Infinity);
			Debug.Log(String.Format("Mesh Point #{0} ({1}, {2})", i, p1.ToString(), p2.ToString()));
		}
	}
	
	void Start ()
	{
		InitRandom();
	}
	void Update ()
	{
	
	}
}
