using UnityEngine;
using System.Collections;

public class PathingAgent : MonoBehaviour {
	public PathingNode lastNode;
	public PathingNode nextNode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(nextNode == null)
		{
			nextNode = lastNode.bestNode;
		}

		if((transform.position.x == nextNode.x) && (transform.position.z == nextNode.z))
		{
			lastNode = nextNode;
			nextNode = lastNode.bestNode;
		}
		else
		{
			nextNode = lastNode.bestNode;
		}
	}

	public Vector3 getNextPos()
	{
		return new Vector3(nextNode.x, 0, nextNode.z);
	}
}
