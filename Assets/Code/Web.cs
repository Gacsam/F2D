using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if (other.tag  == "Player")
        {
			Rigidbody rb = other.gameObject.GetComponent<Rigidbody> ();
			rb.useGravity = false;
			rb.velocity *= 0.75f;
        }        
    }

    private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") {
			other.gameObject.GetComponent<Rigidbody> ().useGravity = true;
		}
	}
}