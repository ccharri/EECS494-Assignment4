using UnityEngine;
using System.Collections;

public class SampleTest : MonoBehaviour {
    public GameObject arcaneTower;
    public GameObject infamousCrate;

	// Use this for initialization
	void Start () {
		GameState g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        g.addPlayer(Network.player);
        //g.addPlayer("2");

        GameObject tower = Instantiate(arcaneTower, new Vector3(0, 1, 0), new Quaternion()) as GameObject;
        ArcaneTower t = tower.GetComponent<ArcaneTower>();
        t.Init(Network.player.guid);
        g.spawnTowerForPlayer(Network.player.guid, t);

        GameObject creep = Instantiate(infamousCrate, new Vector3(10, 1, 0), new Quaternion()) as GameObject;
        Creep c = creep.GetComponent<Creep>();
        c.Init(100, 10, 10, 1);
        c.Init("Infamous Crate", Network.player.guid);
        g.spawnCreepForPlayer(Network.player.guid, c);
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
