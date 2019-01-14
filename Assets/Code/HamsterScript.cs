using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HamsterColor{Yellow, Blue, Red, Green};

public class HamsterScript : MonoBehaviour {

	public delegate void HamsterSaved(Transform self);
	public event HamsterSaved hamsterSaved;

	[SerializeField]
	private	HamsterColor ballColour;
	private	Rigidbody rb;
    
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		this.hamsterSaved += GameObject.FindObjectOfType<MenuController> ().HamsterSaved;
	}

	/*
    void AllowHamsterMovement()
    {
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
            GetComponent<Collider>().isTrigger = false;
        }
    }

	public string test;
	void Update(){
		if (Input.GetKey(test))
			AllowHamsterMovement ();
	}

    
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
            AllowHamsterMovement();
		}
	}
    */

	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "Hole") {
			GameObject other = col.collider.gameObject;
			if (other.GetComponent<HamsterHoleScript> ().GetColor () == ballColour) {
				StartCoroutine (Roll (other));
				if (hamsterSaved != null)
                    hamsterSaved (this.transform);
			}
		}
	}
	
    private float speed = 3;
	IEnumerator Roll(GameObject newParent){
		Vector3 initPos = transform.position;
		while (transform.position != newParent.transform.position) {
			rb.useGravity = false;
			rb.detectCollisions = false;
			rb.constraints = RigidbodyConstraints.None;
			rb.velocity = (newParent.transform.position - transform.position) * speed;
			yield return new WaitForSeconds (Time.deltaTime);
		}
		rb.isKinematic = true;
		yield return null;
	}
}