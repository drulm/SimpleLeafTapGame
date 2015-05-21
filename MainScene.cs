//----------------------------------------------------------------------
// MainScene.cs
//		Handles the main (and only) menu for the game.

using UnityEngine;
using System.Collections;


//----------------------------------------------------------------------
// MainScene class for Unity3d MonoBehaviour
public class MainScene : MonoBehaviour 
{	
	// Constants
	private const bool DEBUG_LOCAL = false;			// Debug on or off

	private const string LeafName = "leaf";			// leaf base name
	private const int MaxLeaves = 50;				// Max size
	private const int MaxLeafTypes = 5;				// Max # materials
	private const float StartZ = 100.0f;			// Starting Distance
	private const float RayDistance = 200.0f;		// The click-ray distance
	private const float SizeX = 10;					// Starting location bounds of leaves X
	private const float SizeY = 6;					// Starting location bounds of leaves Y
	private const int MaxAudioSources = 15;			// Max number of AudioSource for sFX
	private const float LoseDelay = 3.0f;			// Delay after lost game

	// Public for the Unity interface
	public GameObject leaf;							// A leaf Quad prefab
	public GameObject anim1;						// Touched leaf particle animation
	public GameObject points;						// The Points 3dText Object
	public Material[] leafMaterials;				// Different leaves

	// Private vars
	private string lastHitType = "";					// Strores what user hit
	private float timeDown = 0.0f;					// Counter for time before end of game
	private AudioSource[] myAudio;					// To load in AudioSources
	private int maxLeaves;							// Current max leaves
	private int scoreAdd;							// Consecutive any leaf bonus
	private int bonusAdd;							// Bonus points for consecutive same leaves
	private GameObject[] leaves;					// All leaves


	//----------------------------------------------------------------------
	//----------------------------------------------------------------------
	// Unity Start Fn
	void Start()
	{	
		maxLeaves = 3;								// Start w/ only 3 leaves
		MenuScene.currentLeaves = 0;
		scoreAdd = 1;
		bonusAdd = 0;
		leaves = new GameObject[MaxLeaves];
		SetupAudio();
	}

	//----------------------------------------------------------------------
	// Load in audio sources.
	void SetupAudio()
	{
		int i; // Just a for loop var

		// Pull in all AudioSources from Unity, specific for this scene
		AudioSource[] aSources = GetComponents<AudioSource>();

		// Copy audio sources into our own defined array
		myAudio = new AudioSource[MaxAudioSources];
		for (i=0; i < aSources.Length; i++) 
		{
			myAudio[i] = aSources[i];
		}
	}
	

	//----------------------------------------------------------------------
	//----------------------------------------------------------------------
	// Unity Update code
	void Update()
	{
		// If we are not loading the level
		if (! Application.isLoadingLevel)
		{
			// Try to create a leaf, if one is needed.
			CreateLeaf(MenuScene.currentLeaves);

			// Check for mouse clicks
			CheckMouse();

			// Check if the game is over, player let a leaf go by
			CheckGameOver();
		}
	}


	//----------------------------------------------------------------------
	// Is the game over?
	void CheckGameOver()
	{
		// If the game is over because a leaf went past the player
		if (MenuScene.gameOver) 
		{
			// Start a countdown for 3 seconds
			timeDown += Time.deltaTime;
			if (timeDown > LoseDelay)
			{
				// A time has elapsed, get ready to return to the menu
				MenuScene.returnMenu = true;
			}
		}
	
		// If we should return to the menu, load up the menu scene level
		//		Cases are 1) game over, or 2) Android back key [esc]
		if (MenuScene.returnMenu || Input.GetKeyDown(KeyCode.Escape)) 
		{
			Application.LoadLevel("menu_scene");
		}
	}


