// Scene Start

#pragma strict
#pragma downcast

// GLOBALS
static var score : 				int;
static var hiScore : 			int;
static var gameOver : 			boolean		= false;
static var returnMenu : 		boolean		= false;

static var currentCubes :   	int			= 0;			// How many cubes currently

var scoreObj : 					GameObject;					// A cube Prefab

private var fontSizeRatio : 	float = 1.4;
private var fontSize : 			int;
private var myAudio : 			AudioSource[];


//----------------------------------------------------------------------
// Pull in Audio sources
function SetupAudio() {
	var i : int;
	var aSources = GetComponents(AudioSource);
	
	myAudio = new AudioSource[10];
    for (i=0; i < aSources.length; i++) {
    	myAudio[i] = aSources[i];
    }
}

//======================================================================
// !VARIABLES
//======================================================================
var font : 					Font;
var loadingText : 			GameObject;

var mainMenu : 				boolean = true;

// var CustomGUISkin : 		GUISkin;

private var _oldWidth: float;
private var _oldHeight: float;
private var ratio: float = 15;

// Start function
function Start () {
	SetupAudio();
	currentCubes = 0;
}

// Update funtion
function Update () {
	if (! Application.isLoadingLevel) {
		var loadingMsg : GameObject;
		var mn : float;
	    if (_oldWidth != Screen.width || _oldHeight != Screen.height) {
	        _oldWidth = Screen.width;
	        _oldHeight = Screen.height;
	        mn = Screen.width < Screen.height ? Screen.width : Screen.height;
	        fontSize = mn / ratio * fontSizeRatio;
	    }
	    if (Random.Range(1, parseInt(25 + 1.0 / Time.deltaTime)) == 1) {
	    	myAudio[Random.Range(1,7)].Play();
	    }
    }
}

function OnGUI() {
	var screenHeight : int = Screen.height;
	var screenWidth : int = Screen.width;
	var r : int = screenHeight / 10;
	var loadingMsg : GameObject;

	// GUI.skin = CustomGUISkin;
	GUI.skin.label.font = GUI.skin.button.font = GUI.skin.box.font = font;
    GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = fontSize;

	if (mainMenu) {
		// Make a group on the center of the screen
		GUI.BeginGroup(Rect(r*2, r*3, Screen.width-r*2, r*7));

		if (score > hiScore) {
			hiScore = score;
		}
		GUI.Box(Rect(0.0, 0.0, Screen.width-r*4.0, r*6.5), "");
		if (gameOver) {
			// Make a box on GUI
			GUI.Label(Rect(r*1.5, 0.0, Screen.width-r*6.5, r*2.0), score + " points, most is " + hiScore);
		}
		
		// Start the scene
		if (GUI.Button(Rect(10, r*2.0, Screen.width-r*10.0, r), "start")) {
			myAudio[3].Play();
			gameOver = false;
			returnMenu = false;
			score = 0;
			currentCubes = 0;
			Application.LoadLevel("main_scene");
		}
		if (GUI.Button(Rect(10.0, r*3.0, Screen.width-r*10.0, r), "Rules&Rate")) {
			myAudio[5].Play();
			Application.OpenURL("market://details?id=com.AwakeLand.fallleaftapgame");
		}
		if (GUI.Button(Rect(10.0, r*4.0, Screen.width-r*10.0, r), "end...")) {
			//myAudio[3].Play();
			Application.Quit();
		}
		
		// Row 2 - Social Links
		if (GUI.Button(Rect(Screen.width-r*9.0, r*3.0, r*4.8, r), "@fb share")) {
			myAudio[2].Play();
			Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.AwakeLand.fallleaftapgame");
		}
		if (GUI.Button(Rect(Screen.width-r*9.0, r*4.0, r*4.8, r), "devel. WWW")) {
			myAudio[6].Play();
			Application.OpenURL("http://awakeland.com");
		}
		if (hiScore > 0 && GUI.Button(Rect(10.0, r*5.2, Screen.width-r*6.0, r), "@tweet points")) {
			myAudio[4].Play();
			Application.OpenURL("http://twitter.com/home?status=I+scored+" + hiScore + "+in+%22Fall+Leaf+Tap%22%2C+a+%23freegame+for+%23Android+from+%40AwakeLandGames at https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.AwakeLand.FallLeafTap");
		}
		// End of GUI
		GUI.EndGroup();
	}

}





