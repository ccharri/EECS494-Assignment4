using UnityEngine;
using System.Collections;

public class SampleTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameState g = GameState.getGameState();
        g.addPlayer(1);
        g.addPlayer(2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
