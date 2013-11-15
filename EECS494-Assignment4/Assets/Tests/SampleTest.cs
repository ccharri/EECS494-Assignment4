using UnityEngine;
using System.Collections;

public class SampleTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameState g = GameState.getGameState();
        g.addPlayer(1);
        g.addPlayer(2);

        GameObject tower = Instantiate(Resources.Load("Towers/ArcaneTower"), new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        GameObject creep = Instantiate(Resources.Load("Creeps/InfamousCrate"), new Vector3(0, 0, 0), new Quaternion()) as GameObject;
        Creep c = creep.GetComponent<Creep>();
        c.Init(100, 10, 10, 1);
        c.Init("Infamous Crate", 2);

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
