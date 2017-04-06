// main_scene.js

#pragma strict
#pragma downcast

private var DEBUG_LOCAL: 			boolean	= false;		// Starting Distance

private var LEAF_NAME: 				String	= "leaf";		// leaf base name

private var MAX_CUBES: 				int				= 50;		// Max size
private var MAX_LEAF_TYPES:			int 			= 5;		// Max # materials
private var START_Z: 				float			= 100.0;	// Starting Distance
private var rayDistance : 			float 			= 200.0;	// The click-ray distance

private var FORCE_LEAF: 			float	= 6;				// For force
private var FORCE_HALF: 			float	= FORCE_LEAF / 2;	// For force

private var SIZE_X: 				float	= 10;
private var SIZE_Y: 				float	= 6;

private var hitType: 				String	= "";

private var myAudio : 				AudioSource[];

private var timeDown : 				float = 0.0;

var cubes :							GameObject[];				// All cubes
var cube : 							GameObject;					// A cube Prefab

var anim1 : 						GameObject;					// Touch animation

var points : 						GameObject;					// Touch animation

var maxCubes : 						int;						// Current max cubes

var scoreAdd : 						int;
var bonusAdd : 						int;

var leafMaterials : 				Material[];					// Different leafs


//----------------------------------------------------------------------
// Unity Start Fn
function Start () {
	var i:int;
	
	maxCubes = 3;										// Start slow
//	GetInstance(jsMenu_Scene).currentCubes = 0;			// None to start
	jsMenu_Scene.currentCubes = 0;
	
	scoreAdd = 1;
	bonusAdd = 0;
	
	cubes = new GameObject[MAX_CUBES];
	
	SetupAudio();
}


function SetupAudio() {
	var i : int;
	var aSources = GetComponents(AudioSource);
	
	myAudio = new AudioSource[15];
    for (i=0; i < aSources.length; i++) {
    	myAudio[i] = aSources[i];
    }
}


//----------------------------------------------------------------------
// Unity Update Fn
function Update () {
	if (! Application.isLoadingLevel) {
		AddNewCube();
		CheckMouse();
		CheckGameOver();
	}
}

//----------------------------------------------------------------------
// Game over here?
function CheckGameOver() {
	if (jsMenu_Scene.gameOver) {
		timeDown += Time.deltaTime;
		if (timeDown > 3.0) {
			jsMenu_Scene.returnMenu = true;
		}
	}
	if (jsMenu_Scene.returnMenu) {
		Application.LoadLevel("menu_scene");
	}
}

//----------------------------------------------------------------------
// Check mouse click
function CheckMouse() {
	var hitGameObjectName : 			String;
	var hitMaterial : 					String;
	var hit : 							RaycastHit = new RaycastHit();
	var x: float 			= 	(Random.Range(0.0, SIZE_X) - SIZE_X / 2);
	var y: float 			= 	(Random.Range(0.0, SIZE_Y) - SIZE_Y / 2);
	var i: int;
	var hitX: float;
	var hitY: float;
	var hitZ: float;
	
	var hitScored: boolean;
	
	var pos: Vector3;
	
	hitScored = false;
	
	// If the mouse button is clicked and there are no moving objects, and coins left.
	if (Input.GetMouseButtonDown(0)) {
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		if (!jsMenu_Scene.gameOver && Physics.Raycast(ray, hit, rayDistance)) {			
			// Get name
			hitGameObjectName = hit.collider.gameObject.name;
			if (DEBUG_LOCAL) {
				print("name: " + hitGameObjectName + " ( " + hitX + " , " + hitY + " , " + hitZ + " ) ");
			}
			
			// Save position
			pos = hit.collider.gameObject.transform.position;

			if (hitGameObjectName.Contains(LEAF_NAME)) {		
				// Reset location, and add another cube.
				hit.collider.gameObject.transform.position.x = x;
				hit.collider.gameObject.transform.position.y = y;
				hit.collider.gameObject.transform.position.z = START_Z;
				
				// Go set a new random leaf, prevents cheating!
				RandomLeaf(hit.collider.gameObject);

				jsMenu_Scene.score += scoreAdd + bonusAdd;
				hitScored = true;
				
				hitMaterial = hitGameObjectName;
				hitMaterial = hitMaterial.Replace(LEAF_NAME, "");
				
				if (hitMaterial.Trim() == hitType.Trim()) {
					IncreaseBonus();
				}
				else {
					bonusAdd = 0;
				}
				// Is 0 to 5
				hitType = hitMaterial;

				// Plays 1 to 6
				myAudio[parseInt(hitType) + 1].Play();
				CreateAnim1(pos);

				// Chance of new object is less and less.
				if (Random.Range(1,maxCubes * 2) == 1) {
					maxCubes++;
				}
				if (DEBUG_LOCAL) {
					print("Hit");
				}
			}
			else {
				if (DEBUG_LOCAL) {
					print("Miss");
				}
			}
			if (hitScored) {
				scoreAdd++;
			}
		}
		if (!jsMenu_Scene.gameOver) {
			if (!hitScored) {
				myAudio[11].Play();
				scoreAdd = scoreAdd / 2;
				scoreAdd = scoreAdd == 0 ? 1 : scoreAdd;
				bonusAdd = 0;
			}
		}
		if (DEBUG_LOCAL) {
			print("score = " + jsMenu_Scene.score);
		}
	} // if (Input.GetMouseButtonDown(0)) {		
}


