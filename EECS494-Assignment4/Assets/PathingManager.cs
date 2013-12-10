using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathingManager : MonoBehaviour {

	public Dictionary<int, Dictionary<int, PathingNode>> player1Zone;
	public Dictionary<int, Dictionary<int, PathingNode>> player2Zone;

	public Dictionary<int, Dictionary<int, PathingNode>> player1ZoneShadow;
	public Dictionary<int, Dictionary<int, PathingNode>> player2ZoneShadow;

	public int player1ZoneSize;
	public int player2ZoneSize;

	public PathingNode player1End;
	public PathingNode player2End;
	public PathingNode player1EndShadow;
	public PathingNode player2EndShadow;

	public PathingNode player1Spawn;
	public PathingNode player2Spawn;
	public PathingNode player1SpawnShadow;
	public PathingNode player2SpawnShadow;

	public bool recalculateNow = true;

    public LayerMask towerMask;

    public Stack<PathingNode> path;

	void Awake()
	{
        path = new Stack<PathingNode>();
		//Create zones
		
		//Player 1
		player1Zone = makeNodes (-25, 25, 2, 22);
		player1End = player1Zone[-25][12];
		player1Spawn = player1Zone[25][12];

		player1ZoneShadow = makeNodes(-25, 25, 2, 22);
		player1EndShadow = player1ZoneShadow[-25][12];
        player1SpawnShadow = player1ZoneShadow[25][12];
		
		//Player 2
        player2Zone = makeNodes(-25, 25, -22, -2);
		player2End = player2Zone[25][-12];
        player2Spawn = player2Zone[-25][-12];

		player2ZoneShadow = makeNodes(-25, 25, -22, -2);
		player2EndShadow = player2ZoneShadow[25][-12];
        player2SpawnShadow = player2ZoneShadow[-25][-12];
		
		//Calculate
		recalculate(player1Zone, player1End);
		recalculate(player1ZoneShadow, player1End);
		recalculate(player2Zone, player2End);
		recalculate(player2ZoneShadow, player2End);
		
        //Initiate bestNodes
        initBest(player1Zone, player1End, -25, 25, 2, 22);
        initBest(player1ZoneShadow, player1EndShadow, -25, 25, 2, 22);
        initBest(player2Zone, player2End, -25, 25, -22, -2);
        initBest(player2ZoneShadow, player2EndShadow, -25, 25, -22, -2);

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

    public void initBest(Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode endNode, int xmin, int xmax, int zmin, int zmax)
    {
        for (int x = xmin; x <= xmax; x++)
        {
            for (int z = zmin; z <= zmax; z++)
            {
                grid[x][z].bestNode = endNode;
            }
        }
    }

	public void recalculate(Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode endNode)
	{
		int zmin, zmax;
		if(grid == player1Zone || grid == player1ZoneShadow)
		{
			zmin = 2;
			zmax = 22;
		}
		else
		{
			zmin = -22;
            zmax = -2;
		}

		Dictionary<int, Dictionary<int, PathingNode>> visitedNodes = new Dictionary<int, Dictionary<int, PathingNode>>();
		for(int i = -25; i <= 25; i++)
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

        for (int i = -25; i <= 25; i++)
        {
            for (int j = zmin; j <= zmax; j++)
            {
                if (!visitedNodes[i].ContainsKey(j)) continue;

                PathingNode temp = visitedNodes[i][j].bestNode;
                PathingNode last = visitedNodes[i][j];
                Vector3 origin = new Vector3(last.x, 0.1f, last.z);
                Vector3 dest = new Vector3(temp.x, 0.1f, temp.z);
                Vector3 dir = dest - origin;
                if (Physics.Raycast(origin, dir, dir.magnitude, towerMask)) //back up
                {
                    last = visitedNodes[i][j].nextNode;
                    for (temp = last.nextNode; (!temp.Equals(endNode)); temp = temp.nextNode)
                    {
                        dest.x = temp.x;
                        dest.z = temp.z;
                        dir = dest - origin;
                        if (Physics.Raycast(origin, dir, dir.magnitude, towerMask)) break;
                        last = temp;
                    }
                    visitedNodes[i][j].bestNode = last;

                    /*path.Clear();
                    PathingNode prevBest = temp;
                    last = visitedNodes[i][j].nextNode;
                    if (temp == last) break;
                    for (temp = visitedNodes[i][j].nextNode; !temp.Equals(prevBest); temp = temp.nextNode)
                    {
                        path.Push(temp);
                        //Debug.Log("pushing node " + temp.x + "," + temp.z);
                    }
                    for (temp = path.Pop(); !temp.Equals(last); temp = path.Pop())
                    {
                        //Debug.Log("popped node" + temp.x + "," + temp.z);
                        dest.x = temp.x;
                        dest.z = temp.z;
                        dir = dest - origin;
                        if (!Physics.Raycast(origin, dir, dir.magnitude, towerMask)) break;
                    }
                    visitedNodes[i][j].bestNode = temp;*/
                }
                else //go forward
                {
                    last = temp;
                    for (temp = temp.nextNode; (!temp.Equals(endNode)); temp = temp.nextNode)
                    {
                        dest.x = temp.x;
                        dest.z = temp.z;
                        dir = dest - origin;
                        if (Physics.Raycast(origin, dir, dir.magnitude, towerMask)) break;
                        last = temp;
                    }
                    visitedNodes[i][j].bestNode = last;
                }
            }
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
		node.nextNode = evalNode;
		node.dir = dir;
	}


	public bool pathExists(Dictionary<int, Dictionary<int, PathingNode>> grid, PathingNode start, PathingNode end)
	{
		int count = 0;
		int numNodes = 1000;
		PathingNode next = start;

		while(count <= numNodes)
		{
			if(next == end) return true;
			if(!next.pathable) return false;
			next = next.nextNode;
			count++;
		}
		return false;
	}

	public void turnOn(float x, float z)
	{
		Debug.Log("Turning on nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);

		bool isPlayer1 = z > 0;

		Dictionary<int, Dictionary<int, PathingNode>> dict =  isPlayer1 ? player1Zone : player2Zone;
		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = isPlayer1 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode end = isPlayer1 ? player1End : player2End;
		PathingNode endShadow = isPlayer1 ? player1EndShadow : player2EndShadow;

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

		if(recalculateNow)
		{
			recalculate(dict, end);
			recalculate(dictShadow, endShadow);
		}
	}

	public void turnOff(float x, float z)
	{
		Debug.Log("Turning off nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);

		bool isPlayer1 = z > 0;
		
		Dictionary<int, Dictionary<int, PathingNode>> dict = isPlayer1 ? player1Zone : player2Zone;
		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = isPlayer1 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode end = isPlayer1 ? player1End : player2End;
		PathingNode endShadow = isPlayer1 ? player1EndShadow : player2EndShadow;
		
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

		if(recalculateNow)
		{
			recalculate(dict, end);
			recalculate(dictShadow, endShadow);
		}
	}

	public void turnOnShadow(float x, float z)
	{
		Debug.Log("Turning on nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);

		bool isPlayer1 = z > 0;
		
		Dictionary<int, Dictionary<int, PathingNode>> dict = isPlayer1 ? player1Zone : player2Zone;
		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = isPlayer1 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode endShadow = isPlayer1 ? player1EndShadow : player2EndShadow;
		
		for(int i = xmin; i <= xmax; i++)
		{
			for(int j = zmin; j <= zmax; j++)
			{
				Dictionary<int, PathingNode> inDict;
				
				if(!dict.TryGetValue(i, out inDict)) continue;
				if(!inDict.ContainsKey(j)) continue;
				
				dictShadow[i][j].pathable = dict[i][j].pathable;
			}
		}

		if(recalculateNow) recalculate(dictShadow, endShadow);
	}
	
	public void turnOffShadow(float x, float z)
	{
		Debug.Log("Turning off nodes near ("+x+","+z+")");
		int xmin = Mathf.FloorToInt(x);
		int xmax = Mathf.CeilToInt(x);
		int zmin = Mathf.FloorToInt(z);
		int zmax = Mathf.CeilToInt(z);

		bool isPlayer1 = z > 0;

		Dictionary<int, Dictionary<int, PathingNode>> dictShadow = isPlayer1 ? player1ZoneShadow : player2ZoneShadow;
		PathingNode end = isPlayer1 ? player1EndShadow : player2EndShadow;
		
		for(int i = xmin; i <= xmax; i++)
		{
			for(int j = zmin; j <= zmax; j++)
			{
				Dictionary<int, PathingNode> inDict;
				
				if(!dictShadow.TryGetValue(i, out inDict)) continue;
				if(!inDict.ContainsKey(j)) continue;

				dictShadow[i][j].pathable = false;
			}
		}

		if(recalculateNow) recalculate(dictShadow, end);
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
