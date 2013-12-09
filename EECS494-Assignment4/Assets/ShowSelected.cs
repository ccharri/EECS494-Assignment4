using UnityEngine;
using System.Collections;

public class ShowSelected : MonoBehaviour {

	// Use this for initialization
	float width;
	float height;
	public Material selectedMaterial;
	public LayerMask selectedMask;
	public Tower selected; //Can make this a normal GameObject if you want to expand this to creeps
	MeshFilter mf;

	void Start()
	{
		//selected = new GameObject("Selected");
		
		width = 2.0f;
		height = 2.0f;
		gameObject.AddComponent("MeshFilter");
	   	gameObject.AddComponent("MeshRenderer");
	}

	void Update() {
		RaycastHit rhit;
		Renderer renderer = GetComponent<MeshRenderer>().renderer;

		if(!Input.GetMouseButtonDown(0)) //Minor issue: If you place a tower with your mouse over the tower, it is selected immediately
		{
			return;
		}

		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rhit, Camera.main.farClipPlane, selectedMask))
		{
			//selected = rhit.transform.gameObject;
			selected = rhit.collider.gameObject.GetComponent<Tower>();
			renderer.enabled = true;
		}
		else
		{
			selected = null;
			renderer.enabled = false;
			return;
		}

		/*
		Tower towerScript = (Tower)tower.GetComponent("Tower");

		if (!towerScript.selected)
		{
			continue;
		}
		*/
		
		//tower.transform.parent = selected.transform;

		
	   	mf = GetComponent<MeshFilter>();
		
		Mesh mesh = new Mesh();
		mf.mesh = mesh;

		

		renderer.material = selectedMaterial;

		Vector3[] vertices = new Vector3[4];

		Vector3 pos = selected.transform.position;

		vertices[0] = new Vector3(pos.x + 5, 4, pos.z + 1); //ul
		vertices[1] = new Vector3(pos.x + 1, 4, pos.z + 1); //ll
		vertices[2] = new Vector3(pos.x + 5, 4, pos.z - 2.5f); //ur
		vertices[3] = new Vector3(pos.x + 1, 4, pos.z - 2.5f); //lr
		
		mesh.vertices = vertices;

		int[] tri = new int[6];

		tri[0] = 0;
		tri[1] = 2;
		tri[2] = 1;

		tri[3] = 2;
		tri[4] = 3;
		tri[5] = 1;

		mesh.triangles = tri;

		Vector3[] normals = new Vector3[4];

		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;
		normals[3] = Vector3.up;

		mesh.normals = normals;

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1f, 0);
		uv[2] = new Vector2(0, 1f);
		uv[3] = new Vector2(1f, 1f);

		mesh.uv = uv;	
	}
}
