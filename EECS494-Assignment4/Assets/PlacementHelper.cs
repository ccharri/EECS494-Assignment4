using UnityEngine;
using System.Collections;

public class PlacementHelper : MonoBehaviour {
	public bool valid = true;
	public Material validMaterial;
	public Material invalidMaterial;
	public int collisionCount = 0;

	// Use this for initialization
	void Start () {
		markValid();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision info)
	{
		addCollision();
	}

	void OnCollisionExit(Collision info)
	{
		removeCollision();
	}

	void OnCollisionStay(Collision info)
	{
		markInvalid();
	}

	void OnTriggerEnter(Collider info)
	{
		addCollision();
	}

	void OnTriggerExit(Collider info)
	{
		removeCollision();
	}

	void OnTriggerStay(Collider info)
	{
		markInvalid();
	}

	void addCollision()
	{
		collisionCount++;
		if(valid)
			markInvalid();
	}

	void removeCollision()
	{
		collisionCount--;
		
		if(collisionCount == 0)
		{
			markValid();
		}
	}

	void markValid()
	{
		valid = true;
		gameObject.GetComponent<MeshRenderer>().material = validMaterial;
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in renderers)
		{
			render.material = validMaterial;
		}
	}

	void markInvalid()
	{
		valid = false;
		gameObject.GetComponent<MeshRenderer>().material = invalidMaterial;
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in renderers)
		{
			render.material = invalidMaterial;
		}
	}
}
