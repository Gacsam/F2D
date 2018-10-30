using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour {

    private int currentLevel = 1;
	// Use this for initialization
	void Start () {
        SetupLevels();
		this.enabled = false;
	}

    [SerializeField]
    [Range(0.1f, 1)]
    float pauseTime = 1;
    bool canRotate = true;
    // Update is called once per frame
    void Update() {
        if (canRotate)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                if (Input.GetAxis("Horizontal") < 0)
                    ModLevel(-1, false);
                else if (Input.GetAxis("Horizontal") > 0)
                    ModLevel(1, false);
                StartCoroutine(RotationPause(pauseTime));
            }
        }
    }

	IEnumerator RotationPause(float pause)
    {
        canRotate = false;
        yield return new WaitForSeconds(pause);
        canRotate = true;
    }

	public int GetCurrentLevel(){
		return currentLevel;
	}

	[SerializeField]
	private GameObject player;
	public void ReloadCurrentlevel(bool createPlayer){
		Destroy (current);
		current = GetLevel (GetCurrentLevel(), Vector3.back);
		if(createPlayer)
			StartGame ();
	}

	public void StartGame(){
		RotateClick checkExist = GameObject.FindObjectOfType<RotateClick> ();
		if(checkExist)
			Destroy(checkExist);
		current.AddComponent<RotateClick> ();
		LoadPlayer ();
	}

	private void LoadPlayer(){
		player = GameObject.FindWithTag ("Player");
		if(player)
			Destroy (player);
		Transform startingPosition = current.transform.Find ("StartingPos");
		GameObject playerPrefab = Resources.Load<GameObject> ("Prefabs/Characters/Player");
		player = Instantiate (playerPrefab, startingPosition.position, Quaternion.Euler(90,0,180), current.transform);
	}

    bool levelChanged = false;
	public void ModLevel(int mod, bool clickNextLevel)
    {
		GameObject missingLevel = Resources.Load<GameObject>("Prefabs/Levels/" + (currentLevel + mod));
        if ((currentLevel + mod <= 0) || !missingLevel || levelChanged)
            return;

        levelChanged = true;
        currentLevel += mod;
        Vector3 direction = currentLevel > int.Parse(current.name) ? Vector3.up : Vector3.down;
		bool isNextLevel = mod == 1 ? true : false;
		RefreshLevels(isNextLevel);
		spinCoroutine = StartCoroutine(RotateSlowly(direction, clickNextLevel));
    }

	// left middle right levels
    private GameObject left;
    private GameObject current;
    private GameObject right;
    void SetupLevels()
    {
        current = GetLevel(1, Vector3.back);
        right = GetLevel(2, Vector3.right);
    }

	void RefreshLevels(bool isNextLevel)
	{
		if (isNextLevel) {
			if (left)
				Destroy (left);
			left = current;
			current = right;
			right = GetLevel(currentLevel + 1, Vector3.right);
		} else {
			if (right)
				Destroy (right);
			right = current;
			current = left;
			left = GetLevel(currentLevel - 1, Vector3.left);
		}
	}

    [SerializeField]
    [Range(5, 25)]
    private int displacementValue = 5;
	GameObject GetLevel(int level, Vector3 displacement)
    {
        if (level != 0)
        {
			GameObject loadLevel = Resources.Load<GameObject>("Prefabs/Levels/" + level);
            if (!loadLevel)
                return null;

            GameObject createLevel = Instantiate(loadLevel, displacement * displacementValue, Quaternion.Euler(Vector3.zero), this.transform);
            if (!createLevel)
                return null;

            createLevel.name = loadLevel.name;

            return createLevel;
        }
        else
            return null;
    }


    [SerializeField]
    [Range(5, 25)]
    private float rotateTime = 10;
    Coroutine spinCoroutine;
	IEnumerator RotateSlowly(Vector3 theDirection, bool nextLevel)
    {
        float rotationSpeed = (float)90 / rotateTime;

        float x = 0;
        while (true)
        {
            x += rotationSpeed;
			if (x >= 90) {
				StopCoroutine (spinCoroutine);
				levelChanged = false;
				if (nextLevel) {
					StartGame ();
				}
			}else
				this.transform.Rotate(theDirection, rotationSpeed, Space.World);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
