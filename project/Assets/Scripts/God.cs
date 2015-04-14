using UnityEngine;
using System.Collections;

public class God : MonoBehaviour {
	public const int NUM_VECTORS = 8;
	public const int MIN_WHEELS = 1;
	public const int MAX_WHEELS = 3;
	public const float WHEEL_FREQUENCY = 0.5f;
	public const float MAX_VECTOR_MAGNITUDE = 2;
	public const float MIN_WHEEL_RADIUS = 0.2f;
	public const float MAX_WHEEL_RADIUS = 1f;
	public const float MOTOR_SPEED = -500;
	/// <summary>If car is moving at less than this, it is considered idle.</summary>
	public const float IDLE_MIN_SPEED = 0.1f;
	public const float IDLE_MAX_TIME = 5;
	public const int POOL_SIZE = 12;
	//public const int POOL_MATING_SIZE = 6;
	public const float MUTATION_RATE = 0.05f;
	public const float CONCURRENT_SIMULATIONS = 3;

	public static God SingleTon = null;
	public Car CarPrefab;
	public UnityEngine.UI.Text GenCountText;
	public UnityEngine.UI.Text BestScoreText;
	public UnityEngine.UI.Text CurrScoreText;
	public Camera camera;

	private Generation generation;
	private float bestScore = 0;

	private int genCount;
	private int GenCount {
		get { return genCount; }
		set {
			genCount = value;
			GenCountText.text = value.ToString();
		}
	}

	void Start () {
		SingleTon = this;
		GenCount = 1;

		generation = new Generation();
		generation.InitRandom();
		generation.StartTest();
	}

	void Update() {
		float currScore = generation.CurrCar.CalcFitness();
		if (currScore > bestScore)
			bestScore = currScore;

		CurrScoreText.text = currScore.ToString("0.0");
		BestScoreText.text = bestScore.ToString("0.0");
		CameraFollowCar();

		if (generation.CurrCar.HasStopped) {
			Debug.Log("Car has stopped!");
			generation.RemoveCurrCandidate();
			if (generation.HasNextCandidate()) // Same generation
				generation.SpawnNextCandidate();
			else { // Next generation
				generation = generation.Evolve();
				genCount++;
				generation.StartTest();
			}
		}
	}

	void CameraFollowCar() {
		if (generation != null && generation.CurrCar != null) {
			var pos = camera.transform.position;
			var target = generation.CurrCar.position + Vector2.up*2;
			pos.x = target.x;
			pos.y = target.y;

			camera.transform.position = pos;
		}
			
	}
}
