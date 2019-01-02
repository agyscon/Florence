using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform playerTransform;
    private PlayerMovementControls playerMovementControls;

    // Vector from player to camera
    private Vector3 cameraVector;

    void Awake() {
        cameraVector = new Vector3(3f, 2f, -4f);
        playerMovementControls = playerTransform.gameObject.GetComponent<PlayerMovementControls>();
    }

    void Update() {
        Vector3 lookInput = new Vector3(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
        if (lookInput.magnitude > 1) {
            lookInput.Normalize();
        }
        lookInput *= 10;
        RotateCamera(lookInput);
        if (playerMovementControls != null) {
            playerMovementControls.PlayerMove(cameraVector);
        } else {
            Debug.LogError("No player movement control script found.");
        }
    }

    private void RotateCamera(Vector3 angle) {
        // Ortho projection of camera vector onto xz plane
        Vector3 xzRing = new Vector3(cameraVector.x, 0, cameraVector.z);
        float xAngle = Vector3.Angle(Vector3.forward, xzRing);
        if (cameraVector.x < 0) {
            xAngle *= -1;
        }
        float yAngle = Vector3.Angle(xzRing, cameraVector);
        if (cameraVector.y < 0) {
            yAngle *= -1;
        }

        float magnitude = cameraVector.magnitude;
        xAngle += angle.x;
        yAngle += angle.y;
        if (yAngle < 0f) {
            yAngle += 0f - yAngle;
        } else if (yAngle > 60f) {
            yAngle -= yAngle - 60f ;
        }
        xAngle *= Mathf.Deg2Rad;
        yAngle *= Mathf.Deg2Rad;
        cameraVector = new Vector3(xzRing.magnitude * Mathf.Sin(xAngle), cameraVector.magnitude * Mathf.Sin(yAngle), xzRing.magnitude * Mathf.Cos(xAngle));
        transform.position = playerTransform.position + cameraVector;
        transform.rotation = Quaternion.LookRotation(cameraVector * -1);
    }

}
