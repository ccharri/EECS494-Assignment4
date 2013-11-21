using UnityEngine;
using System.Collections;

public class TestAnimation : MonoBehaviour {

	public AnimationClip idle;

	// Use this for initialization
	void Start () {
		animation.AddClip(idle, "Idle");
		animation.wrapMode = WrapMode.Loop;
		animation.CrossFade("Idle");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
