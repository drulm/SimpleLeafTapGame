//----------------------------------------------------------------------
// csMain_Scene.cs
//		Handles the main (and only) menu for the game.

using UnityEngine;
using System.Collections;

public class csMain_Scene : MonoBehaviour 
{	
	private bool  DEBUG_LOCAL			= false;		// Starting Distance	

	private string LEAF_NAME			= "leaf";		// leaf base name
	private int MAX_CUBES				= 50;			// Max size
	private int MAX_LEAF_TYPES 			= 5;			// Max # materials
	private float START_Z				= 100.0f;		// Starting Distance
	private float rayDistance 			= 200.0f;		// The click-ray distance
	
//	private float FORCE_LEAF			= 6;
//	private float FORCE_HALF			= 3;
	
	private float SIZE_X				= 10;
	private float SIZE_Y				= 6;
	
	private string hitType				= "";
	
	private AudioSource[] myAudio;
	
	private float timeDown = 0.0f;
	
	public GameObject[] cubes;				// All cubes
	public GameObject cube;					// A cube Prefab

	public GameObject anim1;				// Touch animation
	public GameObject points;				// Touch animation
	public int maxCubes;					// Current max cubes
	public int scoreAdd;
	public int bonusAdd;
	
	public Material[] leafMaterials;				// Different leafs
	
	
	//----------------------------------------------------------------------
	// Unity Start Fn
	void Start()
	{	
		maxCubes = 3;											// Start slow
		csMenu_Scene.currentCubes = 0;
		scoreAdd = 1;
		bonusAdd = 0;
		cubes = new GameObject[MAX_CUBES];
		SetupAudio();
	}

	//----------------------------------------------------------------------
	// Load in audio sources.
	void SetupAudio()
	{
		int i;
		AudioSource[] aSources = GetComponents<AudioSource>();
		myAudio = new AudioSource[15];
		for (i=0; i < aSources.Length; i++) 
		{
			myAudio[i] = aSources[i];
		}
	}
	
	
	//----------------------------------------------------------------------
	// Unity Update Fn
	void Update()
	{
		if (! Application.isLoadingLevel) 
		{
			CreateCube(csMenu_Scene.currentCubes);
			CheckMouse();
			CheckGameOver();
		}
	}
	
	//----------------------------------------------------------------------
	// Game over here?
	void CheckGameOver()
	{
		if (csMenu_Scene.gameOver) 
		{
			timeDown += Time.deltaTime;
			if (timeDown > 3.0f) 
			{
				csMenu_Scene.returnMenu = true;
			}
		}
		if (csMenu_Scene.returnMenu) 
		{
			Application.LoadLevel("menu_scene");
		}
	}
	
