using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControls : MonoBehaviour {
    
    // linear speed
    private const float LINEAR_FORCE = 5f;

    // Maximum angular speed in degrees per second
    private const float ANGULAR_SPEED = 360f;

    // Controls jump height
    private const float JUMP_FORCE = 5.000f;

    private bool canJump;
    private float jumpCooldownTimer;

    private Rigidbody playerRigidbody;
    //private Animator anim;
    public AudioClip[] stepSounds;

    void Awake() {
        playerRigidbody = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
        canJump = false;
        jumpCooldownTimer = 0f;
    }

    public void Update() {
        if (jumpCooldownTimer >= 0) {
            jumpCooldownTimer -= Time.deltaTime;
        } else {
            if (Mathf.Abs(playerRigidbody.velocity.y) <= 0.002) {
                canJump = true;
                //anim.SetBool("Airborne", false);
            }
        }
        if (Input.GetKey("space")) {
            Jump();
        }
    }

    public void PlayerMove(Vector3 cameraVector) {
        if (cameraVector.y != 0) {
            cameraVector.Set(cameraVector.x, 0f, cameraVector.z);
        }

        // vector representing the normal out of the camera with y component removed
        Vector3 cameraToPlayer = cameraVector.normalized * -1;

        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.magnitude == 0) {
            //anim.SetBool("Walking", false);
            //anim.SetFloat("Y", 0f);
            return;
        } else {
            //anim.SetBool("Walking", true);
        }
        if (inputVector.magnitude > 1f) {
            inputVector.Normalize();
        }

        // angle of the cameraToPlayer vector with respect to forward vector
        float cameraAngle = Vector3.Angle(Vector3.forward, cameraToPlayer);
        if (cameraToPlayer.x < 0) {
            cameraAngle *= -1;
        }
        // angle of the input vector in the xz plane
        float inputAngle = Vector3.Angle(Vector3.forward, inputVector);
        if (inputVector.x < 0) {
            inputAngle *= -1;
        }
        // Input angle transformed with respect to the camera angle
        float worldSpaceInputAngle = cameraAngle + inputAngle;
        float worldSpaceInputAngleRad = worldSpaceInputAngle * Mathf.Deg2Rad;
        Vector3 worldSpaceInputVector = new Vector3(Mathf.Sin(worldSpaceInputAngleRad) * inputVector.magnitude, 0f, Mathf.Cos(worldSpaceInputAngleRad) * inputVector.magnitude);

        float playerOrientationAngle = transform.rotation.eulerAngles.y;
        float playerOrientationAngleRad = playerOrientationAngle * Mathf.Deg2Rad;
        Vector3 playerOrientationVector = new Vector3(Mathf.Sin(playerOrientationAngleRad), 0, Mathf.Cos(playerOrientationAngleRad)).normalized;

        float moveMagnitude = Vector3.Dot(worldSpaceInputVector, playerOrientationVector);
        if (moveMagnitude < 0) {
            moveMagnitude = 0;
        }
        //anim.SetFloat("Y", moveMagnitude);


        // Complicated way of figuring out which way to turn. Only sort of works
        worldSpaceInputAngle = (worldSpaceInputAngle + 360f) % 360f;
        playerOrientationAngle = (playerOrientationAngle + 360f) % 360f;
        float targetOrientationAngle;
        if (playerOrientationAngle > worldSpaceInputAngle) {
            playerOrientationAngle -= 360f;
        }
        // if CW direction is shortest
        if (worldSpaceInputAngle - playerOrientationAngle < 180f) {
            targetOrientationAngle = Mathf.Min(playerOrientationAngle + ANGULAR_SPEED * Time.deltaTime, worldSpaceInputAngle);
        } else { // if CCW direction is shortest
            targetOrientationAngle = Mathf.Max(playerOrientationAngle - ANGULAR_SPEED * Time.deltaTime, worldSpaceInputAngle);
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, targetOrientationAngle, 0));

        transform.position = new Vector3(transform.position.x + moveMagnitude * Time.deltaTime * worldSpaceInputVector.x * LINEAR_FORCE, transform.position.y, transform.position.z + moveMagnitude * Time.deltaTime * worldSpaceInputVector.z * LINEAR_FORCE);
        //playerRigidbody.AddForce(moveMagnitude * worldSpaceInputVector * LINEAR_FORCE);
    }

    private void Jump() {
        if (canJump) {
            playerRigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
            canJump = false;
            jumpCooldownTimer = 1f;
            //anim.SetTrigger("JumpUp");
            //anim.SetBool("Airborne", true);
        }
    }

    public void Step()
    {
        Vector3 stepPos = transform.position;
        
        AudioSource.PlayClipAtPoint(this.stepSounds[Random.Range(0, stepSounds.Length)], stepPos);
    }

}