//----------------------------------------------------------------------
// Decide if we add a new cube
function AddNewCube() {
	// Add a Cube
	CreateCube(jsMenu_Scene.currentCubes);
}

//----------------------------------------------------------------------
// Create the unity 3D object
function CreateCube(cc: int) {
	var x: float 			= 	(Random.Range(0.0, SIZE_X) - SIZE_X / 2);
	var y: float 			= 	(Random.Range(0.0, SIZE_Y) - SIZE_Y / 2);
	var i: int;
	var j: int				=	cc;
	var r: int				=	Random.Range(0, MAX_LEAF_TYPES);
	
	if (jsMenu_Scene.currentCubes < MAX_CUBES && jsMenu_Scene.currentCubes < maxCubes) {
		cubes[cc] = Instantiate(cube, Vector3(x, y, START_Z), Quaternion.identity);
		cubes[cc].name = (LEAF_NAME + r.ToString()).Trim();
		cubes[cc].GetComponent.<Renderer>().material = leafMaterials[r];
		jsMenu_Scene.currentCubes++;
	}
}

//----------------------------------------------------------------------
// Create animation effect
function CreateAnim1(pos: Vector3) {
	var scoreText : GameObject;
	var crl: float = Mathf.Min(bonusAdd, 4) / 4.0;
	
	Instantiate(anim1, pos, Quaternion.identity);
	
	scoreText = Instantiate(points, Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
	scoreText.GetComponent(TextMesh).color = Color(1, 0, crl * 0.8,  1);
	if (bonusAdd > 0) {
		scoreText.GetComponent(TextMesh).fontStyle = FontStyle.Bold;
	}
	scoreText.GetComponent(TextMesh).text = "+" + (scoreAdd + bonusAdd).ToString();
	scoreText.GetComponent(TextMesh).fontSize = 24 + crl * 20;
}

//----------------------------------------------------------------------
// Increase the bonus
function IncreaseBonus() {
	bonusAdd = bonusAdd >= 1000 ? bonusAdd + 1000 : bonusAdd;
	bonusAdd = bonusAdd == 100 ? 1000 : bonusAdd;
	bonusAdd = bonusAdd == 10 ? 100 : bonusAdd;
	bonusAdd = bonusAdd == 0 ? 10 : bonusAdd;
	/*if (bonusAdd >= 2000) {
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
	}*/
}

//----------------------------------------------------------------------
function RandomLeaf(leaf: GameObject) {
	var r: int				=	Random.Range(0, MAX_LEAF_TYPES);
	leaf.name = (LEAF_NAME + r.ToString()).Trim();
	leaf.GetComponent.<Renderer>().material = leafMaterials[r];
}


