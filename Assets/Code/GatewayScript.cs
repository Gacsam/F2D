using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

	void Update(){
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

			int currentAngle = (int)Vector3.Angle(transform.up, Camera.main.transform.parent.up);
//			int currentAngle = (int)Vector3.Angle(transform.TransformDirection(transform.up), transform.TransformDirection(other.transform.up));

			Debug.Log (currentAngle);

			if (currentAngle != 0)
                return;

            // Only exit in tunnel

                if (thisGateway == GatewayType.Tunnel)
                {
                    other.transform.position = tunnelExit.position;
                    theFace = exitFace;
                }
			else if(thisGateway == GatewayType.Edge)
                {
					Vector3 tempPos = other.transform.position;
					tempPos.x = transform.position.x;
					tempPos.z = transform.position.z;
					other.transform.position = tempPos;
                }
            // Player event
            if (playerEntered != null)
            {
                playerEntered(this);
            }
        }
    }
}


// Custom editor, pretty much for designers to understand stuff in inspector
[CustomEditor(typeof(GatewayScript))]
[CanEditMultipleObjects]
public class GatewayScriptEditor : Editor {
	override public void OnInspectorGUI(){
		GatewayScript myScript = target as GatewayScript;
		myScript.thisGateway = (GatewayType) EditorGUILayout.EnumPopup ("Type of gateway:", myScript.thisGateway);
		myScript = target as GatewayScript;

		if (myScript.thisGateway == GatewayType.Tunnel) {
			myScript.enterFace = (int)EditorGUILayout.IntSlider ("Which face is this on?", myScript.enterFace, 1, 6);
			myScript.exitFace = (int)EditorGUILayout.IntSlider ("Which face is the exit on?", myScript.exitFace, 1, 6);
			myScript.tunnelExit = (Transform)EditorGUILayout.ObjectField ("Where is the exit?", myScript.tunnelExit, typeof(Transform), true);
		}
	}
}