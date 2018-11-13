using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterHoleScript : MonoBehaviour {
	[SerializeField]
	private HamsterColor holeColour;

	public HamsterColor GetColor(){
		return holeColour;
	}
}
