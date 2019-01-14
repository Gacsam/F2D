using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObject : MonoBehaviour {

    private Transform movingPart;
    private Collider movingCollider;


    private enum ObjectType{Hand, Pillar};
    [SerializeField]
    private ObjectType thisHiddenObject;
    [SerializeField]
    float minHidingTime = 1, maxHidingTime = 2, minShowingTime = 1, maxShowingTime = 2, moveSpeed = 1;

    private float offset;
    [SerializeField]
    [HideInInspector]
    private float defaultScaleY;

    // Use this for initialization
    void Start () {
        movingPart = this.transform.Find("MovingPart");
        movingCollider = this.transform.GetComponentInChildren<Collider>();
        if (movingCollider == null)
            Debug.Log(this.gameObject);
        offset = movingCollider.bounds.size.y * (movingPart.localScale.y/transform.localScale.y);
        if(defaultScaleY == 0)
            defaultScaleY = movingPart.transform.localScale.y;
        if (thisHiddenObject == ObjectType.Pillar)
        {
			movingPart.transform.localScale = new Vector3(movingPart.transform.localScale.x, 0.1f, movingPart.transform.localScale.z);
			movingPart.localPosition = movingPart.InverseTransformDirection (-transform.up) * 0.5f;
        }
        else
        {
			movingPart.localPosition = movingPart.InverseTransformDirection (-transform.up) * (movingCollider.bounds.size.y / 2);
        }

		BeginMoving (true);
	}

    Coroutine runningCoroutine;
    public void BeginMoving(bool toStart)
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = null;

        if (toStart)
            runningCoroutine = StartCoroutine(ShowHideObject());
    }

    IEnumerator ShowHideObject()
    {
        if (isMoving)
            yield break;

        while (true)
        {
            float delay = Random.Range(minHidingTime, maxHidingTime);
            yield return new WaitForSeconds(delay);
            StartCoroutine(ShowMove(true));
            yield return new WaitUntil(() => isMoving == false);
            // Wait until animation is done
            delay = Random.Range(minShowingTime, maxShowingTime);
            yield return new WaitForSeconds(delay);
            StartCoroutine(ShowMove(false));
            yield return new WaitUntil(() => isMoving == false);
        }
    }

    private bool isMoving = false;

    [SerializeField]
    [Range(0, 0.1f)]
    private float TestValueMinimumPillarSize;
    IEnumerator ShowMove(bool movingUp)
    {
        isMoving = true;

        if (movingUp)
        {
            if (thisHiddenObject == ObjectType.Pillar)
            {
                ResourceLoader.SoundPlay("PillarSound", transform, true);
            }
            else
            {
                ResourceLoader.SoundPlay("HandSound", transform, true);
            }
        }

        while (true)
        {
			Vector3 direction = movingUp ? movingPart.transform.up: -movingPart.transform.up;
			direction = movingPart.InverseTransformDirection (direction);

			movingPart.localPosition += direction * Time.deltaTime * moveSpeed;
            if (thisHiddenObject == ObjectType.Pillar)
			{
				movingPart.localScale += direction * Time.deltaTime * moveSpeed;
                if (movingPart.localScale.y > defaultScaleY || movingPart.localScale.y < TestValueMinimumPillarSize)
                {
                    if (movingPart.localScale.y < TestValueMinimumPillarSize)
                        movingPart.localScale = new Vector3(movingPart.localScale.x, TestValueMinimumPillarSize, movingPart.localScale.z);
                    else
                        movingPart.localScale = new Vector3(movingPart.localScale.x, defaultScaleY, movingPart.localScale.z);
                    
                    isMoving = false;
                    yield break;
                }
            }
            else {
                if (movingPart.localPosition.y > offset || movingPart.localPosition.y < -offset)
                {
                    movingPart.localPosition = direction * offset;
                    isMoving = false;
                    yield break;
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
