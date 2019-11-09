using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
	public bool lockCursor;
	public Transform target;
	public float distanceToTarget = 2f; 
	public float mouseSensitivity = 3.5f;
	public Vector2 pitchMinMax = new Vector2(-40, 85);
	private float pitch;
	private float yaw;
	public float rotationSmoothTime = .12f;
	private Vector3 currentRotation;
	private Vector3 rotationSmoothVelocity;

	// Use this for initialization
	void Start () {
		if (lockCursor){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
		
		Vector3 targetRotation = new Vector3(pitch, yaw);
		currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		transform.position = target.position - transform.forward * distanceToTarget;
	}
}
