using UnityEngine;
using System.Collections;

public class PlacementHelper : MonoBehaviour {
	public bool valid;
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
		if(info.gameObject.tag == "Buildable") return;

		addCollision();
	}

	void OnCollisionExit(Collision info)
	{
		if(info.gameObject.tag == "Buildable") return;

		removeCollision();
	}

	void OnCollisionStay(Collision info)
	{
		if(info.gameObject.tag == "Buildable") return;

		markInvalid();
	}

	void OnTriggerEnter(Collider info)
	{
		if(info.gameObject.tag == "Buildable") return;

		addCollision();
	}

	void OnTriggerExit(Collider info)
	{
		if(info.gameObject.tag == "Buildable") return;

		removeCollision();
	}

	void OnTriggerStay(Collider info)
	{
		if(info.gameObject.tag == "Buildable") return;

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
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in renderers)
		{
			render.material = validMaterial;
		}
	}

	void markInvalid()
	{
		valid = false;
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in renderers)
		{
			render.material = invalidMaterial;
		}
	}
}
