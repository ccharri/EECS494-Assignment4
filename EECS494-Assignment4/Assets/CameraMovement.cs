using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	bool rightDown;
	GameObject mainCamera;
	float scrollArea = 25.0f;
	float scrollSpeed = 25.0f;

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

		var mPosX = Input.mousePosition.x;
		var mPosY = Input.mousePosition.y;


		if (mPosX <= scrollArea) 
		{
			mainCamera.transform.position += (mainCamera.transform.right * -scrollSpeed * Time.deltaTime);
		}
		if (mPosX >= Screen.width-scrollArea)
		{
			mainCamera.transform.position += (mainCamera.transform.right * scrollSpeed * Time.deltaTime);
		}
		if (mPosY <= scrollArea)
		{
			mainCamera.transform.position += (Vector3.Cross(mainCamera.transform.right, Vector3.up) * -scrollSpeed * Time.deltaTime);
		}
		if (mPosY >= Screen.height-scrollArea)
		{
			mainCamera.transform.position += (Vector3.Cross(mainCamera.transform.right, Vector3.up) * scrollSpeed * Time.deltaTime);
		}
	}

	void LateUpdate()
	{
		if(rightDown) //1 = right mouse button
		{
			float MouseX = Input.GetAxis("Mouse X");
			float MouseY = Input.GetAxis("Mouse Y");
			Vector3 CameraPos = new Vector3(- MouseY, 0, MouseX);
			 
			mainCamera.transform.position += CameraPos;
		}
	}
}
