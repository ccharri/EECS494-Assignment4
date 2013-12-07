using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathingManager : MonoBehaviour {

	public Dictionary<int, Dictionary<int, PathingNode>> player1Zone;
	public Dictionary<int, Dictionary<int, PathingNode>> player2Zone;

	private Dictionary<int, Dictionary<int, PathingNode>> player1ZoneShadow;
	private Dictionary<int, Dictionary<int, PathingNode>> player2ZoneShadow;

	public int player1ZoneSize;
	public int player2ZoneSize;

	public PathingNode player1End;
	public PathingNode player2End;

	public PathingNode player1Spawn;
	public PathingNode player2Spawn;

	// Use this for initialization
	void Start () {
		//Create zones

		//Player 1
		player1Zone = makeNodes (0, 44, -10, 10);
		player1ZoneShadow = makeNodes (0, 44, -10, 10);
		player1End = player1Zone[43][0];
		player1Spawn = player1Zone[0][0];

		//Player 2
		player2Zone = makeNodes (-44, 0, -10, 10);
		player2ZoneShadow = makeNodes (-44, 0, -10, 10);
		player2End = player2Zone[-43][0];
		player2Spawn = player2Zone[0][0];


		//Calculate
		recalculate(player1Zone, player1End);
		recalculate(player1ZoneShadow, player1End);
		recalculate(player2Zone, player2End);
		recalculate(player2ZoneShadow, player2End);

		//For Show
		player1ZoneSize = player1Zone.Count;
		player2ZoneSize = player2Zone.Count;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Dictionary<int, Dictionary<int, PathingNode>> makeNodes(int xmin, int xmax, int zmin, int zmax)
	{
		Dictionary<int, Dictionary<int, PathingNode>> ret = new Dictionary<int, Dictionary<int, PathingNode>>();

		for(int x = xmin; x <= xmax; x++)
		{
			ret[x] = new Dictionary<int, PathingNode>();

			for(int z = zmin; z <= zmax; z++)
			{
				ret[x][z] = new PathingNode(x,z);
			}
		}

		return ret;
	}

	public void recalculate(Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode endNode)
	{
		int xmin, xmax;
		if(grid == player1Zone || grid == player1ZoneShadow)
		{
			xmin = 0;
			xmax = 44;
		}
		else
		{
			xmin = -44;
			xmax = 0;
		}

		Dictionary<int, Dictionary<int, PathingNode>> visitedNodes = new Dictionary<int, Dictionary<int, PathingNode>>();
		for(int i = xmin; i <= xmax; i++)
		{
			visitedNodes[i] = new Dictionary<int, PathingNode>();
		}

		List<PathingNode> enqueuedNodes = new List<PathingNode>();

		enqueuedNodes.Add(endNode);

		while(enqueuedNodes.Count > 0)
		{
			PathingNode evalNode = enqueuedNodes[0];
			enqueuedNodes.RemoveAt(0);

			int x = evalNode.x;
			int z = evalNode.z;

			if(visitedNodes[x].ContainsKey(z)) continue;
            if (!evalNode.pathable) continue;

			visitedNodes[evalNode.x][evalNode.z] = evalNode;

			enqueueNode(enqueuedNodes, grid, evalNode, x + 1, z);
			enqueueNode(enqueuedNodes, grid, evalNode, x - 1, z);
			enqueueNode(enqueuedNodes, grid, evalNode, x, z + 1);
			enqueueNode(enqueuedNodes, grid, evalNode, x, z - 1);
		}
	}

	private void enqueueNode(List<PathingNode> queue, Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode evalNode, int x, int z)
	{

		PathingNode node;

		if(!grid.ContainsKey(x)) return;
		if(!grid[x].TryGetValue(z, out node)) return;

		queue.Add(node);
		node.bestNode = evalNode;
	}


	bool pathExists(PathingNode[] transientNodes, int playerNum)
	{
		return true;
	}

//	PathingNode getNode(int x, int z, PathingNode[][] grid)
//	{
//		int xIndex;
//		int zIndex;
//		
//		if(grid == player1Zone)
//		{
//			xIndex = x;
//			zIndex = z - grid[xIndex].Length;
//		}
//		else
//		{
//			xIndex = grid.Length + x;
//			zIndex = z - grid[xIndex].Length;
//		}
//		
//		return grid[xIndex][zIndex];
//	}
}
