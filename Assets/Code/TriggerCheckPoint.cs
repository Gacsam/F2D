using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckPoint : MonoBehaviour {

	public bool triggered;
	private void Awake(){
		if(triggered)
			Destroy (this.gameObject);
	}

    private void OnTriggerEnter(Collider other)
    {
        float testValue = Vector3.Dot(transform.up, Camera.main.transform.parent.up);
        if (Mathf.Approximately(testValue, 1) || Mathf.Approximately(testValue, -1))
        {
            
            if (other.tag == "Player" && !triggered && !WorldController.isSpinning())
            {
                triggered = true;
                Checkpoint.SaveLevel(WorldController.GetWorld());
                Checkpoint.SaveHamsters(MenuController.GetHamsters());
                Checkpoint.SaveGems(GemsManager.Gems);
                ResourceLoader.LoadImageObjectForTime("UI/Checkpoint", MenuController.Instance.transform, 1);
                ResourceLoader.SoundPlay("Checkpoint", MenuController.Instance.transform);
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.Log(testValue);
        }
    }
}
