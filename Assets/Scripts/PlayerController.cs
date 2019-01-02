using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = .5f;
    public float groundCheck = 1.0f;
    public float jumpForce = 5.0f;
    public float addedGravity = 0.0f;
    public GameObject camera;

    private Rigidbody playerRB;
    private bool isGrounded;

    // Use this for initialization
    void Start () {
        playerRB = GetComponent<Rigidbody>();
		
	}

	// Update is called once per frame
	void Update () {
        // Raycast underneath to see if the object is grounded
        isGrounded = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), groundCheck);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * groundCheck, Color.red);

        // Jump check
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Get keyboard inputs
        float deltaX = Input.GetAxis("Horizontal");
        float deltaY = Input.GetAxis("Vertical");
        Vector3 deltaMovement = new Vector3(deltaX, 0f, deltaY);

        // Get the angle between the Camera Right Vector and the World Right (1, 0, 0)
        float angleCam2Right = Vector3.Angle(Vector3.right, camera.transform.right);

        // If the Z is positive flip the sign, I'm not entirely sure why this works tbh
        if (camera.transform.right.z > 0)
        {
               angleCam2Right *= -1;
        }
        deltaMovement = Quaternion.AngleAxis(angleCam2Right, Vector3.up) * deltaMovement;


        // Finally move the object
        transform.Translate(deltaMovement * speed * Time.deltaTime);
		
	}

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            playerRB.AddForce(Vector3.down * addedGravity * playerRB.mass);

        }
    }

    private void Jump()
    {   

        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
