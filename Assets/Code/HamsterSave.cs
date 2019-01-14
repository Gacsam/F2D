using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterSave : MonoBehaviour
{
    public static HamsterSave Instance;

    void Awake(){
        if (Instance) {
            if (this == Instance) {
                return;
            } else
                Destroy (gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad (Instance);
        }
    }

    public void SaveTriggered(Transform parent, float disappearTime)
    {
        GameObject HamsterLoaded = (GameObject)Resources.Load("UI/HamsterSaved");
        HamsterLoaded = Instantiate(HamsterLoaded, parent);
        Destroy(HamsterLoaded, disappearTime);
    }
}
