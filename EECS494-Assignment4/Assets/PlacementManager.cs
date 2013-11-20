using UnityEngine;
using System.Collections;

public class PlacementManager : MonoBehaviour {
	public bool placing = false;
	private GameObject placeObject;

	GameState gstate;

	public float gridSize = .5f;

	void Awake() 
	{
		gstate = GetComponent<GameState>();
	}

	// Use this for initialization
	void Start () {
	
	}

	public void beginPlacing(GameObject placePrefab_)
	{
		placing = true;
		placeObject = Instantiate(placePrefab_) as GameObject;
		placeObject.GetComponent<Tower>().enabled = false;
		placeObject.GetComponent<NavMeshObstacle>().enabled = false;
		placeObject.layer = 2;
	}
	
	// Update is called once per frame
	void Update () {
		if(placing)
		{
			RaycastHit rhit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhit))
			{
				Vector3 point = alignToGrid (rhit.point);
				placeObject.transform.position = point;
				
				if(Input.GetMouseButtonDown(0))
				{
					place(alignToGrid(rhit.point));
				}
			}
		}
	}

	void place(Vector3 point)
	{
		Tower t = placeObject.GetComponent<Tower>();
		string tid = t.id;

		if (t == null)
		Debug.Log ("t == null");

		Debug.Log ("t.id = " + tid);

		Destroy(placeObject);
		
		//		Instantiate(placePrefab, alignToGrid(point), Quaternion.identity);
		placing = false;
		enabled = false;

		if(Network.isServer)
		{
			gstate.tryTowerSpawn(tid, point, Network.player);
		}
		else
		{
			networkView.RPC("tryTowerSpawn", RPCMode.Server, tid, point, Network.player);
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
