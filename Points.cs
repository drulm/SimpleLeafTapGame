//----------------------------------------------------------------------
// Points.cs
//		Just move, fade, and destroy the points that show up.

using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------
// Points class for Unity3d MonoBehaviour
public class Points : MonoBehaviour {

	// Constants
	private const float moveSpeed = 3.0f; // The move speed


	//----------------------------------------------------------------------
	//----------------------------------------------------------------------
	// Unity Update code.
	void Update()
	{
		// Pull in the current color
		Color c = GetComponent<Renderer>().material.color;

		// Calculate the move delta time
		float moveStep = moveSpeed * Time.deltaTime;

		// Move the bonus points up
		transform.Translate(Vector3.up * moveStep);

		// Decrease the alpha channel to fade out points
		c.a -= Time.deltaTime;
		// Save the color back with the modified alpha channel
		GetComponent<Renderer>().material.color = c;

		// If the alpha channel is zero or negative, destroy the points game object 
		if (c.a <= 0.0f)
		{
			Destroy(this.gameObject);
		}

	} // end: void Update()

} // end: public class Points : MonoBehaviour {
