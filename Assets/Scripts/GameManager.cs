﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameMode
	{
		Growth,
		Survival
	}

	public AmoebaManager ProtectedAmoeba;
	public DamageBar DamageBar;
	public UnityEngine.UI.Text GrowthLevelText;
	public CanvasGroup[] GameOverGroup;

	public static float MaxLashoutLength = 5f;

	public static float LashoutDuration { get; private set; }

	public static float GrowthLevelIncrement = 10f;
	public static float OverallDifficultyFactor = 0.5f;
	public static GameMode CurrentGameMode;

	public static int GrowthLevel { get; private set; }

	public static float CurrentDifficulty { get; private set; }

	private float gameOverTime = -1f;

	void Update ()
	{
		CurrentDifficulty = ComputeTimeBasedDifficulty();

		GrowthLevel = (int) Mathf.Floor (ProtectedAmoeba.MaxStressLevel / GrowthLevelIncrement);
		GrowthLevelText.text = GrowthLevel.ToString ();

		if (ProtectedAmoeba.IsLashingOut && gameOverTime < 0) {
			LashoutDuration += Time.deltaTime;

			if (LashoutDuration > MaxLashoutLength) {
				TriggerGameOver ();
			} else {
				DamageBar.percent = 1f - LashoutDuration / MaxLashoutLength;
			}
		} else if (gameOverTime > 0 && Time.timeScale > 0) {
			if (Time.timeScale > 0) {
				float newScale = 1f - Mathf.Sqrt (Time.realtimeSinceStartup - gameOverTime);
				Time.timeScale = Mathf.Max (newScale, 0f);
			}

			// TODO fix problem where alpha doesn't technically reach 1.0 before we stop entering this branch
			float newAlpha = (Time.realtimeSinceStartup - gameOverTime);
			foreach (CanvasGroup group in GameOverGroup) {
				group.alpha = newAlpha;
			}
		}
	}

	void TriggerGameOver ()
	{
		// TODO implement effects outside of this class
		DamageBar.percent = 0;
		gameOverTime = Time.realtimeSinceStartup;
	}

	private static float ComputeTimeBasedDifficulty ()
	{
		// Period (in sec) between difficulty spikes (first spike is at period / 2)
		const float period = 25f;
		const float periodAmplitude = 8f;
		const float startingDifficulty = 10f;

		const float difficultyIncreasePerPeriod = 4f;
		const float difficultyIncreasePerGrowthLevel = 10f;

		if (CurrentGameMode == GameMode.Growth) {
			return OverallDifficultyFactor *
			(startingDifficulty + difficultyIncreasePerGrowthLevel * GrowthLevel
			- periodAmplitude * Mathf.Cos (Time.time * 2 * Mathf.PI / period));
		} else {
			return OverallDifficultyFactor *
			(startingDifficulty + Time.time * difficultyIncreasePerPeriod / period
			- periodAmplitude * Mathf.Cos (Time.time * 2 * Mathf.PI / period));
		}
	}
}
