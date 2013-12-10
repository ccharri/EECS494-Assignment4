using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathingAgent : MonoBehaviour {
    public Dictionary<int, Dictionary<int, PathingNode>> grid;
	public PathingNode curNode;
	public PathingNode nextNode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {/*
		if(Network.isClient) return;
		if(nextNode == null)
		{
			nextNode = curNode.bestNode;
		}

		if((transform.position.x == nextNode.x) && (transform.position.z == nextNode.z))
		{
			curNode = nextNode;
			nextNode = curNode.bestNode;
		}
		else
		{
			nextNode = curNode.bestNode;
		}*/
	}

	public Vector3 getNextPos(Vector3 pos)
	{
        curNode = grid[(int)(pos.x + 0.5f)][(int)(pos.z + 0.5f)];
        nextNode = curNode.bestNode;
		return new Vector3(nextNode.x, 0, nextNode.z);
	}
}
