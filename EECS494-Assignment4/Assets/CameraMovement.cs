using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	bool rightDown;
	GameObject mainCamera;
	float scrollArea = 25.0f;
	float scrollSpeed = 25.0f;
    float rotationSpeed = 0.50f;

    float maxY = 25;
    float minY = -25;
    float maxX = 25;
    float minX = -25;

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

        if(mPosX <= scrollArea || Input.GetKey(KeyCode.LeftArrow)) 
		{
            if(mainCamera.transform.position.x > minX) 
				mainCamera.transform.position += (mainCamera.transform.right * -scrollSpeed * Time.deltaTime);
		}
		if(mPosX >= Screen.width-scrollArea || Input.GetKey(KeyCode.RightArrow))
		{
			if(mainCamera.transform.position.x < maxX)
				mainCamera.transform.position += (mainCamera.transform.right * scrollSpeed * Time.deltaTime);
		}
		
		if(mPosY <= scrollArea || Input.GetKey(KeyCode.UpArrow))
		{
            if(mainCamera.transform.position.y < maxY)
			    mainCamera.transform.position += (Vector3.Cross(mainCamera.transform.right, Vector3.up) * -scrollSpeed * Time.deltaTime);
		}
		if(mPosY >= Screen.height-scrollArea || Input.GetKey(KeyCode.DownArrow))
		{
            if(mainCamera.transform.position.y < minY)
			    mainCamera.transform.position += (Vector3.Cross(mainCamera.transform.right, Vector3.up) * scrollSpeed * Time.deltaTime);
		}

        if(Input.GetKey(KeyCode.RightControl))
        {
            rotateCamera(mainCamera, -rotationSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.LeftControl))
        {
            rotateCamera(mainCamera, rotationSpeed * Time.deltaTime);
            
        }
	}

    void rotateCamera(GameObject camera, float radians)
    {
        Vector3 lookAt = getCameraLookAt(mainCamera, 0);
        float xzRadius = new Vector3(lookAt.x - camera.transform.position.x, 0, lookAt.z - camera.transform.position.z).magnitude;
        Debug.Log("radius: " + xzRadius);
        float xzAngle = Mathf.Atan2(-lookAt.z + camera.transform.position.z, -lookAt.x + camera.transform.position.x);
        Debug.Log("angle: " + xzRadius);
        Vector3 newPos = new Vector3(lookAt.x + xzRadius * Mathf.Cos(xzAngle + radians), 
                                     mainCamera.transform.position.y, 
                                     lookAt.z + xzRadius * Mathf.Sin(xzAngle + radians));
        mainCamera.transform.position = newPos;
        Debug.Log("pos: " + newPos);
        mainCamera.transform.LookAt(lookAt);
    }

    Vector3 getCameraLookAt(GameObject camera, float yHeight)
    {
        Vector3 cameraVector = camera.transform.forward.normalized;
        float yDiff = camera.transform.position.y - yHeight;
        float xzScaling = yDiff / -cameraVector.y;
        return new Vector3(camera.transform.position.x + cameraVector.x * xzScaling, 
                            yHeight, 
                            camera.transform.position.z + cameraVector.z * xzScaling);
    }

	void LateUpdate()
	{
		/*
		if(rightDown) //1 = right mouse button
		{
			float MouseX = Input.GetAxis("Mouse X");
			float MouseY = Input.GetAxis("Mouse Y");
			Vector3 CameraPos = mainCamera.transform.right * MouseX;
			CameraPos = CameraPos + (Vector3.Cross (mainCamera.transform.right, Vector3.up) * MouseY);
			 
			mainCamera.transform.position += CameraPos;
		}
		*/
	}
}
