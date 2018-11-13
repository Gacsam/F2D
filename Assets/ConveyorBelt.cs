using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour
{
    public GameObject conveyorBelt;
    public Transform endPoint;
    public float speed = 2.0f;

     void OnTriggerStay(Collider other)
    {
        other.transform.position = Vector3.MoveTowards(other.transform.position, endPoint.position, speed * Time.deltaTime);
    }
}