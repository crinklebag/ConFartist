﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tootorial : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1;
            this.gameObject.SetActive(false);
        }	
	}
}
