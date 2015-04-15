using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class God : MonoBehaviour {
	public const int NUM_VECTORS = 8;
	public const int MIN_WHEELS = 1;
	public const int MAX_WHEELS = 8;
	public const float WHEEL_FREQUENCY = 0.2f;
	public const float MAX_VECTOR_MAGNITUDE = 2;
	public const float MIN_WHEEL_RADIUS = 0.2f;
	public const float MAX_WHEEL_RADIUS = 1f;
	public const float MOTOR_SPEED = -800;
	/// <summary>If car is moving at less than this, it is considered idle.</summary>
	public const float IDLE_MIN_SPEED = 0.1f;
	public const float IDLE_MAX_TIME = 2.5f;
	public const int POOL_SIZE = 20;
	//public const int POOL_MATING_SIZE = 6;
	public const float MUTATION_RATE = 0.03f;
	public const float CONCURRENT_SIMULATIONS = 8;
	public const float TERRAIN_MAX_DELTA = 100;
	public const float TERRAIN_MAX_SLOPE = 45;
	public const float TERRAIN_SEGMENT_LENGTH = 2.3f;
	public const int TERRAIN_EASY_SEGMENTS = 50;

	public static God SingleTon = null;
	public Car CarPrefab;
	public GameObject GroundPrefab;
	public UnityEngine.UI.Text GenCountText;
	public UnityEngine.UI.Text BestScoreText;
	public UnityEngine.UI.Text CurrScoreText;
	public UnityEngine.UI.Text TestingStateText;
	public Camera camera;

	private Generation generation;
	private float bestScore = 0;
	private List<GameObject> groundPieces = new List<GameObject>();

	private int genCount;
	public int GenCount {
		get { return genCount; }
		private set {
			genCount = value;
			GenCountText.text = value.ToString();
		}
	}

	void Start () {
		SingleTon = this;
		GenCount = 1;

		FileLogger.Instance.Open("stats.txt", false);

		BuildTerrain();

		generation = new Generation();
		generation.InitRandom();
		generation.StartTest();
	}

	void Update() {
		if (!generation.SimulationDone()) {
			var best = generation.BestTestingCandidate();
			float currScore = best.CalcFitness();
			if (currScore > bestScore)
				bestScore = currScore;

			TestingStateText.text = String.Format("{0}/{1}", generation.tested.Count, generation.numSubjects);

			CurrScoreText.text = currScore.ToString("0.0");
			BestScoreText.text = bestScore.ToString("0.0");
			CameraFollowCar();
		}

		for (int i=0; i<generation.testing.Count; i++) {
			if (generation.testing[i].IsIdle && generation.testing[i].idleTime > 0.2f)
				generation.testing[i].chassis.GetComponent<Renderer>().material.color = Color.gray;
			else
				generation.testing[i].chassis.GetComponent<Renderer>().material.color = Color.yellow;

			if (generation.testing[i].HasStopped) {
				Debug.Log("Car has stopped!");
				generation.FinishCandidate(i);
				if (generation.HasMoreCandidates())
					generation.SpawnNextCandidates();
				else if (generation.SimulationDone()) {
					Debug.Log("Simulation over! Evolution...");
					generation = generation.Evolve();
					GenCount++;
					generation.StartTest();
				}
				break;
			}
		}
	}

	void CameraFollowCar() {
		var best = generation.BestTestingCandidate();
		if (best != null) {
			var pos = camera.transform.position;
			var target = best.position + Vector2.up*2;
			pos.x = target.x;
			pos.y = target.y;

			camera.transform.position = Vector3.Lerp(camera.transform.position, pos, Time.deltaTime * 2 );
		}
			
	}

	void BuildTerrain() {
		Vector2 prevP = Vector2.right * -3;
		float prevA = 0;
		for (int i = 0; i < 1000; i++) {
			var maxDelta = i < TERRAIN_EASY_SEGMENTS ? (float) i / TERRAIN_EASY_SEGMENTS * TERRAIN_MAX_DELTA : TERRAIN_MAX_DELTA;
			var maxSlope = i < TERRAIN_EASY_SEGMENTS ? (float)i / TERRAIN_EASY_SEGMENTS * TERRAIN_MAX_SLOPE : TERRAIN_MAX_SLOPE;
			var angleMin = Mathf.Max(-maxSlope * Mathf.Deg2Rad, prevA - maxDelta * Mathf.Deg2Rad);
			var angleMax = Mathf.Min(maxSlope * Mathf.Deg2Rad, prevA + maxDelta * Mathf.Deg2Rad);
			var angle = Random.Range(angleMin, angleMax);
			var vectorP = new VectorP(angle, TERRAIN_SEGMENT_LENGTH);

			var newP = prevP + vectorP.ToVector2();
			var midpoint = (newP + prevP) / 2;

			Debug.DrawLine(prevP, newP, Color.blue, Mathf.Infinity);

			GameObject ground = (GameObject) Instantiate(GroundPrefab, Vector3.zero, Quaternion.identity);

			ground.transform.position = midpoint;

			ground.transform.localScale = new Vector3(vectorP.Magnitude, 0.4f, 1f);

			var tmp = ground.transform.localEulerAngles;
			tmp.z = angle * Mathf.Rad2Deg;
			ground.transform.localEulerAngles = tmp;
			prevA = angle;
			prevP = newP;
		}
	}
}
