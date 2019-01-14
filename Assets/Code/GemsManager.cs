using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemsManager : MonoBehaviour
{
	public static int Gems;
	Text text;
	
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Gems < 0)
			Gems = 0;
        text.text = Gems.ToString();
	}
	public static void AddGems (int GemsToAdd)
	{
		Gems += GemsToAdd;
		if (Gems == null)
			Gems = 0;

	}

    public static void ResetGems(){
        if (Checkpoint.GetGems() != 0)
            Gems = Checkpoint.GetGems();
        else
            Gems = 0;
    }

}
