using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour {
	public Vector3 position;

	// Use this for initialization
	void Start () {
		position = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = position;
	}
}
