using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faces {
	public Vector3 getFace(int i){
		switch (i) {
		case 1:
			return Vector3.zero;
		case 2:
			return Vector3.up * 90;
		case 3:
			return Vector3.up * 180;
		case 4:
			return new Vector3 (90, 180, 0);
		case 5:
			return new Vector3 (90, 180, -90);
		case 6:
			return new Vector3 (90, 180, -90);
		default:
			return Vector3.zero;
		}

	}
}
