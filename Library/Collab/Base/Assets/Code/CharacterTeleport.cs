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

	static bool isPlayerPaused = true;

	private Coroutine currentCoroutine;

	[SerializeField]
	private bool isFinal = false;

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			int currentAngle = (int)Vector3.Angle (transform.TransformDirection (transform.up), Camera.main.transform.parent.up);
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

	IEnumerator ReturnPlayerControl(){
		yield return new WaitUntil (() => isPlayerPaused == false);
		GameObject.FindGameObjectWithTag ("Player").GetComponent<playerMovement> ().enabled = true;
	}

	public bool PlayerPaused(){
		return isPlayerPaused;
	}

	public void PlayerPaused(bool paused){
		isPlayerPaused = paused;
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
				thePlayer.transform.position += (Camera.main.transform.parent.forward * disappearSpeed * Time.deltaTime);
			} else {
				thePlayer.transform.position = nextPosition.position;
				thePlayer.GetComponent<Rigidbody> ().isKinematic = false;
				StartCoroutine (ReturnPlayerControl ());
				GameObject.FindObjectOfType<RotateClick> ().StartRotating (whichDirection);
				StopCoroutine (currentCoroutine);
			}
			i++;
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
}