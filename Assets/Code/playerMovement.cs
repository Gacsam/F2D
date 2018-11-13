﻿

using UnityEngine;
using System.Collections;
 
public class playerMovement : MonoBehaviour
{

	[SerializeField]
    private float walkSpeed = 2f;
	[SerializeField]
    private float jumpHeight = 3f;
    private Rigidbody rb;
    private bool inAir = true;
	private bool canJump = true;
	[SerializeField]
	[Range(0.5f, 1)]
	private float speedIncrease = 0.75f;
	[SerializeField]
	[Range(0.25f, 0.75f)]
	private float speedDecrease = 0.25f;
	private Transform groundCheck;
	public bool lookingRight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
		groundCheck = transform.Find ("GroundCheck");
    }

    void Update()
	{
        // Check for ground 
        if ((rb.velocity.y != 0 || inAir) && Input.GetAxisRaw("Jump") == 0) inAir = GroundCheck();
    }

	// FixedUpdate for movement and non-frame-based (same speed even if lagging)
    private void FixedUpdate()
    {		
		// If horizontal movement happens LEFT/RIGHT
		if (Input.GetAxisRaw ("Horizontal") != 0) {
			// Move the player
			rb.velocity += Camera.main.transform.parent.right * Input.GetAxisRaw ("Horizontal") * speedIncrease;

			// Check player facing direction
			if ((Input.GetAxisRaw ("Horizontal") > 0 && transform.rotation.eulerAngles.y != 180) || (Input.GetAxisRaw ("Horizontal") < 0 && transform.rotation.eulerAngles.y != 0)) {
				Vector3 rotation = transform.rotation.eulerAngles;
				// Set rotation.y to (if z = 0) set it to 180 otherwise 0
				rotation.y = (Input.GetAxisRaw ("Horizontal") > 0) == true ? 180 : 0;
				lookingRight = (Input.GetAxisRaw ("Horizontal") > 0) == true ? true : false;
				transform.rotation = Quaternion.Euler (rotation);
			}

			// if above max/walk speed negate the overflow
			if (rb.velocity.x > walkSpeed)
				rb.velocity -= Camera.main.transform.parent.right * Input.GetAxisRaw ("Horizontal") * (rb.velocity.x - walkSpeed);
			else if (rb.velocity.x < -walkSpeed)
				rb.velocity += Camera.main.transform.parent.right * Input.GetAxisRaw ("Horizontal") * (rb.velocity.x + walkSpeed);
		} else {
			// Slowly decrease speed
			if(!inAir)
				rb.velocity *=  1 - speedDecrease;
		}

		// Jump
		if (canJump) {
			if (Input.GetAxisRaw ("Jump") != 0 && !inAir) {
				inAir = true;
				canJump = false;
				rb.AddForce (Vector3.up * jumpHeight, ForceMode.VelocityChange);
			}
		} else if (Input.GetAxisRaw ("Jump") == 0) {
			canJump = true;
		}
	}

    bool GroundCheck()
	{
		RaycastHit hit;
		if (Physics.Raycast (groundCheck.position, -Camera.main.transform.parent.up, out hit, 0.1f)) {
			return false;
		}
		return true;
	}
}


