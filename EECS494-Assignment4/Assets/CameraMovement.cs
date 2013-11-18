using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	bool rightDown;
	GameObject mainCamera;

	// Use this for initialization
	void Start () 
	{
		rightDown = false;
		mainCamera = GameObject.FindWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update ()
	{
		rightDown = Input.GetMouseButton(1);
	}

	void LateUpdate()
	{
		if(rightDown) //1 = right mouse button
		{
			float MouseX = Input.GetAxis("Mouse X");
			float MouseY = Input.GetAxis("Mouse Y");
			Vector3 CameraPos = new Vector3(- MouseX, 0, - MouseY);
			 
			mainCamera.transform.position += CameraPos;
		}
	}
}
