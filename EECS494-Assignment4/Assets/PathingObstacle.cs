using UnityEngine;
using System.Collections;

public class PathingObstacle : MonoBehaviour {
	void OnEnable()
	{
		GameState.getInstance().pathMan.turnOff(transform.position.x, transform.position.z);
	}

	void OnDisable()
	{
		GameState.getInstance().pathMan.turnOn(transform.position.x, transform.position.z);
	}
}
