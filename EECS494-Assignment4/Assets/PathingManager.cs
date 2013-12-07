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

	void Awake()
	{
		//Create zones
		
		//Player 1
		player1Zone = makeNodes (-25, 25, 2, 22);
        player1ZoneShadow = makeNodes(-25, 25, 2, 22);
		player1End = player1Zone[-25][12];
		player1Spawn = player1Zone[25][12];
		
		//Player 2
        player2Zone = makeNodes(-25, 25, -2, -22);
        player2ZoneShadow = makeNodes(-25, 25, -2, -22);
		player2End = player2Zone[25][-12];
		player2Spawn = player2Zone[-25][-12];
		
		
		//Calculate
		recalculate(player1Zone, player1End);
		recalculate(player1ZoneShadow, player1End);
		recalculate(player2Zone, player2End);
		recalculate(player2ZoneShadow, player2End);
		
		//For Show
		player1ZoneSize = player1Zone.Count;
		player2ZoneSize = player2Zone.Count;
	}

	// Use this for initialization
	void Start () {

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
			xmin = -25;
			xmax = 25;
		}
		else
		{
			xmin = -25;
			xmax = 25;
		}

		Dictionary<int, Dictionary<int, PathingNode>> visitedNodes = new Dictionary<int, Dictionary<int, PathingNode>>();
		for(int i = xmin; i <= xmax; i++)
		{
			visitedNodes[i] = new Dictionary<int, PathingNode>();
		}

		Queue<PathingNode> enqueuedNodes = new Queue<PathingNode>();

		enqueuedNodes.Enqueue(endNode);
		visitedNodes[endNode.x][endNode.z] = endNode;

		while(enqueuedNodes.Count > 0)
		{
			PathingNode evalNode = enqueuedNodes.Dequeue();

			int x = evalNode.x;
			int z = evalNode.z;

            if (!evalNode.pathable) continue;

			enqueueNode(enqueuedNodes, visitedNodes, grid, evalNode, x + 1, z, 4);
			enqueueNode(enqueuedNodes, visitedNodes, grid, evalNode, x - 1, z, 2);
			enqueueNode(enqueuedNodes, visitedNodes, grid, evalNode, x, z + 1, 3);
			enqueueNode(enqueuedNodes, visitedNodes, grid, evalNode, x, z - 1, 1);
		}
	
//		if(grid == player1Zone)
//		{
//			Debug.Log("-----------------------------------");
//			string val = "";
//			for(int i = 10; i >= -10; i--)
//			{
//
//
//				for(int j = xmin; j <= xmax; j++)
//				{
//					if(grid[j][i] == player1End)
//						val += "E";
//					else
//						val += grid[j][i].dir;
//				}
//				val += "\n";
//
//			}
//			Debug.Log (val);
//		}
	}

	private void enqueueNode(Queue<PathingNode> queue, Dictionary<int, Dictionary<int, PathingNode>> visitedNodes, Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode evalNode, int x, int z, int dir)
	{

		PathingNode node;

		if(!grid.ContainsKey(x)) return;
		if(!grid[x].TryGetValue(z, out node)) return;

		if(visitedNodes[x].ContainsKey(z)) return;
		
		visitedNodes[x][z] = node;

		queue.Enqueue(node);
		node.bestNode = evalNode;
		node.dir = dir;
	}


	public bool pathExists(PathingNode[] transientNodes, int playerNum)
	{
		return true;
	}

	public void turnOn(float x, float z)
	{
		Debug.Log("Turning on nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);

		Dictionary<int, Dictionary<int, PathingNode>> dict = x > 0 ? player1Zone : player2Zone;
		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = x > 0 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode end = x > 0 ? player1End : player2End;

		for(int i = xmin; i <= xmax; i++)
		{
			for(int j = zmin; j <= zmax; j++)
			{
				Dictionary<int, PathingNode> inDict;

				if(!dict.TryGetValue(i, out inDict)) continue;
				if(!inDict.ContainsKey(j)) continue;

				dict[i][j].pathable = true;
				dictShadow[i][j].pathable = true;
			}
		}

		recalculate(dict, end);
		recalculate(dictShadow, end);
	}

	public void turnOff(float x, float z)
	{
		Debug.Log("Turning off nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);
		
		Dictionary<int, Dictionary<int, PathingNode>> dict = x > 0 ? player1Zone : player2Zone;
		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = x > 0 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode end = x > 0 ? player1End : player2End;
		
		for(int i = xmin; i <= xmax; i++)
		{
			for(int j = zmin; j <= zmax; j++)
			{
				Dictionary<int, PathingNode> inDict;
				
				if(!dict.TryGetValue(i, out inDict)) continue;
				if(!inDict.ContainsKey(j)) continue;

				dict[i][j].pathable = false;
				dictShadow[i][j].pathable = false;
			}
		}
		
		recalculate(dict, end);
		recalculate(dictShadow, end);
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
