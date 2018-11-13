using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDoor : MonoBehaviour {

	[SerializeField]
	private GameObject doorForThisKey;

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			Destroy (doorForThisKey);
			Destroy (this.gameObject);
		}
	}
}
