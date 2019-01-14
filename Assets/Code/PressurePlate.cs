using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {


    public enum pressuretype {soft, hard};
    [SerializeField]
    private pressuretype current;
    [SerializeField]
    private GameObject[] pressureObjects;

    private void OnTriggerEnter(Collider other)
    {
        ResourceLoader.SoundPlay("PadPressed", transform, true);

        if (pressureObjects.Length == 0 || other.tag != "Player")
            return;

        if (current == pressuretype.soft)
        {
            foreach (GameObject currentObject in pressureObjects)
            {
                currentObject.GetComponent<MeshRenderer>().enabled = false;
				currentObject.GetComponent<Collider> ().enabled = false;
            }
        }
        else
        {
            foreach (GameObject currentObject in pressureObjects)
            {
				Destroy(currentObject);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        ResourceLoader.SoundPlay("PadPressed", transform);

        if (current == pressuretype.soft)
        {
            foreach (GameObject currentObject in pressureObjects)
            {
				currentObject.GetComponent<MeshRenderer>().enabled = true;
				currentObject.GetComponent<Collider> ().enabled = true;

            }
        }
    }



}
