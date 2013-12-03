using UnityEngine;
using System.Collections;

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

	GameState gstate;

	public float gridSize = .5f;

	void Awake() 
	{
		gstate = GetComponent<GameState>();
		NavMeshAgent spawnAgent = gstate.spawnLocation.GetComponent<NavMeshAgent>();
		spawnAgent.ResetPath();
		NavMeshPath path = new NavMeshPath();
		spawnAgent.CalculatePath(gstate.getEndPoint(), path);
		
		while(spawnAgent.pathPending)
		{
		}
		
		Vector3[] p = path.corners;
		renderer.SetVertexCount(p.Length);
		for(int i = 0 ; i < p.Length; i++)
		{
			renderer.SetPosition(i, p[i]);
		}

	}

	// Use this for initialization
	void Start () {
	
	}

	public void beginPlacing(GameObject placePrefab_, string id_)
	{
		if(placing) return;

		placing = true;
		ready = false;
//		shift = (Input.GetKeyDown("shift"));
		placeObject = Instantiate(placePrefab_) as GameObject;
		placeObject.networkView.enabled = false;
		placeObject.GetComponent<Tower>().enabled = false;
		helper = placeObject.AddComponent<PlacementHelper>();
		helper.validMaterial = validMaterial;
		helper.invalidMaterial = invalidMaterial;

		var obstacles = placeObject.GetComponentsInChildren<NavMeshObstacle>();
		foreach(NavMeshObstacle obs in obstacles)
		{
			obs.enabled = false;
		}

		placeObject.layer = 2;
		id = id_;
	}
	
	// Update is called once per frame
	void Update () {
		if(placing)
		{
			renderer.enabled = true;
			if(Input.GetMouseButtonUp(0))
			{
				ready = true;
			}

//			shift = Input.GetKeyDown("shift");
			RaycastHit rhit;
//			int layerMask = LayerMask.NameToLayer("Buildable");
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhit/*, Camera.main.farClipPlane, layerMask*/))
			{
				Vector3 point = alignToGrid (rhit.point);
				placeObject.transform.position = point;
				
				if(Input.GetMouseButtonDown(0) && ready && helper.valid)
				{
					place(alignToGrid(rhit.point));
				}
			}
		}
		else
		{
			renderer.enabled = false;
		}
	}

	void place(Vector3 point)
	{
		Tower t = placeObject.GetComponent<Tower>();
		string tid = t.id;

		if (t == null)
		Debug.Log ("t == null");

		Debug.Log ("t.id = " + id);

		Destroy(placeObject);
		
		//		Instantiate(placePrefab, alignToGrid(point), Quaternion.identity);
		if(!shift)
		{
			placing = false;
		}
		ready = false;
		enabled = false;

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
		rval.x = ((int)((Mathf.Abs(point.x) / gridSize) + .5)) * gridSize * (Mathf.Abs (point.x)/point.x) ;
		rval.z = ((int)((Mathf.Abs(point.z) / gridSize) + .5)) * gridSize * (Mathf.Abs(point.z)/point.z) ;
		return rval;
	}
}
