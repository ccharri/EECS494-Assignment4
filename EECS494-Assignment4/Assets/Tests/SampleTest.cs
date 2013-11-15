using UnityEngine;
using System.Collections;

public class SampleTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameState g = GameState.getGameState();
        g.addPlayer(1);
        g.addPlayer(2);


        Tower t = new ArcaneTower();
        t.prefab = Instantiate(Resources.Load("Towers/ArcaneTower"), new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        g.spawnTowerForPlayer(1, t);

        Creep c = new Creep(100, 10, 10, 1);
        c.prefab = Instantiate(Resources.Load("Creeps/InfamousCrate"), new Vector3(2, 2, 0), new Quaternion()) as GameObject;
        g.spawnCreepForPlayer(1, c); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
