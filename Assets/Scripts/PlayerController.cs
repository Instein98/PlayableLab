using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun {
	public float walkSpeed = 2;
	public float runSpeed = 6;
	public float turnSmoothTime = 0.2f;
	public float speedSmoothTime = 0.1f;
	public float gravity = -12;
	public float jumpHeight = 1;
	[Range(0,1)]
	public float airControlPercent = .3f;
	float turnSmoothVelocity;
	float velocityY;
	float currentSpeed;
	float speedSmoothVelocity;
	/* fixing the grouded issue: 
	The controller.isGrounded is not reliable all the time. 
	It may because it is used with controller.Move()
	grounded only update when the isGrounded is reliable*/
	bool grounded = true;
	Animator animator;
	Transform cameraT;
	CharacterController controller;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController>();
		if (photonView.IsMine){
			cameraT.GetComponent<ThirdPersonCamera>().target = transform.Find("Look Target");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.IsMine == false && PhotonNetwork.IsConnected == true){
			return;
		}
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDir = input.normalized;

		bool running = Input.GetKey(KeyCode.LeftShift);

		move(inputDir, running);

		grounded = controller.isGrounded;

		if (Input.GetKeyDown(KeyCode.Space)){
			jump();
		}
		float animationSpeedPercent = (running? currentSpeed/runSpeed : currentSpeed/walkSpeed * 0.5f);
		animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
	}


	void move(Vector2 inputDir, bool running){
		// Debug.Log(controller.isGrounded ? "GROUNDED" : "NOT GROUNDED");
		if (controller.isGrounded){
			velocityY = 0;
		}else{
			velocityY += Time.deltaTime * gravity;
		}

		if (inputDir != Vector2.zero){
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, getModifiedSmoothTime(turnSmoothTime));
		}
		float targetSpeed = (running ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, getModifiedSmoothTime(speedSmoothTime));
		Vector3 velocity = transform.forward * currentSpeed + transform.up * velocityY;
		controller.Move(velocity * Time.deltaTime);

		currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

	}

	void jump(){
		if (grounded){
			float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float getModifiedSmoothTime(float smoothTime){
		if (grounded){
			return smoothTime;
		}
		if (airControlPercent == 0){
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;

	}
}
