using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    static private GameObject levelState;

    // Use this for initialization

    static public void SaveLevel(GameObject level)
	{
        if (levelState != null)
            Destroy(levelState);

        levelState = (GameObject)Instantiate(level);

        levelState.SetActive(false);
    }

    static public GameObject GetLevel()
    {
        return levelState;
    }

	static public void Reset(){
		Destroy (levelState);
		levelState = null;
		hamsterSaved = 0;
        gems = 0;
	}

    static int hamsterSaved;

    static public void SaveHamsters (int hamsterSaved)
	{
		Checkpoint.hamsterSaved = hamsterSaved;
	}

	static public int GetHamsters()
    {
        return hamsterSaved;
    }

    static int gems;

    static public void SaveGems(int gems){
        Checkpoint.gems = gems;
    }

    static public int GetGems(){
        return Checkpoint.gems;
    }
}
