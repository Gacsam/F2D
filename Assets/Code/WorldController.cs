using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	private static WorldController singleton;
	private WorldController worldScript;
	public enum GameState{Menu, Player};
	private GameState currentState;
	Coroutine spinCoroutine;

	private int currentLevel = 1;
	// left middle right levels
	private GameObject left;
	private GameObject current;
	private GameObject right;
	private int faceFacingCamera = 1;

	[SerializeField]
	private float rotationSpeed = 3;
    GameObject player;

	void Awake(){
		if (singleton == null) {
			singleton = this;
		} else {
			Destroy (gameObject);
		}
		worldScript = GetComponent<WorldController> ();
		DontDestroyOnLoad (gameObject);
		SetupLevels ();
	}

	void Update(){
        if(player != null)
        {
            Vector3 rotation = player.transform.rotation.eulerAngles;
            if (Mathf.Round(rotation.x) != 270)
            {
                rotation.x = 270;
                rotation.y = player.GetComponent<playerMovement>().lookingRight ? 90 : 270;
                player.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        if (spinCoroutine != null)
            return;
        if (currentState == GameState.Player)
        {
            if (Input.GetAxisRaw("RotateZ") != 0)
            {
                PlayerRotation((int)Mathf.Sign(Input.GetAxisRaw("RotateZ")));
            }
        }
    }

	public void UpdateState(GameState newState){
		currentState = newState;
	}

	/*
	 * Player and Level scripts
	 */

	void SetupLevels()
	{
		current = GetLevel(1, Vector3.back);
		right = GetLevel(2, Vector3.right);
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

	public void ModLevel(int mod, bool clickNextLevel)
	{
		GameObject missingLevel = Resources.Load<GameObject>("Prefabs/Levels/" + (currentLevel + mod));
		if ((currentLevel + mod <= 0) || !missingLevel)
			return;

		currentLevel += mod;
		bool isNextLevel = mod == 1 ? true : false;
		RefreshLevels(isNextLevel);
		ChangeToNextCube (isNextLevel);
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


	public int GetCurrentLevel(){
		return currentLevel;
	}
		
	public void ReloadCurrentlevel(bool createPlayer){
		Destroy (current);
		current = GetLevel (GetCurrentLevel(), Vector3.back);
//		Quaternion temp = current.transform.rotation;
//		temp.eulerAngles = current.GetComponent<TempFix_InitRotation> ().init;
//		current.transform.rotation = temp;
		if(createPlayer)
			StartGame ();
	}

	public void StartGame(){
		LoadPlayer ();
		foreach (GatewayScript door in current.GetComponentsInChildren<GatewayScript>()) {
			if (!door.isFinal)
				door.playerEntered += PlayerEntered;
		}
	}

    private void PlayerEntered(GatewayScript theScript)
    {
        faceFacingCamera = theScript.GetFace();
		RotateToFace();
    }

	private void LoadPlayer(){
		player = GameObject.FindWithTag ("Player");
		if(player)
			Destroy (player);
		Transform startingPosition = current.transform.Find ("StartingPos" + faceFacingCamera);
		GameObject playerPrefab = Resources.Load<GameObject> ("Prefabs/Characters/Player");
		player = Instantiate (playerPrefab, startingPosition.position, Quaternion.Euler(90,0,180), current.transform);
	}

	/*
	 * Rotation scripts
	 * Putting spinCoroutine = StartCoroutine()
	 * Down here allows me to easily use either void function without coroutine mess
	 */

	private void ChangeToNextCube(bool isNextLevel){
		Vector3 theDirection = isNextLevel ? Vector3.up : Vector3.down;
		if (spinCoroutine == null) {
			spinCoroutine = StartCoroutine (LevelSelection (theDirection));
		}
	}


	IEnumerator LevelSelection(Vector3 theDirection){
		float x = 0;
		while (true) {
			x += rotationSpeed;
			this.transform.Rotate (theDirection, rotationSpeed, Space.World);
			if (x >= 90) {
				if(spinCoroutine != null)
					spinCoroutine = null;
				yield break;
			}
			yield return new WaitForSeconds (Time.deltaTime);
		}
	}

	public void RotateToFace()
	{
		if (spinCoroutine == null) {
			spinCoroutine = StartCoroutine (RotateFacing ());
			PausePlayer ();
		}
	}

	public int GetCurrentFace(){
		return faceFacingCamera;
	}

	public void RotateToFace(int newFace)
	{
		if (spinCoroutine == null) {
			faceFacingCamera = newFace;
			spinCoroutine = StartCoroutine (RotateFacing ());
			PausePlayer ();
		}
	}

	IEnumerator RotateFacing(){
		Faces faceDir = new Faces ();
		Vector3 direction = faceDir.getFace (faceFacingCamera);

		Quaternion initialRotation = current.transform.rotation;
		Quaternion targetRotation = Quaternion.Euler (direction);
		while (true) {
			current.transform.rotation = Quaternion.RotateTowards (current.transform.rotation, targetRotation, rotationSpeed);
			if (current.transform.rotation == targetRotation) {
				current.transform.rotation = targetRotation;
				if(spinCoroutine != null)
					spinCoroutine = null;
				yield break;
			}
			yield return new WaitForSeconds (Time.deltaTime);
		}
	}

	public void PlayerRotation(int i){
		if (spinCoroutine == null) {
			Vector3 direction = Vector3.zero;
			if (i == 1) {
				direction = Vector3.forward * 90;
			} else {
				direction = Vector3.forward * -90;
			}
			spinCoroutine = StartCoroutine (PlayerRotate (direction));
		}
	}

	IEnumerator PlayerRotate(Vector3 theDirection){
		float x = 0;
		while (true) {
			x += rotationSpeed;
			current.transform.Rotate (theDirection, rotationSpeed, Space.World);
			if (x >= 90) {
				if(spinCoroutine != null)
					spinCoroutine = null;
				yield break;
			}
			yield return new WaitForSeconds (Time.deltaTime);
		}
	}

	void PausePlayer(){
		StartCoroutine (TemporaryPlayerPause ());
	}

	IEnumerator TemporaryPlayerPause(){
		playerMovement playerScript = GameObject.FindObjectOfType<playerMovement> ();
		if (playerScript != null) {
			// stop player script from interfering or allow player to move
			playerScript.enabled = false;
			// isKinematic makes the player animation dependant not physics (stops movement)
			playerScript.GetComponent<Rigidbody> ().isKinematic = true;
			yield return new WaitUntil (() => spinCoroutine == null);
			playerScript.enabled = true;
			playerScript.GetComponent<Rigidbody> ().isKinematic = false;
		}
		yield return null;
	}

	public bool isSpinning(){
		return spinCoroutine != null;
	}
}