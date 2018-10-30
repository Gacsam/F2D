using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTeleport : MonoBehaviour {

	// Level
	[SerializeField]
	private Transform nextPosition = null;
	[SerializeField]
	private Vector3 whichDirection;
	[SerializeField]
	private int disappearSpeed = 1;

	private Coroutine currentCoroutine;

	[SerializeField]
	private bool isFinal = false;

	void Start(){
		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			int currentAngle = (int)Vector3.Angle (transform.TransformDirection (transform.up), Camera.main.transform.up);
			if (currentAngle == 0) {
				if (isFinal) {
					Destroy (other.gameObject);
					GameObject.FindObjectOfType<menuController> ().LevelFinished(true);
					return;
				}
				currentCoroutine = StartCoroutine(MovePlayerToNext ());
			}
		}
	}

	IEnumerator MovePlayerToNext(){
		GameObject.FindObjectOfType<RotateClick> ().disableRotation ();
		int i = 0;
		// Stop moving and disable movement
		GameObject thePlayer;
		thePlayer = GameObject.FindGameObjectWithTag ("Player");
		Rigidbody rb = thePlayer.GetComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.isKinematic = true;
		thePlayer.GetComponent<playerMovement> ().enabled = false;
		Vector3 rotation = thePlayer.transform.rotation.eulerAngles;
		rotation.y = 90;
		thePlayer.transform.rotation = Quaternion.Euler (rotation);
		while (true) {
			if (i < 30) {
				thePlayer.transform.position += (Camera.main.transform.forward * disappearSpeed * Time.deltaTime);
			} else {
				thePlayer.transform.position = nextPosition.position;
				thePlayer.transform.rotation = nextPosition.rotation;
				GameObject.FindObjectOfType<RotateClick> ().StartRotating (whichDirection);
				rb.isKinematic = false;
				thePlayer.GetComponent<playerMovement> ().enabled = true;
				StopCoroutine (currentCoroutine);
			}
			i++;
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
}