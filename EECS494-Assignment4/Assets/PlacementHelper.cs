using UnityEngine;
using System.Collections;

public class PlacementHelper : MonoBehaviour {
	public bool valid;
	public Material validMaterial;
	public Material invalidMaterial;
	public int collisionCount = 0;
	public Vector3 position;
	public bool checking = false;
	public LineRenderer renderer;

	private bool validCollision;
	private bool validPath;

	// Use this for initialization
	void Start () {
		validCollision = true;
		validPath = true;
		renderer = GameState.getInstance().pMan.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
//		if((position.x != gameObject.transform.position.x) || (position.z != gameObject.transform.position.z))
		{
			refreshBlockingValidity();
			position = gameObject.transform.position;
		}

		valid = validCollision && validPath;

		if(valid)
		{
			markValid ();
		}
		else
		{
			markInvalid ();
		}
	}

	public Vector3 getDestination()
	{
		return GameState.getInstance().getEndPoint();
	}

	bool filterOut(string tag)
	{
		if(tag == "Buildable") return true;
		if(tag == "Creep") return true;
		if(tag == "Projectile") return true;
		return false;
	}

	void OnCollisionEnter(Collision info)
	{
		if(filterOut(info.gameObject.tag)) return;

		addCollision();
	}

	void OnCollisionExit(Collision info)
	{
		if(filterOut(info.gameObject.tag)) return;

		removeCollision();
	}

	void OnCollisionStay(Collision info)
	{
		if(filterOut(info.gameObject.tag)) return;

		markInvalid();
	}

	void OnTriggerEnter(Collider info)
	{
		if(filterOut(info.gameObject.tag)) return;

		addCollision();
	}

	void OnTriggerExit(Collider info)
	{
		if(filterOut(info.gameObject.tag)) return;

		removeCollision();
	}

	void OnTriggerStay(Collider info)
	{
		if(filterOut(info.gameObject.tag)) return;

		validCollision = false;
	}

	void addCollision()
	{
		collisionCount++;
		if(valid)
			validCollision = false;
	}

	void removeCollision()
	{
		collisionCount--;
		
		if(collisionCount == 0)
		{
			validCollision = true;
		}
	}

	public void refreshValidity()
	{
		refreshCollisionValidity();
		refreshBlockingValidity();
	}

	public void refreshCollisionValidity()
	{
		if(collisionCount == 0)
		{
			validCollision = true;
		}
		else {
			validCollision = false;
		}
	}

	public void refreshBlockingValidity()
	{
		if(checking) return;

		turnOnColliders();
		
		checking = true;
		
		NavMeshPath path = new NavMeshPath();
		NavMeshAgent spawnAgent = GameState.getInstance().spawnLocation.GetComponent<NavMeshAgent>();
		spawnAgent.ResetPath();
		bool hasPath = spawnAgent.CalculatePath(getDestination(), path);
//		Debug.Log("hasPath = " + hasPath + ", path.status = " + path.status);
		
		while(spawnAgent.pathPending)
		{
		}
		
		Vector3[] p = path.corners;
		renderer.SetVertexCount(p.Length);
		for(int i = 0 ; i < p.Length; i++)
		{
			renderer.SetPosition(i, p[i]);
		}
		
		hasPath = hasPath && (path.status == NavMeshPathStatus.PathComplete);
		//Debug.Log("--hasPath = " + hasPath + ", path.status = " + path.status);
		
//		turnOffColliders();
		
		validPath = hasPath;
		
		checking = false;

		//StartCoroutine("checkPath", (valid));
	}

	IEnumerator checkPath(bool validity)
	{
		turnOnColliders();
		
		checking = true;
		
		NavMeshPath path = new NavMeshPath();
		NavMeshAgent spawnAgent = GameState.getInstance().spawnLocation.GetComponent<NavMeshAgent>();
		spawnAgent.ResetPath();
		bool hasPath = spawnAgent.CalculatePath(getDestination(), path);
//		Debug.Log("hasPath = " + hasPath + ", path.status = " + path.status);
		
		while(spawnAgent.pathPending)
		{
		}

		Vector3[] p = path.corners;
		renderer.SetVertexCount(p.Length);
		for(int i = 0 ; i < p.Length; i++)
		{
			renderer.SetPosition(i, p[i]);
		}

		hasPath = hasPath && (path.status == NavMeshPathStatus.PathComplete);
//		Debug.Log("--hasPath = " + hasPath + ", path.status = " + path.status);
		
		turnOffColliders();

		if(validity && hasPath)
		{
			markValid();
		}
		else 
		{
			markInvalid();
		}
		
		checking = false;

		yield return new WaitForFixedUpdate();
	}

	void turnOffColliders()
	{
		var obstacles = gameObject.GetComponentsInChildren<NavMeshObstacle>();
		foreach(NavMeshObstacle obs in obstacles)
		{
			obs.enabled = false;
		}
	}

	void turnOnColliders()
	{
		var obstacles = gameObject.GetComponentsInChildren<NavMeshObstacle>();
		foreach(NavMeshObstacle obs in obstacles)
		{
			obs.enabled = true;
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

//		turnOnColliders();
	}

	void markInvalid()
	{
		valid = false;
		var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer render in renderers)
		{
			render.material = invalidMaterial;
		}

//		turnOffColliders();
	}
}
