using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeEntrance : MonoBehaviour {

    public bool isFront = true;
    private bool isPlayerInArea = false;
    private GameObject thePlayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerInArea = true;
            thePlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerInArea = false;
            thePlayer = other.gameObject;
        }
    }

    private void Update()
    {
        if (isPlayerInArea)
        {
            if (Input.GetAxisRaw("Use") != 0)
            {
                GetComponentInParent<Tube>().MoveAlongPath(isFront, ref thePlayer);
            }
        }
    }
}
