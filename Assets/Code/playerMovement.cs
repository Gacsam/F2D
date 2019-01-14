using UnityEngine;
using System.Collections;
 
public class playerMovement : MonoBehaviour
{

	[SerializeField]
    private float walkSpeed = 2f;
	[SerializeField]
    private float jumpHeight = 3f;
    private Rigidbody rb;
    private bool inAir = false;
	[SerializeField]
	[Range(0.5f, 1)]
	private float speedIncrease = 0.75f;
	public bool lookingRight = true;

    [SerializeField]
    [Range(0.75f, 1)]
    private float speedDecrease = 0.75f;

	private float colliderExtents;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
		colliderExtents = this.GetComponent<Collider> ().bounds.size.y;
    }

    void Update()
	{
        // Check for ground
        if (inAir || rb.velocity.y < -1) inAir = GroundCheck();

		if (PushedUp && PushedDown)
		{
			Debug.Log("askljdfhasoijdfhydsa");
		}
		PushedUp = false;
		PushedDown = false;
    }

	// FixedUpdate for movement and non-frame-based (same speed even if lagging)
    private void FixedUpdate()
	{		


		// If horizontal movement happens LEFT/RIGHT
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            // Move the player
            rb.velocity += Camera.main.transform.parent.right * Input.GetAxisRaw("Horizontal") * speedIncrease;

            // Check player facing direction
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                lookingRight = (Input.GetAxisRaw("Horizontal") > 0) == true ? true : false;
            }

            // if above max/walk speed negate the overflow
            if (rb.velocity.x > walkSpeed)
                rb.velocity = new Vector3(walkSpeed, rb.velocity.y, rb.velocity.z);
            else if (rb.velocity.x < -walkSpeed)
                rb.velocity = new Vector3(-walkSpeed, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            if (rb.velocity.x != 0)
            {
                Vector3 tempVel = rb.velocity;
                tempVel.x *= speedDecrease;
                rb.velocity = tempVel;

                tempVel = rb.angularVelocity;
                tempVel.x *= speedDecrease * Time.deltaTime;
                rb.angularVelocity = tempVel;
            }
        }

		// Jump
		if (!inAir && rb.useGravity) {
			if (Input.GetAxisRaw ("Jump") != 0) {
				inAir = true;
				rb.velocity = new Vector3 (rb.velocity.x, jumpHeight, rb.velocity.z);
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, jumpHeight, rb.angularVelocity.z);
			}
		}
	}

    // we're testing if we're in the air
    bool GroundCheck()
    {
        if (rb.velocity.y > 0)
            return true;

		Vector3 testPos = transform.position;
		testPos.y -= colliderExtents * 0.5f;

        if(Physics.CheckSphere(testPos, 0.1f, 1, QueryTriggerInteraction.Ignore)){
            Destroy(Instantiate((GameObject) Resources.Load("Effects/CFX3_Hit_SmokePuff"), transform.position, transform.rotation), 1);
			return false;
		}
		return true;   
	}

	bool PushedUp, PushedDown;
	void OnCollisionStay(Collision collide)
	{
		for (int i = 0; i < collide.contacts.Length; i++) {
			if (collide.contacts [i].point.y < transform.position.y + colliderExtents) {
				PushedUp = true;
			} else if (collide.contacts [i].point.y > transform.position.y + colliderExtents  * 0.8f) {
  				PushedDown = true;
			}
		}
	}
}