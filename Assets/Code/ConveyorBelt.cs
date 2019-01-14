using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour
{

    public float speed = 1.0f;
    public bool movingleft = false;


     void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Hamster")
            return;

        int currentAngle = (int)Vector3.Angle(transform.up, Camera.main.transform.parent.up);
        if (currentAngle != 0)
            return;

        Vector3 direction = movingleft ? Vector3.left : Vector3.right;
        other.transform.position += direction * speed * Time.deltaTime;
    }
}