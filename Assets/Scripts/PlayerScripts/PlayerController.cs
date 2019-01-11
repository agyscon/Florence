using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = .5f;
    public float groundCheck = 1.0f;
    public float jumpForce = 5.0f;
    public float addedGravity = 0.0f;
    public float rotationSpeed = 90.0f;
    public GameObject camera;

    private Rigidbody playerRB;
    private bool isGrounded;
    private bool isAddGravity = true;
    private GameObject playerMesh;
    private float originalSpeed;
    private Vector3 deltaMovement = Vector3.zero;
    private float airSpeed = 0f;

    // Use this for initialization
    void Start () {
        playerMesh = GameObject.FindGameObjectWithTag("PlayerMesh");
        if (playerMesh == null)
        {
            Debug.Log("Player Mesh tagged object was not found in project");
        }

        playerRB = playerMesh.GetComponent<Rigidbody>();
        originalSpeed = speed;
	}

	// Update is called once per frame
	void Update () {
        // Raycast underneath to see if the object is grounded
        isGrounded = Physics.Raycast(playerMesh.transform.position, playerMesh.transform.TransformDirection(Vector3.down), groundCheck);
        //Debug.Log(isGrounded);
        Debug.DrawRay(playerMesh.transform.position, playerMesh.transform.TransformDirection(Vector3.down) * groundCheck, Color.red);

        // Jump check
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }


        Move();
       

    }

    private void FixedUpdate()
    {
        if (isAddGravity)
        {
            playerRB.AddForce(Vector3.down * addedGravity * playerRB.mass);

        }
    }

    

    private void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Move()
    {
        

        // Get keyboard inputs
        float deltaX = Input.GetAxis("Horizontal");
        float deltaY = Input.GetAxis("Vertical");
        deltaMovement = new Vector3(deltaX, 0f, deltaY);

        // Get the angle between the Camera Right Vector and the World Right (1, 0, 0)
        float angleCam2Right = Vector3.Angle(Vector3.right, camera.transform.right);

        // If the Z is positive flip the sign, I'm not entirely sure why this works tbh
        if (camera.transform.right.z > 0)
        {
            angleCam2Right *= -1;
        }
        deltaMovement = Quaternion.AngleAxis(angleCam2Right, Vector3.up) * deltaMovement;


        // Finally move the object
        if (speed != 0)
        {
            float rotStep = rotationSpeed * Time.deltaTime;
            Vector3 newHeading = Vector3.RotateTowards(playerMesh.transform.forward, deltaMovement, rotStep, 0.0f);
            playerMesh.transform.rotation = Quaternion.LookRotation(newHeading);
        }
        //playerMesh.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(deltaMovement), Mathf.Clamp(deltaMovement.magnitude, 0f, 1f));
        transform.Translate(deltaMovement * speed * Time.deltaTime);

        //Adding leftover Velocity if in air
        if (!isGrounded)
        {
            Vector3 forwardPlayerVec = Vector3.Normalize(playerMesh.transform.forward);
            transform.Translate(forwardPlayerVec * airSpeed * Time.deltaTime);
        } else
        {
            airSpeed = 0;
        }
        
    }

    public void setIsAddGravity(bool setter)
    {
        isAddGravity = setter;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void DisablePlayerControl()
    {
        playerRB.useGravity = false;
        setIsAddGravity(false);
        setSpeed(0);
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
    }

    public void EnablePlayerControl()
    {
        playerRB.useGravity = true;
        setIsAddGravity(true);
        setSpeed(originalSpeed);
    }

    public Vector3 GetHeading()
    {
        return deltaMovement;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public void SetAirSpeed(float newSpeed)
    {
        airSpeed = newSpeed;
    }

    public void resetSpeed()
    {
        speed = originalSpeed;
    }
}
