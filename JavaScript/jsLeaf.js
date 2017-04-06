// Cube.js

#pragma strict
#pragma downcast

private var DEBUG_LOCAL: 			boolean	= false;				// Starting Distance

private var START_Z: 				float	= 100.0;			// Starting Distance
private var FORCE_LEAF: 			float	= 20;				// For force
private var FORCE_HALF: 			float	= FORCE_LEAF / 2;	// For force

private var BREAK_POINT:			float	= 30.0;				// Where to move leaf away

private var OFF_SCREEN: 			float	= -35.0;			// For forces

private var SIZE_X: 				float	= 10;
private var SIZE_Y: 				float	= 6;

private var rotateSpeed : 			float 	= 200.0;			// The rotate speed
private var moveSpeed : 			float 	= 15;				// The move speed

private var lastRotate : 			float	= 0;				// The rotate speed
private var slow : 					Vector3;					// Direction near end

private var x : float;			// Make it fly out
private var y : float;
private var leafRand: float;

function Start () {
	x = 	(Random.Range(0.0, SIZE_X) - SIZE_X / 2.0);
	y = 	(Random.Range(0.0, SIZE_Y) - SIZE_Y / 2.0);
	SetLeafRand();
	if (DEBUG_LOCAL) {
		print("LeafRand = " + leafRand);
	}
}

function SetLeafRand() {
	leafRand = jsMenu_Scene.currentCubes / 4.0 + 2.0;
	leafRand = Mathf.Min(leafRand, 50.0);
	leafRand = Mathf.Max(leafRand, 2.0);
	slow = Vector3(
		x * leafRand, 
		y * leafRand,
		OFF_SCREEN
	);
}

function Update () {
	var rot: float 			= 	(Random.Range(0.0, FORCE_LEAF) - FORCE_HALF) / 10.0;
	
	if (lastRotate > 0 && rot < 0 || lastRotate < 0 && rot > 0) {
		rot = -rot;
	}
	
	lastRotate = rot;
	
	var rotateStep: float = rotateSpeed * Time.deltaTime * rot;	// The rotate delta time
	var moveStep: float = moveSpeed * Time.deltaTime;			// The move delta time

	transform.Translate(Vector3.forward * - moveStep);
	
	var target:Vector3 = Vector3(
			transform.position.x + x * leafRand / 10.0 * Time.deltaTime, 
			transform.position.y + y * leafRand / 10.0 * Time.deltaTime,
			transform.position.z
		);
	var center:Vector3 = Vector3(
		0.0, 
		0.0,
		OFF_SCREEN
	);
	var leftScreen:Vector3 = Vector3(
		OFF_SCREEN, 
		0.0,
		OFF_SCREEN
	);
	
	if (!jsMenu_Scene.gameOver) {
		// Near end.
		if (transform.position.z >= START_Z - 20) {
			transform.position = Vector3.MoveTowards(transform.position, center, moveStep / 2.0);
			transform.Rotate(Vector3.forward, rotateStep);
		}
		else if (transform.position.z < BREAK_POINT) {
			transform.position = Vector3.MoveTowards(transform.position, slow, moveStep / 2.0);
			transform.Rotate(Vector3.forward, - rotateStep);
		}
		else {
			transform.position = Vector3.MoveTowards(transform.position, target, moveStep / 2.0);
			transform.position = Vector3.MoveTowards(transform.position, center, moveStep / 3.0);
			transform.Rotate(Vector3.forward, rotateStep);
		}
	}
	else {
		//transform.position = Vector3.MoveTowards(transform.position, target, moveStep);
		transform.position = Vector3.MoveTowards(transform.position, leftScreen, moveStep);
		transform.Rotate(Vector3.left, rotateStep);
	}
	
	if (transform.position.z < OFF_SCREEN + 5) {
		transform.position = Vector3(x, y, START_Z);
		jsMenu_Scene.gameOver = true;
		if (DEBUG_LOCAL) {
			print("OFF SCREEN");
		}
	}
}

