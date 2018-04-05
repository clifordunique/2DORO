﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour {

    public bool enemyInside = false;
    public GameObject enemyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {
            enemyInside = true;
            enemyObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "enemy")
        {
            enemyInside = false;
            enemyObject = null;
        }
    }

}
