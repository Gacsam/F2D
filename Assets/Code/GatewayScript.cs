using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GatewayType{Edge, Tunnel, Final};

public class GatewayScript : MonoBehaviour
{
    public int exitFace, enterFace;

    public delegate void PlayerEntered(GatewayScript theScript);
    public event PlayerEntered playerEntered;
	private int theFace = 1;
	public GatewayType thisGateway;

    public Transform tunnelExit;

    public int GetFace()
    {
        return theFace;
    }

	void Awake(){
		if (this.playerEntered == null) {
			if (this.thisGateway == GatewayType.Tunnel)
				this.playerEntered += GameObject.FindObjectOfType<WorldController> ().PlayerEnteredTunnel;
			else if (this.thisGateway == GatewayType.Edge)
				this.playerEntered += GameObject.FindObjectOfType<WorldController> ().PlayerEnteredEdge;
			else if (this.thisGateway == GatewayType.Final)
				this.playerEntered += GameObject.FindObjectOfType<MenuController> ().PlayerEntered;
		}
	}

    private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {

			int currentAngle = (int)Vector3.Angle (transform.up, Camera.main.transform.parent.up);

			if (currentAngle != 0)
				return;

			// Only exit in tunnel

			if (thisGateway == GatewayType.Tunnel) {
				other.transform.position = tunnelExit.position;
				theFace = exitFace;
			} else if (thisGateway == GatewayType.Edge) {
				Vector3 tempPos = transform.parent.position;
                tempPos.y = other.transform.position.y;
				other.transform.position = tempPos;
			}
			// Player event
			if (playerEntered != null) {
				playerEntered (this);
			}
		}
	}

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(this.transform.parent.position + this.transform.up * 0.25f, 0.1f);
        Gizmos.DrawWireCube(this.transform.parent.position, Vector3.one * 0.1f);
    }
}