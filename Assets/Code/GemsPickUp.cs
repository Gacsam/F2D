using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsPickUp : MonoBehaviour {

	public int GemsToAdd;
    
    void OnTriggerEnter(Collider other)
	{
		/* ???
		if (other.GetComponent<Collider> () == null)
		
			return;
		*/

        if (other.tag != "Player" && other.tag != "Hamster")
			return;
		
		GemsManager.AddGems (GemsToAdd);
        Destroy(Instantiate((GameObject) Resources.Load("Effects/CFX_Poof"), transform.position, transform.rotation), 1);
        ResourceLoader.SoundPlay("GemCollected", MenuController.Instance.transform);
		Destroy (gameObject);


}
  
}
