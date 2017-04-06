//----------------------------------------------------------------------
// csLeaf.cs
//		Handles the main (and only) menu for the game.
using UnityEngine;
using System.Collections;

public class csLeaf : MonoBehaviour 
{
	// csLeaf class private variables.
	private bool  DEBUG_LOCAL	= false;			// Starting Distance
	
	private float START_Z		= 100.0f;			// Starting Distance
	private float FORCE_LEAF	= 20;				// For force
	private float FORCE_HALF	= 10;				// For force
	
	private float BREAK_POINT	= 30.0f;			// Where to move leaf away
	
	private float OFF_SCREEN	= -35.0f;			// For forces
	
	private float SIZE_X		= 10;
	private float SIZE_Y		= 6;
	
	private float rotateSpeed 	= 200.0f;			// The rotate speed
	private float moveSpeed 	= 15;				// The move speed
	private float lastRotate	= 0;				// The rotate speed

	private Vector3 slow;							// Direction near end
	
	private float x;								// Make it fly out
	private float y;
	private float leafRand;

	
	//----------------------------------------------------------------------
	// Unity Start() function
	void  Start()
	{
		x = 	(Random.Range(0.0f, SIZE_X) - SIZE_X / 2.0f);
		y = 	(Random.Range(0.0f, SIZE_Y) - SIZE_Y / 2.0f);

		SetLeafRand();

		if (DEBUG_LOCAL) 
		{
			Debug.Log("LeafRand = " + leafRand);
		}
	}


	//----------------------------------------------------------------------
	// SetLeafRand()
	void  SetLeafRand()
	{
		leafRand = csMenu_Scene.currentCubes / 4.0f + 2.0f;
		leafRand = Mathf.Min(leafRand, 50.0f);
		leafRand = Mathf.Max(leafRand, 2.0f);

		csMain_Scene.WriteVector3(out slow, x * leafRand, y * leafRand, OFF_SCREEN);
	}


	//----------------------------------------------------------------------
	// Unity Update() function
	void  Update()
	{
		Vector3 target;
		Vector3 center;
		Vector3 leftScreen;
		float rotateStep;
		float moveStep;
		float rot 			= 	(Random.Range(0.0f, FORCE_LEAF) - FORCE_HALF) / 10.0f;
		
		if (lastRotate > 0 && rot < 0 || lastRotate < 0 && rot > 0) 
		{
			rot = -rot;
		}
		
		lastRotate = rot;

		// Calculate the rotate and move step values.
		rotateStep = rotateSpeed * Time.deltaTime * rot;	
		moveStep = moveSpeed * Time.deltaTime;
		
		transform.Translate(Vector3.forward * - moveStep);

		csMain_Scene.WriteVector3(out target, 
		                          transform.position.x + x * leafRand / 10.0f * Time.deltaTime,
		                          transform.position.y + y * leafRand / 10.0f * Time.deltaTime,
		                          transform.position.z
		                          );
		csMain_Scene.WriteVector3(out center, 
		                          0.0f, 
		                          0.0f,
		                          OFF_SCREEN
		                          );
		csMain_Scene.WriteVector3(out leftScreen, 
		                          OFF_SCREEN,
		                          0.0f,
		                          OFF_SCREEN
		                          );

		// If the game is still going, this is normal leaf movement.
		if (!csMenu_Scene.gameOver) 
		{
			// Movement for leaf starting out.
			if (transform.position.z >= START_Z - 20) 
			{
				transform.position = Vector3.MoveTowards(transform.position, center, moveStep / 2.0f);
				transform.Rotate(Vector3.forward, rotateStep);
			}
			// Movement leaf past breaking point, change rotation.
			else if (transform.position.z < BREAK_POINT) 
			{
				transform.position = Vector3.MoveTowards(transform.position, slow, moveStep / 2.0f);
				transform.Rotate(Vector3.forward, - rotateStep);
			}
			// Normal movement towards user.
			else 
			{
				transform.position = Vector3.MoveTowards(transform.position, target, moveStep / 2.0f);
				transform.position = Vector3.MoveTowards(transform.position, center, moveStep / 3.0f);
				transform.Rotate(Vector3.forward, rotateStep);
			}
		}
		// Once the game is over make all the leaves rotate and move towards left screen.
		else 
		{
			transform.position = Vector3.MoveTowards(transform.position, leftScreen, moveStep);
			transform.Rotate(Vector3.left, rotateStep);
		}

		// Leaf has gone off the screen, set the flag to signal returning to the menu.
		if (transform.position.z < OFF_SCREEN + 5) 
		{
			// Also reset the leaf for good measure.
			transform.position = new Vector3(x, y, START_Z);

			// To indicate the game is over.
			csMenu_Scene.gameOver = true;

			if (DEBUG_LOCAL) 
			{
				Debug.Log("OFF SCREEN");
			}
		}

	} // void  Update()

} // public class csLeaf : MonoBehaviour 
