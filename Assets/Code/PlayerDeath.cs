using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {

	public delegate void PlayerEntered();
	public event PlayerEntered playerEntered;

	void Start(){
		this.playerEntered += GameObject.FindObjectOfType<menuController> ().PlayerDied;
	}
		

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			if (playerEntered != null)
				playerEntered();
		}
	}
}
