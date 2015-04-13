using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
	private const int MIN_VECTORS = 8;
	private const int MAX_VECTORS = 8;
	public GameObject body;
	private Mesh bodyMesh;

	private List<VectorP> corners;

	void init() {
		
	}

	void initRandom() {
		int numVectors = (MAX_VECTORS == MIN_VECTORS) ? MIN_VECTORS : Random.Range(MIN_VECTORS, MAX_VECTORS + 1);

		for (int i = 0; i < numVectors; i++) {
			VectorP newP = new VectorP();
		}
	}
	
	void Start ()
	{
	
	}
	void Update ()
	{
	
	}
}
