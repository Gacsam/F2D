//using System.Collections;
//using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Custom editor, pretty much for designers to understand stuff in inspector
[CustomEditor(typeof(GatewayScript))]
[CanEditMultipleObjects]
public class PrettyGatewayScript : Editor {
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