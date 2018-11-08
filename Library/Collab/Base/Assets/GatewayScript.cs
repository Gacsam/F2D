using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatewayScript : MonoBehaviour
{
    [SerializeField]
    private int exitFace, enterFace;

    public delegate void PlayerEntered(GatewayScript theScript);
    public event PlayerEntered playerEntered;
    private int nextFace = 1;

    [SerializeField]
    private bool isTunnel;
    [SerializeField]
    private Transform tunnelExit;

    public int GetFace()
    {
        return nextFace;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            int currentAngle = (int)Vector3.Angle(transform.TransformDirection(transform.up), Camera.main.transform.parent.up);

            if (currentAngle != 0)
                return;

            // Only exit in tunnel
            if (isTunnel)
            {
                other.transform.position = tunnelExit.position;
                nextFace = exitFace;
            }
            else
            {
                // Otherwise allow both ways
                if (other.transform.position.x > this.transform.position.x)
                {
                    nextFace = exitFace;

                }
                else
                {
                    nextFace = enterFace;
                }
            }

            if (playerEntered != null)
            {
                playerEntered(this);
            }
        }
    }
}
