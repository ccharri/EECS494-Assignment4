using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementManager : MonoBehaviour {
	public bool placing = false;
	public bool ready = false;
	public bool shift = false;
	public PlacementHelper helper;
	public Material validMaterial;
	public Material invalidMaterial;
	private GameObject placeObject;
	private string id;
	public LineRenderer renderer = new LineRenderer();
	public List<PathingNode> path;

	GameState gstate;

	public float gridSize = 1f;

	public LayerMask myLayerMask;

	void Awake() 
	{
		gstate = GetComponent<GameState>();
	}

	// Use this for initialization
	void Start () {
		updatePath();

	}

	public void beginPlacing(GameObject placePrefab_, string id_)
	{
		if(placing) return;

		placing = true;
		ready = false;
//		shift = (Input.GetKeyDown("shift"));
		GameState.getInstance().pathMan.recalculateNow = false;
		placeObject = Instantiate(placePrefab_) as GameObject;
		placeObject.networkView.enabled = false;
		placeObject.GetComponent<Tower>().enabled = false;
		placeObject.GetComponent<PathingObstacle>().enabled = false;
		GameState.getInstance().pathMan.recalculateNow = true;
		helper = placeObject.AddComponent<PlacementHelper>();
		helper.man = this;
		helper.validMaterial = validMaterial;
		helper.invalidMaterial = invalidMaterial;

		placeObject.layer = 2;
		id = id_;
	}

	public void updatePath()
	{
		PathingNode start;
		PathingNode end;
		
		if(Network.isServer)
		{
			start = GameState.getInstance().pathMan.player1SpawnShadow;
			end = GameState.getInstance().pathMan.player1EndShadow;
		}
		else
		{
			start = GameState.getInstance().pathMan.player2SpawnShadow;
			end = GameState.getInstance().pathMan.player2EndShadow;
		}
		
		path = new List<PathingNode>();
		PathingNode next = start.nextNode;
		path.Add(next);
		int count = 0;
		while(next != end && count < 1000)
		{
			next = next.nextNode;
			path.Add(next);
			count++;
		}
		
		renderer.SetVertexCount(path.Count);
		for(int i = 0 ; i < path.Count; i++)
		{
			renderer.SetPosition(i, new Vector3(path[i].x, 0, path[i].z));
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			Destroy(placeObject);
			placing = false;
			//ready = false;
			//enabled = false;
		}

		if(placing)
		{
			renderer.enabled = true;
			if(Input.GetMouseButtonUp(0))
			{
				ready = true;
			}

//			shift = Input.GetKeyDown("shift");
			RaycastHit rhit;
			//int layerMask = LayerMask.NameToLayer("Buildable");
			//layerMask = ~layerMask;

			Camera cam = Camera.main;

			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhit, Camera.main.farClipPlane, myLayerMask))
			{
				Vector3 point = alignToGrid (rhit.point);
				placeObject.transform.position = point;
				
				if(Input.GetMouseButtonDown(0) && ready && helper.valid)
				{
					place(point);
					//place(alignToGrid(rhit.point));
				}
			}
		}
		else
		{
			updatePath();
//			renderer.enabled = false;
		}
	}

	void OnGUI()
	{
		shift = Input.GetKey(KeyCode.LeftShift);
//		Debug.Log ("shift = " + shift);
	}

	void place(Vector3 point)
	{
		Tower t = placeObject.GetComponent<Tower>();
		string tid = t.id;

//		if (t == null)
//		Debug.Log ("t == null");
//
//		Debug.Log ("t.id = " + id);

		//		Instantiate(placePrefab, alignToGrid(point), Quaternion.identity);		

		if(!shift)
		{
			Destroy(placeObject);
			placing = false;
			ready = false;
		}

		if(Network.isServer)
		{
			gstate.tryTowerSpawn(id, point, Network.player);
		}
		else
		{
			networkView.RPC("tryTowerSpawn", RPCMode.Server, id, point, Network.player);
		}
	}
	
	Vector3 alignToGrid(Vector3 point)
	{
		Vector3 rval;
		rval.y = 0;
		rval.x = ((int)((Mathf.Abs(point.x) / gridSize) + .5)) * gridSize * (Mathf.Abs (point.x)/point.x) + .5f ;
		rval.z = ((int)((Mathf.Abs(point.z) / gridSize) + .5)) * gridSize * (Mathf.Abs(point.z)/point.z)  + .5f;
		return rval;
	}
}
