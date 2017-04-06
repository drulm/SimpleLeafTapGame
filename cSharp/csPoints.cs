//----------------------------------------------------------------------
// csPoints.cs
//		Just move, fade, and destroy the points that show up.
using UnityEngine;
using System.Collections;

public class csPoints : MonoBehaviour {

	private float moveSpeed = 3;							// The move speed
	
	void Update()
	{
		Color c = GetComponent<Renderer>().material.color;

		// The move delta time
		float moveStep = moveSpeed * Time.deltaTime;

		// Move the bonus points up.
		transform.Translate(Vector3.up * moveStep);

		c.a -= Time.deltaTime;
		GetComponent<Renderer>().material.color = c;
		if (c.a <= 0.0f)
		{
			Destroy(this.gameObject);
		}
	}
}
