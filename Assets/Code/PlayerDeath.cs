using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {

	public delegate void PlayerEntered();
	public event PlayerEntered playerDied;

	void Start(){
		this.playerDied += GameObject.FindObjectOfType<menuController> ().PlayerDied;
	}
		

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Hamster") {
			if (playerDied != null)
				playerDied();
		}
	}
}