	//----------------------------------------------------------------------
	// Check mouse click and perform operations when object hit.
	void CheckMouse()
	{
		float x;								// (x,y) stores random new leaf position
		float y;
		Ray raycst;								// Result of Camera.main.ScreenPointToRay(Input.mousePosition)
		Vector3 v3;								// Temp vector w/ new updated position of leaf hit
		Vector3 pos;							// Position of leaf hit	
		string hitName;							// Name of object hit, used for id
		bool  hitScored;						// Was a hit scored?
		GameObject hitObject;					// Object type hit
		string hitGameObjectName;				// Full object hit name
		RaycastHit hit = new RaycastHit();		// Create a raycast

		RandomLeafPosition(out x, out y);
		
		hitScored = false;

		// Check the left mouse button, or the touch screen.
		if (Input.GetMouseButtonDown(0)) {

			// Get a raycast.
			raycst = Camera.main.ScreenPointToRay(Input.mousePosition);	

			if (!MenuScene.gameOver && Physics.Raycast(raycst, out hit, RayDistance)) 
			{			
				// Get hit Object
				hitObject = hit.collider.gameObject;

				// Get name of object
				hitGameObjectName = hitObject.name;
				
				// Save position
				pos = v3 = hitObject.transform.position;

				// Is this object a leaf?
				if (hitGameObjectName.Contains(LeafName)) 
				{
					// Reset location, and add another leaf
					WriteVector3(out v3, x, y, StartZ);

					// Reset the object to the starting distance
					hitObject.transform.position = v3;
					
					// Go set a new random leaf, prevents cheating
					RandomLeaf(hit.collider.gameObject);

					// Add in the bonus
					MenuScene.score += scoreAdd + bonusAdd;
					hitScored = true;

					// Get the object name
					hitName = hitGameObjectName;
					// Remove "leaf" from the object name, only leaf id number
					hitName = hitName.Replace(LeafName, "");

					// If the same type of leaf was hit as previous, increase bonus
					if (hitName.Trim() == lastHitType.Trim()) 
					{
						IncreaseBonus();
					}
					else
					{
						// Else reset the leaf type bonus
						bonusAdd = 0;
					}

					// Save the last hit
					lastHitType = hitName;
					
					// Plays a wind-chime 1 to 6, create the animation
					PlayAudio(int.Parse(hitName) + 1);
					CreateAnim(pos);
					
					// Chance of new leaf creation is less and less
					if (Random.Range(1,maxLeaves * 2) == 1) 
					{
						maxLeaves++;
					}
					if (DEBUG_LOCAL) {
						Debug.Log("Hit");
					}
				}
				// It's not a leaf, only for a log
				else if (DEBUG_LOCAL) 
					{
						Debug.Log("Miss");
					}

				// Incease the score bonus +1 if not missed a leaf yet
				if (hitScored) 
				{
					scoreAdd++;
				}
			}

			// If the game isn't over yet
			if (!MenuScene.gameOver) 
			{
				if (!hitScored) 
				{
					// Missed, reduce score bonuses
					PlayAudio(11);
					scoreAdd = scoreAdd / 2;
					scoreAdd = scoreAdd == 0 ? 1 : scoreAdd;
					bonusAdd = 0;
				}
			}

			if (DEBUG_LOCAL) {
				Debug.Log("score = " + MenuScene.score);
			}
		} // if (Input.GetMouseButtonDown(0)) {		
	}

	
	//----------------------------------------------------------------------
	// Create the unity 3D object, a leaf
	void CreateLeaf(int cc)
	{
		float x;											// x,y random start location of leaf
		float y;
		Vector3 v;											// Just a vector temp var
		int r =	Random.Range(0, MaxLeafTypes);				// To set a random leaf

		// Get random starting position of a leaf
		RandomLeafPosition(out x, out y);

		// Setup our vector for v to make the object
		WriteVector3(out v, x, y, StartZ);

		// If maxLeafs not created, make another
		if (MenuScene.currentLeaves < MaxLeaves && MenuScene.currentLeaves < maxLeaves) {
			// Create the game object
			leaves[cc] = (GameObject)Instantiate(leaf, v, Quaternion.identity);

			// Update the game object name so we can determine what the object is when hit
			leaves[cc].name = (LeafName + r.ToString()).Trim();

			// Change the object material
			leaves[cc].GetComponent<Renderer>().material = leafMaterials[r];

			// Add to the current number of leaves
			MenuScene.currentLeaves++;
		}
	}

	
	//----------------------------------------------------------------------
	// Create points numerical animation effect at Vector3 pos
	void CreateAnim (Vector3 pos)
	{
		Color c;										// Stores the color of the points 3d text
		GameObject scoreText;							// For the points text
		float crl = Mathf.Min(bonusAdd, 4) / 4.0f;		// To change color based on the bonus

		// Create a halo effect where the leaf was located
		Instantiate(anim1, pos, Quaternion.identity);

		// Create 3dtext for the score bonus
		scoreText = (GameObject)Instantiate(points, pos, Quaternion.identity);
	
		// Set the color of the points text based on the bonus
		c.r = 1.0f;
		c.g = 0.0f;
		c.b = crl * 0.8f;
		c.a = 1.0f;
		scoreText.GetComponent<TextMesh>().color = c;

		// Make the text bold if there was a special bonus
		if (bonusAdd > 0) 
		{
			scoreText.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
		}

		// Change the text of the TextMesh
		scoreText.GetComponent<TextMesh>().text = "+" + (scoreAdd + bonusAdd).ToString();

		// Increase size of font for larger bonuses
		scoreText.GetComponent<TextMesh>().fontSize = 24 + (int)crl * 10;
	}


	//----------------------------------------------------------------------
	// Increase the bonus stepwise
	void IncreaseBonus()
	{
		bonusAdd = bonusAdd >= 1000 ? bonusAdd + 1000 : bonusAdd;
		bonusAdd = bonusAdd == 100 ? 1000 : bonusAdd;
		bonusAdd = bonusAdd == 10 ? 100 : bonusAdd;
		bonusAdd = bonusAdd == 0 ? 10 : bonusAdd;
	}


	//----------------------------------------------------------------------
	// Randomize a leaf GameObject's material and game-type
	void  RandomLeaf(GameObject leaf)
	{
		int r = Random.Range(0, MaxLeafTypes);
		leaf.name = (LeafName + r.ToString()).Trim();
		leaf.GetComponent<Renderer>().material = leafMaterials[r];
	}


	//----------------------------------------------------------------------
	// Play a sound by predefined index
	void PlayAudio(int aud)
	{
		myAudio[aud].Play();
	}


	//----------------------------------------------------------------------
	// Set coordinate for a new random leaf starting position
	void RandomLeafPosition(out float x, out float y)
	{
		// x,y random start location of leaf
		x = Random.Range (0.0f, SizeX) - SizeX / 2;
		y = Random.Range (0.0f, SizeY) - SizeY / 2;
	}


	//----------------------------------------------------------------------
	// Public Functions
	//----------------------------------------------------------------------
	
	//----------------------------------------------------------------------
	// Load a vector w/o using 'new' to avoid garbage colletion on Vector3
	public static void WriteVector3(out Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
	}

} // End of class
