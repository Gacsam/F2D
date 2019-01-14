using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {

	public delegate void CharacterDied();
	public event CharacterDied charDied;
    private ParticleSystem charDeath;
    bool triggered;


    [SerializeField]
    float timeWait = 2;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Hamster") {
            other.GetComponent<Rigidbody>().isKinematic = true;
            GameObject poofParticle = Instantiate((GameObject) Resources.Load("Effects/CFX_MagicPoof"), other.transform.position + Vector3.back, other.transform.rotation);
            Destroy(other.gameObject);
            Instantiate((GameObject) Resources.Load("Effects/CFX_Virus"), poofParticle.transform.position, poofParticle.transform.rotation, poofParticle.transform);
            ResourceLoader.SoundPlay("DeathHappens", other.transform);
            // replace timeWait with above when sound is in
            StartCoroutine(PlayerDied(timeWait));
		}
	}

    IEnumerator PlayerDied(float waitTime){
        yield return new WaitForSeconds(waitTime);
        this.charDied += GameObject.FindObjectOfType<MenuController> ().PlayerDied;
        if (charDied != null) {
            charDied ();
            this.charDied -= GameObject.FindObjectOfType<MenuController> ().PlayerDied;
        }
        yield return null;
    }
}
