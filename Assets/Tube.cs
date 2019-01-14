using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour {
    [SerializeField]
    Coroutine playerTravelling;
    GameObject thePlayer;
    [Range(0.01f, 1)]
    public float delay = 0.03f;

    public void MoveAlongPath(bool forward, ref GameObject playerObject)
    {
        if(playerTravelling == null)
        {
            thePlayer = playerObject;
            playerTravelling = StartCoroutine(MoveAlongSpline(forward));
        }
    }
    private IEnumerator MoveAlongSpline(bool forward)
    {
        if (playerTravelling != null)
            yield break;

        Transform[] splinePoints = transform.Find("Splines").GetComponentsInChildren<Transform>();

        while (true)
        {
            if (forward)
            {
                for (int i = 1; i < splinePoints.Length; i++)
                {
                    thePlayer.transform.position = splinePoints[i].position;
                    yield return new WaitForSeconds(delay);
                }
            }
            else
            {
                for (int i = splinePoints.Length-1; i > 0; i--)
                {
                    thePlayer.transform.position = splinePoints[i].position;
                    yield return new WaitForSeconds(delay);
                }
            }
            Debug.Log("FINISH");
            yield return new WaitForSeconds(1);
            playerTravelling = null;
            yield break;
        }
    }
}
