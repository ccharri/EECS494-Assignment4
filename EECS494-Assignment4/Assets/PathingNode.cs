using UnityEngine;
using System.Collections;

public class PathingNode {
	public bool pathable;
	public int x;
	public int z;
	public PathingNode bestNode;

	public PathingNode(int x_, int z_)
	{
		x = x_;
		z = z_;
		pathable = true;
		bestNode = this;
	}
}
