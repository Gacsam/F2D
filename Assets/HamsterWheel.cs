using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterWheel : MonoBehaviour {

    public GameObject theObjectToResize;
    public float resizeSpeed = 0.1f, centeringSpeed = 0.01f, minimalOffset = 0.3f, maximumSize = 15;

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            float valueToCheck = (transform.position - collision.contacts[0].point).x;
            Vector3 resizeDirection = valueToCheck > 0 ? Vector3.right: Vector3.left;

            if (theObjectToResize.transform.localScale.x < maximumSize && theObjectToResize.transform.localScale.x > 1)
            {
                if (valueToCheck > minimalOffset)
                {
                    theObjectToResize.transform.localScale += resizeDirection * resizeSpeed * Time.deltaTime;
                }
                else if (valueToCheck < -minimalOffset)
                {
                    theObjectToResize.transform.localScale += resizeDirection * resizeSpeed * Time.deltaTime;
                }
            }
            Vector3 thisPos = transform.position;
            thisPos.y = collision.transform.position.y;
            collision.transform.position = Vector3.Lerp(collision.transform.position, thisPos, centeringSpeed);
        }
    }
}
