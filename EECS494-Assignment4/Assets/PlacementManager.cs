using UnityEngine;
using System.Collections;

public class PlacementManager : MonoBehaviour {
	public GameObject placePrefab;
	public bool placing = false;
	private GameObject placeObject;
	
	public float gridSize = .5f;

	// Use this for initialization
	void Start () {
	
	}

	public void beginPlacing(GameObject placePrefab_)
	{
		placePrefab = placePrefab_;
		placing = true;
		placeObject = (GameObject)Instantiate(placePrefab);
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
					place(rhit.point);
				}
			}
		}
	}

	void place(Vector3 point)
	{
		Destroy(placeObject);
		placeObject = null;
		Instantiate(placePrefab, alignToGrid(point), Quaternion.identity);
		placing = false;
		enabled = false;
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
