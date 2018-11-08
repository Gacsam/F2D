using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPush : MonoBehaviour {

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("MoveVertical") != 0)
        {
            rb.AddForce(Camera.main.transform.up * Input.GetAxis("MoveVertical") * 100);
        }
        if (Input.GetAxis("MoveHorizontal") != 0)
        {
            rb.AddForce(Camera.main.transform.right * Input.GetAxis("MoveHorizontal") * 100);
        }
        if (Input.GetAxis("MoveHorizontal") == 0 && Input.GetAxis("MoveVertical") == 0)
            ZeroIt();
    }

    void ZeroIt()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
