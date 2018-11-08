using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatewayScript : MonoBehaviour
{
    [SerializeField]
    private int exitFace, enterFace;

    public delegate void PlayerEntered(GatewayScript theScript);
    public event PlayerEntered playerEntered;
	private int theFace = 1;

    public bool isTunnel, isFinal;
	public delegate void PlayerFinish();
	public event PlayerFinish playerFinish;
    [SerializeField]
    private Transform tunnelExit;

    public int GetFace()
    {
        return theFace;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

//            int currentAngle = (int)Vector3.Angle(transform.TransformDirection(transform.up), Camera.main.transform.parent.up);


			Debug.Log (transform.rotation.eulerAngles);

			if (transform.rotation.eulerAngles.z != 0)
                return;

            // Only exit in tunnel
            if (!isFinal)
            {
                if (isTunnel)
                {
                    other.transform.position = tunnelExit.position;
                    theFace = exitFace;
                }
                else
                {
                    // Otherwise allow both ways
                    if (other.transform.position.x > this.transform.position.x)
                    {
                        theFace = exitFace;

                    }
                    else
                    {
                        theFace = enterFace;
                    }
                }
            }
            // Player event
            if (playerEntered != null)
            {
                playerEntered(this);
            }
        }
    }
}