	//----------------------------------------------------------------------
	// Check mouse click and perform operations when object hit.
	void CheckMouse()
	{
		string hitGameObjectName;
		string hitName;
		GameObject hitObject;
		RaycastHit hit = new RaycastHit();
		float x = Random.Range(0.0f, SIZE_X) - SIZE_X / 2;
		float y = Random.Range(0.0f, SIZE_Y) - SIZE_Y / 2;
		Ray raycst;
		bool  hitScored;
		Vector3 pos;
		Vector3 v3;
		
		hitScored = false;

		// Check the left mouse button, or the touch screen.
		if (Input.GetMouseButtonDown(0)) {
			// Get a raycast.
			raycst = Camera.main.ScreenPointToRay(Input.mousePosition);	

			if (!csMenu_Scene.gameOver && Physics.Raycast(raycst, out hit, rayDistance)) 
			{			
				// Get hit Object
				hitObject = hit.collider.gameObject;

				// Get name of object
				hitGameObjectName = hitObject.name;
				
				// Save position
				pos = v3 = hitObject.transform.position;

				// Is this object a leaf?
				if (hitGameObjectName.Contains(LEAF_NAME)) 
				{
					// Reset location, and add another cube.
					WriteVector3(out v3, x, y, START_Z);
					// Reset the object to the starting distance.
					hitObject.transform.position = v3;
					
					// Go set a new random leaf, prevents cheating!
					RandomLeaf(hit.collider.gameObject);

					// Add in the bonus.
					csMenu_Scene.score += scoreAdd + bonusAdd;
					hitScored = true;
					
					hitName = hitGameObjectName;
					hitName = hitName.Replace(LEAF_NAME, "");
					
					if (hitName.Trim() == hitType.Trim()) 
					{
						IncreaseBonus();
					}
					else 
					{
						bonusAdd = 0;
					}
					hitType = hitName;
					
					// Plays a wind-chime 1 to 6, create the tounch anim.
					myAudio[int.Parse(hitType) + 1].Play();
					CreateAnim(pos);
					
					// Chance of new object is less and less.
					if (Random.Range(1,maxCubes * 2) == 1) 
					{
						maxCubes++;
					}
					if (DEBUG_LOCAL) {
						Debug.Log("Hit");
					}
				}
				// It's not a leaf, only for a log.
				else if (DEBUG_LOCAL) 
					{
						Debug.Log("Miss");
					}

				// Incease the score bonus +1 if not missed a leaf yet.
				if (hitScored) 
				{
					scoreAdd++;
				}
			}

			// If the game isn't over yet.
			if (!csMenu_Scene.gameOver) 
			{
				if (!hitScored) 
				{
					// Missed, reduce score bonuses.
					myAudio[11].Play();
					scoreAdd = scoreAdd / 2;
					scoreAdd = scoreAdd == 0 ? 1 : scoreAdd;
					bonusAdd = 0;
				}
			}

			if (DEBUG_LOCAL) {
				Debug.Log("score = " + csMenu_Scene.score);
			}
		} // if (Input.GetMouseButtonDown(0)) {		
	}

	
	//----------------------------------------------------------------------
	// Create the unity 3D object, a leaf.
	void CreateCube (int cc)
	{
		float x = Random.Range(0.0f, SIZE_X) - SIZE_X / 2;
		float y = Random.Range(0.0f, SIZE_Y) - SIZE_Y / 2;
		int r =	Random.Range(0, MAX_LEAF_TYPES);
		Vector3 v;

		WriteVector3(out v, x, y, START_Z);

		// If maxLeafs (cubes) not created, make another.
		if (csMenu_Scene.currentCubes < MAX_CUBES && csMenu_Scene.currentCubes < maxCubes) {
			// Create the game object
			cubes[cc] = (GameObject)Instantiate(cube, v, Quaternion.identity);

			// Update the game object name
			cubes[cc].name = (LEAF_NAME + r.ToString()).Trim();

			// Change the object material.
			cubes[cc].GetComponent<Renderer>().material = leafMaterials[r];

			// One more leaf.
			csMenu_Scene.currentCubes++;
		}
	}

	
	//----------------------------------------------------------------------
	// Create animation effect
	void CreateAnim (Vector3 pos)
	{
		Color c;
		GameObject scoreText;
		float crl = Mathf.Min(bonusAdd, 4) / 4.0f;
		Vector3 v3;
		
		Instantiate(anim1, pos, Quaternion.identity);

		WriteVector3(out v3, pos.x, pos.y, pos.z);

		scoreText = (GameObject)Instantiate(points, v3, Quaternion.identity);
	
		//c = Color(1, 0, crl * 0.8f, 1);
		c.r = 1.0f;
		c.g = 0.0f;
		c.b = crl * 0.8f;
		c.a = 1.0f;
		scoreText.GetComponent<TextMesh>().color = c;

		if (bonusAdd > 0) 
		{
			scoreText.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
		}

		scoreText.GetComponent<TextMesh>().text = "+" + (scoreAdd + bonusAdd).ToString();
//		scoreText.GetComponent<TextMesh>().fontSize = 24 + crl * 20;
	}
	
	//----------------------------------------------------------------------
	// Increase the bonus
	void IncreaseBonus()
	{
		bonusAdd = bonusAdd >= 1000 ? bonusAdd + 1000 : bonusAdd;
		bonusAdd = bonusAdd == 100 ? 1000 : bonusAdd;
		bonusAdd = bonusAdd == 10 ? 100 : bonusAdd;
		bonusAdd = bonusAdd == 0 ? 10 : bonusAdd;
		/*
		if (bonusAdd >= 2000) {
			myAudio[7].Play();
		}
		else if (bonusAdd == 1000) {
			myAudio[8].Play();
		}
		else if (bonusAdd == 100) {
			myAudio[10].Play();
		}
		else if (bonusAdd == 10) {
			myAudio[9].Play();
		}
		*/
	}
	
	//----------------------------------------------------------------------
	// Make a random leaf.
	void  RandomLeaf(GameObject leaf)
	{
		int r = Random.Range(0, MAX_LEAF_TYPES);
		leaf.name = (LEAF_NAME + r.ToString()).Trim();
		leaf.GetComponent<Renderer>().material = leafMaterials[r];
	}

	//----------------------------------------------------------------------
	// Public Functions
	//----------------------------------------------------------------------
	
	//----------------------------------------------------------------------
	// Load a vector w/o using new to avoid garbage colletion
	public static void WriteVector3(out Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
	}

} // End of class
