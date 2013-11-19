using UnityEngine;
using System.Collections;

public class PlacementManagerWithNetworkIntegration : MonoBehaviour {
	public GameObject placePrefab;
	public bool placing = false;
	private GameObject placeObject;

	public float gridSize = 10;

	// Use this for initialization
	void Start () {
	
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
					place(rhit.point);
				}
			}
		}
	}

	void OnGUI() {
		if(!placing)
		{
			if(GUI.Button(new Rect(20, Screen.height-70, 100, 50), "Place a box"))
			{
				placing = true;
				placeObject = (GameObject)Instantiate(placePrefab);
				//We want this local
				placeObject.networkView.enabled = false;
				placeObject.layer = 2;
			}
		}
	}

	void place(Vector3 point)
	{
		Destroy(placeObject);
		placeObject = null;
		placing = false;

		if(Network.isClient)
			networkView.RPC("PlaceBlock", RPCMode.Server, point);
		else
			PlaceBlock(point);
	}
	
	Vector3 alignToGrid(Vector3 point)
	{
		Vector3 rval;
		rval.y = 0;
		rval.x = ((int)((Mathf.Abs(point.x) / gridSize) + .5)) * gridSize * (Mathf.Abs (point.x)/point.x) ;
		rval.z = ((int)((Mathf.Abs(point.z) / gridSize) + .5)) * gridSize * (Mathf.Abs(point.z)/point.z) ;
		return rval;
	}

	[RPC]
	void PlaceBlock(Vector3 point, NetworkMessageInfo info)
	{
		Debug.Log ("PlaceBlock received from " + info.sender);
		PlaceBlock (point);
	}

	void PlaceBlock(Vector3 point)
	{
		Vector3 gridPoint = alignToGrid(point);

		//Do Model Checks
		
		Network.Instantiate(placePrefab, gridPoint, Quaternion.identity, 0);
	}
}
