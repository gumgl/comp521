using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

class Generation
{
	public List<Car> candidates = new List<Car>();
	public List<Car> testing = new List<Car>();
	public List<Car> tested = new List<Car>();

	/// <summary>Initialize using preset chromosomes.</summary>
	/// <param name="chromosomes">A List of chromosomes.</param>
	public void InitChromosomes(List<ArrayList> chromosomes) {
		foreach (var chromosome in chromosomes) {
			Car car = (Car) GameObject.Instantiate(God.SingleTon.CarPrefab, Vector2.zero, Quaternion.identity);
			car.position = new Vector2(0, God.MAX_VECTOR_MAGNITUDE * 2 + God.MAX_WHEEL_RADIUS);
			car.InitChromosome(chromosome);
			car.gameObject.SetActive(false);

			candidates.Add(car);
		}
	}

	/// <summary>Initialize with a completely random population.</summary>
	public void InitRandom() {
		while (candidates.Count < God.POOL_SIZE) {
			Car car = (Car) GameObject.Instantiate(God.SingleTon.CarPrefab, Vector2.zero, Quaternion.identity);
			car.position = new Vector2(0, God.MAX_VECTOR_MAGNITUDE * 2 + God.MAX_WHEEL_RADIUS);
			car.InitRandom();
			car.gameObject.SetActive(false);

			candidates.Add(car);
		}
	}

	public void StartTest() {
		SpawnNextCandidates();
	}

	public Car BestTestingCandidate() {
		if (testing == null)
			return null;

		float score = 0;
		Car best = null;
		foreach (var car in testing) {
			if (best == null || car.CalcFitness() > score) {
				best = car;
				score = car.CalcFitness();
			}
		}
		return best;
	}

	public void TestCandidate(int i) {
		var candidate = candidates[i];
		candidates.RemoveAt(i);
		testing.Add(candidate);
		candidate.Running = true;
	}

	public void FinishCandidate(int i) {
		var candidate = testing[i];
		testing.RemoveAt(i);
		tested.Add(candidate);
		candidate.Running = false;
	}

	public void SpawnNextCandidates() {
		while (candidates.Count > 0 && testing.Count < God.CONCURRENT_SIMULATIONS) {
			TestCandidate(0);
		}
	}

	public bool SimulationDone() {
		return candidates.Count == 0 && testing.Count == 0;
	}

	public bool HasMoreCandidates() {
		return candidates.Count > 0;
	}

	/// <summary>Apply evolutionary genetic algorithms to evolve the population based on their fitness score.</summary>
	/// <returns>The evolved generation</returns>
	public Generation Evolve() {
		List<Car> matingPool = TournamentSelection(tested, 2);
		//while (matingPool.Count < God.POOL_MATING_SIZE) {}
		List<Car[]> couples = Mating(matingPool);

		var chromosomes = new List<ArrayList>();

		foreach (var couple in couples)
			chromosomes.AddRange(Crossover((couple)));

		chromosomes = Mutation(chromosomes);

		foreach (var candidate in tested) {
			GameObject.Destroy(candidate.gameObject);
		}

		Generation next = new Generation();
		next.InitChromosomes(chromosomes);
		next.InitRandom(); // Fill up pool to desired size with more random candidates

		return next;
	}

	/// <summary>Selects candidates using the tournament process</summary>
	/// <param name="pool">List of candidates.</param>
	/// <param name="k">Size of each tournament (and consequently the number of winners returns).</param>
	/// <returns>The winners of each tournament</returns>
	List<Car> TournamentSelection(List<Car> pool, int k) {
		var poolCopy = new List<Car>(pool);
		var winners = new List<Car>();

		while (poolCopy.Count > 0) {
			var tournament = new List<Car>();
			for (int i = 0; i < k; i++) {
				int which = Random.Range(0, poolCopy.Count);
				tournament.Add(poolCopy[which]);
				poolCopy.RemoveAt(which);
			}
			// Sort in decreasing order, best to worst
			tournament.Sort((car1, car2) => car2.fitness.CompareTo(car1.fitness));
			winners.Add(tournament[0]);
		}

		return winners;
	}

	/// <summary>Pairs candidates randomly.</summary>
	/// <param name="matingPool">List of candidates selected for mating.</param>
	/// <returns>Candidates matched in pairs</returns>
	List<Car[]> Mating(List<Car> matingPool) {
		var couples = new List<Car[]>();

		while (matingPool.Count >= 2) { // Until there's not enough to create a couple
			Car[] couple = new Car[2];
			for (int i = 0; i < 2; i++)
			{
				int which = Random.Range(0, matingPool.Count);
				couple[i] = matingPool[which];
				matingPool.RemoveAt(which);
			}
			couples.Add(couple);
		}
		return couples;
	}

	/// <summary>Mates two candidates to obtain their offspring using the two-point crossover technique.</summary>
	/// <param name="couple">An array containing exactly two mates.</param>
	/// <returns>An array containing the chromosomes of the two offspring</returns>
	ArrayList[] Crossover(Car[] couple) {
		var chromosomes = new ArrayList[2];
		for (int i = 0; i < 2; i++)
			chromosomes[i] = couple[i].GetChromosome();

		var pt1 = Random.Range(0, chromosomes[0].Count);
		var pt2 = Random.Range(0, chromosomes[0].Count);
		if (pt2 < pt1) {// swap!
			var temp = pt1;
			pt1 = pt2;
			pt2 = temp;
		}

		ArrayList[] offspringC = new ArrayList[2];
		for (int i = 0; i < 2; i++)
			offspringC[i] = new ArrayList(chromosomes[i]);
		//for (int c = 0; c < 2; c++)
		for (int i = 0; i < chromosomes[0].Count; i++)
			if (pt1 <= i && i <= pt2) {
				offspringC[0][i] = chromosomes[0][i];
				offspringC[1][i] = chromosomes[1][i];
			} else {
				offspringC[0][i] = chromosomes[1][i];
				offspringC[1][i] = chromosomes[0][i];
			}

		return offspringC;
	}

	List<ArrayList> Mutation(List<ArrayList> chromosomes) {
		var copy = new List<ArrayList>(chromosomes);

		for (int c = 0; c < copy.Count(); c++)
			for (int i = 0; i < copy[c].Count; i++)
				if (Random.Range(0f, 1f) < God.MUTATION_RATE) // Mutate
					copy[c][i] = Car.ChromosomeRandomValue(i);

		return copy;
	}
}
