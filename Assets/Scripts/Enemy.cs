﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health;
    public HealthBarHandler healthHandler;
	
    public void depleteHealth()
    {
        healthHandler.healthDepleter();
    }
}