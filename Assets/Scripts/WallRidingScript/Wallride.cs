using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallride : MonoBehaviour {
    public float wallCheckDist = 1.0f;
    public float wallRideSpeed = 5.0f;

    private int rideLayer;
    private int rideObjectLayer;
    private Rigidbody playerRB;
    private PlayerController playerController;
    private int rideObjectMask;
    private Vector3 wallRideDirection;
    private bool isWallRide = false;
    private Vector3 prevNormal;

    //private GameObject playerMesh;

	// Use this for initialization
	void Start ()
    { 
        rideLayer = LayerMask.NameToLayer("Rideable");
        rideObjectLayer = LayerMask.NameToLayer("RideableObject");
        rideObjectMask = LayerMask.GetMask("RideableObject");
        playerRB = GetComponent<Rigidbody>();
        playerController = GetComponentInParent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {

        // Code that shoots a ray initially to check to see if it's near a rideable wall
        RaycastHit initHit;
        Vector3 rayDirInit = Vector3.Normalize(playerController.GetHeading());
        bool initialHit = Physics.Raycast(transform.position, rayDirInit, out initHit, wallCheckDist, rideObjectMask);
        Debug.DrawRay(transform.position, rayDirInit * wallCheckDist, Color.cyan);

        // If there is a hit and the normal of the deteted wall is different from the last
        // proceed to move the reorient the object and set a forward direction
        if (initialHit && !initHit.normal.Equals(prevNormal))
        {

            playerController.DisablePlayerControl();

            // Code to determine the distance between the wall and the player
            Vector3 wall2Player = transform.position - initHit.point;
            float distance = Vector3.Dot(wall2Player, Vector3.Normalize(initHit.normal));

            // Get the vector going into the wall and use the iTween library to move the object to the wall over a time span of 1 second
            Vector3 normalDirection = Vector3.Normalize(initHit.normal) * -1;
            iTween.MoveBy(transform.parent.gameObject, normalDirection * distance, 1f);

            // Using the up and normal vector of the wall, get the vector parallel to the wall
            // and set the wall ride direction
            Vector3 faceFront = Vector3.Cross(Vector3.up, initHit.normal);
            wallRideDirection = Vector3.Normalize(faceFront);

            // Get the angle between the vector parallel to the wall and the vector of the player's heading
            // to determine whether it should be facing forwards or backwards.
            float angleFromCross2Heading = Vector3.Angle(faceFront, Vector3.Normalize(playerController.GetHeading()));
            if (angleFromCross2Heading > 90)
            {
                wallRideDirection *= -1;
            }

            // Set rotation
            transform.rotation = Quaternion.LookRotation(wallRideDirection);

            // Store the normal information 
            prevNormal = initHit.normal;

            isWallRide = true;
        }

        if (isWallRide)
        {
            RaycastHit hit;

            // Move the object in wall ride direction
            transform.parent.Translate(wallRideDirection * wallRideSpeed * Time.deltaTime);

            // Get input information from player controller to get vector of headed direction
            Vector3 rayDir = Vector3.Normalize(playerController.GetHeading());
            isWallRide = Physics.Raycast(transform.position, rayDir, out hit, wallCheckDist, rideObjectMask);
            Debug.DrawRay(transform.position, rayDir * wallCheckDist, Color.blue);

            // If the vector doesn't hit then give control back to the player
            if (!isWallRide)
            {
                playerController.EnablePlayerControl();
                playerController.resetSpeed();
                playerController.SetAirSpeed(wallRideSpeed);
                prevNormal = Vector3.up;
            }


        }
        
	}

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    // A stupid collider solution that I'm keeping because I can't let go of my problems
    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        RaycastHit hit;
        if (other.gameObject.layer == rideLayer)
        {
            playerController.DisablePlayerControl();
            Vector3 rayDir = Vector3.Normalize(playerController.GetHeading());
            Physics.Raycast(transform.position, rayDir, out hit, wallCheckDist, rideObjectMask);
            Debug.DrawRay(transform.position, rayDir * wallCheckDist, Color.blue);
            //Debug.Log(hit.collider.gameObject.name);

            Vector3 wall2Player = transform.position - hit.point;
            float compPlayer2Normal = Vector3.Dot(hit.normal, wall2Player) / Vector3.Magnitude(hit.normal);
            Vector3 projPlayer2Norm = compPlayer2Normal * Vector3.Normalize(hit.normal);
            float distance = Vector3.Dot(wall2Player, Vector3.Normalize(hit.normal));

            Vector3 normalDirection = Vector3.Normalize(hit.normal) * -1;
            iTween.MoveBy(transform.parent.gameObject, normalDirection * distance, 1f);
            //transform.parent.Translate(normalDirection * distance);

            Vector3 faceFront = Vector3.Cross( Vector3.up, hit.normal);

            //Debug.Log(faceFront);
            transform.rotation = Quaternion.LookRotation(faceFront);

            wallRideDirection = Vector3.Normalize(faceFront);
            isWallRide = true;
            //Debug.Log(transform.position);
            //transform.Translate


        }
    }
    */

 


}
