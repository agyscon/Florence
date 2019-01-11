using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float distanceFromPlayer = 10f;
    public float rotationSpeed;
    public GameObject LookAtObject;

	// Use this for initialization
	void Start () {
        transform.LookAt(LookAtObject.transform);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -1 * Input.GetAxis("Mouse Y");
        
        //Vector of the target lookAt
        Vector3 lookAtObjectCoord = LookAtObject.transform.position;

        float distCameraToTarget = transform.position.y - lookAtObjectCoord.y;

        //Using the vector from the target->cam and target->above to get the plane the cam lies in
        Vector3 vecTarget2Cam = transform.position - lookAtObjectCoord;
        Vector3 vecTarget2Up = Vector3.up;

        //Get the Cross Prod of the above Vectors to get the vector orthogonal to the above plane
        Vector3 xOrthoCamTargetPlane = Vector3.Cross(vecTarget2Cam, vecTarget2Up);

        //Booleans to set boundaries for top and bottom for the camera
        bool cameraIsTop = distCameraToTarget > distanceFromPlayer - 0.1 && mouseY >= 0;
        bool cameraIsBot = distCameraToTarget < .1 && mouseY < 0;

        //Debug.Log("distance: " + distCameraToTarget + "    mouse Y: " + mouseY + "     Check: " + cameraIsTop);

        //Setting camera distance from the target
        transform.position = lookAtObjectCoord + (transform.position - lookAtObjectCoord).normalized * distanceFromPlayer;

        //Moving in the X Axis
        transform.RotateAround(lookAtObjectCoord, new Vector3(0.0f, 1.0f, 0.0f), rotationSpeed * Time.deltaTime * mouseX);

        //Moving in the Y axis
        if (!cameraIsTop && !cameraIsBot)
        {
            transform.RotateAround(lookAtObjectCoord, transform.right, rotationSpeed * Time.deltaTime * mouseY);

        }

        transform.LookAt(LookAtObject.transform);

    }
}
