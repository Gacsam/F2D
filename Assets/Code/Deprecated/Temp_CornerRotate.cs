using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_CornerRotate : MonoBehaviour {
	[SerializeField]
	private Vector3 rotationDirection;

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			GameObject player = other.gameObject;
			if (player.transform.position.x < this.transform.position.x) {
				return;
			}
		}
	}
}