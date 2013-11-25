using UnityEngine;
using System.Collections;

public class ZGridMaker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		new List<GameObject> ZGrid;
		for (int i = -138; i < 140; i += 2)
		{
			LineRenderer renderer;
			new GameObject line;
			renderer = gameObject.AddComponent<LineRenderer>();
			renderer.useWorldSpace = true;
			renderer.material = 
			ZGrid.Add();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
