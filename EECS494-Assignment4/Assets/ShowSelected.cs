using UnityEngine;
using System.Collections;

public class ShowSelected : MonoBehaviour {

	// Use this for initialization
	float width;
	float height;
	public Material selectedMaterial;
	GameObject selected;

	void Start()
	{
		selected = new GameObject("Selected");
		width = 2.0f;
		height = 2.0f;
	}

	void Update() {
		GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
		//Debug.Log("Ran Show Selected Script");
		foreach (GameObject tower in towers)
		{
			Debug.Log("Entered Loop in Selected Script");

			Tower towerScript = (Tower)tower.GetComponent("Tower");

			if (!towerScript.selected)
			{
				continue;
			}
			
			tower.transform.parent = selected.transform;
			
			gameObject.AddComponent("MeshFilter");
	   		gameObject.AddComponent("MeshRenderer");

			MeshFilter mf = GetComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			mf.mesh = mesh;

			Renderer renderer = GetComponent<MeshRenderer>().renderer;

			renderer.material = selectedMaterial;

			Vector3[] vertices = new Vector3[4];

			Vector3 pos = tower.transform.position;
			vertices[0] = new Vector3(pos.x + width, 4, pos.z - height);
			vertices[1] = new Vector3(pos.x + width, 4, pos.z + height);
			vertices[2] = new Vector3(pos.x - width, 4, pos.z - height);
			vertices[3] = new Vector3(pos.x - width, 4, pos.z + height);

/*
			vertices[0] = new Vector3(pos.x, 4, pos.z);
			vertices[1] = new Vector3(pos.x - 5, 4, pos.z + 5);
			vertices[2] = new Vector3(pos.x - 5, 4, pos.z - 5);
			vertices[3] = new Vector3(pos.x , 4, pos.z );*/

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
			uv[1] = new Vector2(1, 0);
			uv[2] = new Vector2(0, 1);
			uv[3] = new Vector2(1, 1);

			mesh.uv = uv;
		}
	}
}
