﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorGeneratorController : MonoBehaviour
{
	public StressorController stressorTemplate;
	public float generatedRadius = 20f;

	//	private int generatedCount = 0;
	[System.Obsolete]
	private float lastGeneratedTime;
	private float nextGeneratedTime = 0;

	private float currentDifficulty = float.MinValue;

	// Update is called once per frame
	void Update ()
	{
		currentDifficulty = ComputeDifficulty ();
		Debug.logger.Log (currentDifficulty);

		if (ShouldGenerate ()) {
			float angle = ComputeAngle ();
			Quaternion quaternion = Quaternion.AngleAxis (angle, Vector3.up);

			Vector3 spawnPoint = quaternion * new Vector3 (generatedRadius, 0, generatedRadius);

			float stressLevel = ComputeStressLevel ();

			StressorController stressor = (StressorController)Instantiate (stressorTemplate, spawnPoint, Quaternion.identity);

			stressor.applyForce (spawnPoint.normalized * -1 * ComputeInitialForce ());
			stressor.setStressLevel (stressLevel);

			// Kill the created stressors in 5 seconds.
			Destroy (stressor.gameObject, 3);

			nextGeneratedTime = ComputeNextGenerationTime ();
		}
	}

	// Skeleton to allow more logic to be put in here
	private float ComputeInitialForce ()
	{
		return 200f;
	}

	private bool ShouldGenerate ()
	{
		// TODO determine a way to generate multiple per frame (maybe)
		return Time.time > nextGeneratedTime;
	}

	private float ComputeAngle ()
	{
		const float difficultyAtFullRadius = 40f;
		const int minRange = 30;
		const int midpoint = 360 / 2;

		if (currentDifficulty >= difficultyAtFullRadius) {
			return Random.Range (0, 359);
		}

		float range = (midpoint - minRange) * currentDifficulty / difficultyAtFullRadius + minRange;

		return Random.Range (midpoint - range, midpoint + range);
	}

	private float ComputeStressLevel ()
	{
		float smallStressorWeight = 40;
		float mediumStressorWeight = currentDifficulty > 40 ? (currentDifficulty - 40) : 0;
		float largeStressorWeight = currentDifficulty > 80 ? (currentDifficulty - 80) : 0;

		float totalStressorWeight = smallStressorWeight + mediumStressorWeight + largeStressorWeight;

		int difficultySizeBoost = (int)currentDifficulty / 40;

		float randomGen = Random.Range (0, totalStressorWeight);

		if (randomGen < smallStressorWeight) {
			return 5 + difficultySizeBoost;
		} else if (randomGen < mediumStressorWeight) {
			return 10 + difficultySizeBoost;
		} else {
			return 15 + difficultySizeBoost;
		}
	}

	private float ComputeDifficulty ()
	{
//		Rough formula based off of ideal difficulty curves. Percise numbers should likely be tweaked.

		// Period between difficulty spikes (first spike is at period / 2)
		const float period = 10f;

		float modifiedTime = Time.time * period / (2 * Mathf.PI);

		return modifiedTime + 3 * Mathf.Sin (modifiedTime);
	}

	private float ComputeNextGenerationTime ()
	{
		const float varianceFactor = 0.2f;
		const float maxBaseline = 2.5f;

		float baselineTimeDelay = Mathf.Min(50 / currentDifficulty, maxBaseline);
//		float baselineFrequency = currentDifficulty / 50;

		float minTime = baselineTimeDelay * (1 - varianceFactor);
		float maxTime = baselineTimeDelay * (1 + varianceFactor);

		float timeUntilNext = Random.Range (minTime, maxTime);

		return Time.time + timeUntilNext;
	}
}
