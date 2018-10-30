using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateClick : MonoBehaviour {

    private Transform cube;
    [SerializeField]
	[Range(5, 15)]
    private int rotateTime = 12;
    private bool isRotating = false;
    private Coroutine thisSpinning;
    [SerializeField]
    bool canRotate = false;

	[SerializeField]
	[Range(0.1f, 1)]
	private float timeBetweenSpins = 0.5f;

    // Use this for initialization
    void Start() {
        cube = this.transform;
    }

    // Update is called once per frame
    void Update() {
        /*
         * Single rotation at a time
         */
        if (!isRotating)
        {
            Vector3 rotationDirection = Vector3.zero;
			if (canRotate) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					thisSpinning = StartCoroutine (RotateAround (Vector3.up));
				} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					thisSpinning = StartCoroutine (RotateAround (Vector3.down));
				} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
					thisSpinning = StartCoroutine (RotateAround (Vector3.right));
				} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
					thisSpinning = StartCoroutine (RotateAround (Vector3.left));
				}
			}
            if (Input.GetAxisRaw("RotateZ") != 0)
            {
                rotationDirection.Set(0, 0, Mathf.Sign(Input.GetAxisRaw("RotateZ")));
				disableRotation ();
                thisSpinning = StartCoroutine(RotateAround(rotationDirection));
            }
        }
    }

	IEnumerator RotateAround(Vector3 theDirection)
    {
		float rotationSpeed = (float) 90  / rotateTime;

		float x = 0;
        while (true)
        {
            x += rotationSpeed;
			cube.Rotate (theDirection, rotationSpeed, Space.World);

			/*
			if (theDirection.z != null) {
				GameObject player = GameObject.FindWithTag ("Player");
				Vector3 centre = new Vector3(0,0,-12);
				player.transform.RotateAround (centre, theDirection * -1, rotationSpeed);
			}
			*/

			if (x >= 90) {
				StartCoroutine(enableRotation ());
				StopCoroutine (thisSpinning);
			}
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

	// Three functions for public use, disable rotation, and start rotating (triggers SceneRotation) separate to put animation in between

	public void disableRotation(){
		isRotating = true;
	}

	IEnumerator enableRotation(){
		yield return new WaitForSeconds(timeBetweenSpins);
		isRotating = false;
	}

	public void StartRotating(Vector3 theDirection){
		disableRotation ();
		thisSpinning = StartCoroutine(RotateAround(theDirection));
	}
}
