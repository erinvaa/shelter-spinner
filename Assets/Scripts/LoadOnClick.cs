﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadOnClick : MonoBehaviour
{

	public void LoadScene (int scene)
	{
		SceneManager.LoadSceneAsync (scene);
	}
}