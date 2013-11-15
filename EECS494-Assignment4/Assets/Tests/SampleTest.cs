﻿using UnityEngine;
using System.Collections;

public class SampleTest : MonoBehaviour {
    public GameObject sampleTower;

	// Use this for initialization
	void Start () {
        GameState g = GameState.getGameState();
        g.addPlayer("1");
        g.addPlayer("2");

        GameObject tower = Instantiate(sampleTower, new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        Tower t = tower.GetComponent<Tower>();
        g.spawnTowerForPlayer("1", t);

        GameObject creep = Instantiate(sampleTower, new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        Creep c = creep.GetComponent<Creep>();
        c.Init(100, 10, 10, 1);
        c.Init("Infamous Crate", "2");
        g.spawnCreepForPlayer("1", c);
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
