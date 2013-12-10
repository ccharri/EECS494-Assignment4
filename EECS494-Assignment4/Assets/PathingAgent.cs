using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathingAgent : MonoBehaviour {
    public Dictionary<int, Dictionary<int, PathingNode>> grid;
	public PathingNode tempNode;
	public PathingNode nextNode;
    public LayerMask towerMask;
    public Vector3 origin;
    public Vector3 dir;
    public Vector3 temp;

	// Use this for initialization
	void Start ()
	{
        if (Network.isClient) nextNode = new PathingNode(25, -12);
        else nextNode = new PathingNode(-25, 12);
	    origin.y = 0.1f;
	    temp.y = 0.1f;
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
        Debug.Log("pos = " + pos.x + "," + pos.z);
	    origin.x = pos.x;
	    origin.z = pos.z;
	    temp.x = nextNode.x;
	    temp.z = nextNode.z;
	    dir = temp - origin;
	    if (Physics.Raycast(origin, dir, dir.magnitude, towerMask))
	    {
	        nextNode = grid[(int) (pos.x + 0.5f)][(int) (pos.z + 0.5f)].nextNode;
			Debug.Log ("Tower found");
	    }
	    else
	    {
			Debug.Log ("Tower not found");
	        for (tempNode = nextNode.nextNode; !nextNode.Equals(tempNode); tempNode = tempNode.nextNode)
	        {
				dir = tempNode.getPos() - origin;
	            if (Physics.Raycast(origin, dir, dir.magnitude, towerMask)) break;
	            nextNode = tempNode;
	        }
	    }

		Debug.Log("Next node = {"+nextNode.x+","+nextNode.z+"}");

	    return new Vector3(nextNode.x, 0, nextNode.z);
	}
}
