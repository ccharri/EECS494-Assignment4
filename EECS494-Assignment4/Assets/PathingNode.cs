using UnityEngine;
using System.Collections;

public class PathingNode {
	public bool pathable;
	public int x;
	public int z;
    public PathingNode nextNode;
	public PathingNode bestNode;
	public int dir;

	public PathingNode(int x_, int z_)
	{
		x = x_;
		z = z_;
		pathable = true;
		bestNode = this;
	    nextNode = this;
	}

	public Vector3 getPos()
	{
		return new Vector3(x, 0, z);
	}
}
