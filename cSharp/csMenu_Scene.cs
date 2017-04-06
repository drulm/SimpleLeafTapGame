//----------------------------------------------------------------------
// csMenu_Scene.cs
//		Handles the main (and only) menu for the game.
using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------
// csMenu_Scene class for Unity3d MonoBehaviour
public class csMenu_Scene : MonoBehaviour 
{
	// Static globals available to other scripts
	public static 	bool 		gameOver				= false;
	public static 	bool 		returnMenu				= false;
	public static	bool		mainMenu				= true;
	public static 	int 		currentCubes			= 0;
	public static 	int 		score;
	public static 	int 		hiScore;
	
	public 			GameObject 	scoreObj;
	public 			Font 		menuFont;
	
	GameObject loadingText;
	
	// GUISkin CustomGUISkin;

	// Private vars
	private float fontSizeRatio = 1.0f;
	private int fontSize;
	private AudioSource[] myAudio;
	private float _oldWidth;
	private float _oldHeight;
	private float ratio = 15;


	//----------------------------------------------------------------------
	// Start function
	void  Start()
	{
		SetupAudio();
		currentCubes = 0;
	}

	//----------------------------------------------------------------------
	// Pull in Audio sources
	void SetupAudio()
	{
		int i;
		AudioSource[] aSources = GetComponents<AudioSource>();
		myAudio = new AudioSource[10];
		for (i=0; i < aSources.Length; i++) 
		{
			myAudio[i] = aSources[i];
		}
	}
	
	// Update funtion
	void  Update()
	{
		if (! Application.isLoadingLevel) 
		{
			//			GameObject loadingMsg;
			float mn;
			if (_oldWidth != Screen.width || _oldHeight != Screen.height) 
			{
				_oldWidth = Screen.width;
				_oldHeight = Screen.height;
				mn = Screen.width < Screen.height ? Screen.width : Screen.height;
				fontSize = (int)(mn / ratio * fontSizeRatio);
			}
			if (Random.Range(1, (int)(25.0 + 1.0f / Time.deltaTime)) == 1) 
			{
				myAudio[Random.Range(1,7)].Play();
			}
		}
	}
	
	void  OnGUI ()
	{
		int screenHeight = Screen.height;
		//		int screenWidth = Screen.width;
		int r = screenHeight / 10;
		//		GameObject loadingMsg;
		
		// GUI.skin = CustomGUISkin;
		GUI.skin.label.font = GUI.skin.button.font = GUI.skin.box.font = menuFont;
		GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = fontSize;
		
		if (mainMenu) 
		{
			// Make a group on the center of the screen
			GUI.BeginGroup( new Rect(r*2, r*3, Screen.width-r*2, r*7));
			
			if (score > hiScore) 
			{
				hiScore = score;
			}
			GUI.Box( new Rect(0.0f, 0.0f, Screen.width-r*4.0f, r*6.5f), "");
			if (gameOver) 
			{
				// Make a box on GUI
				GUI.Label( new Rect(r*1.5f, 0.0f, Screen.width-r*6.5f, r*2.0f), score + " points, most is " + hiScore);
			}
			
			// Start the scene
			if (GUI.Button( new Rect(10, r*2.0f, Screen.width-r*10.0f, r), "start")) 
			{
				myAudio[3].Play();
				gameOver = false;
				returnMenu = false;
				score = 0;
				currentCubes = 0;
				Application.LoadLevel("main_scene");
			}
			if (GUI.Button( new Rect(10.0f, r*3.0f, Screen.width-r*10.0f, r), "Rules&Rate")) 
			{
				myAudio[5].Play();
				Application.OpenURL("market://details?id=com.AwakeLand.fallleaftapgame");
			}
			if (GUI.Button( new Rect(10.0f, r*4.0f, Screen.width-r*10.0f, r), "end...")) 
			{
				myAudio[3].Play();
				Application.Quit();
			}
			
			// Row 2 - Social Links
			if (GUI.Button( new Rect(Screen.width-r*9.0f, r*3.0f, r*4.8f, r), "@fb share")) 
			{
				myAudio[2].Play();
				Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.AwakeLand.fallleaftapgame");
			}
			if (GUI.Button( new Rect(Screen.width-r*9.0f, r*4.0f, r*4.8f, r), "devel. WWW")) 
			{
				myAudio[6].Play();
				Application.OpenURL("http://awakeland.com");
			}
			if (hiScore > 0 && GUI.Button( new Rect(10.0f, r*5.2f, Screen.width-r*6.0f, r), "@tweet points")) 
			{
				myAudio[4].Play();
				Application.OpenURL("http://twitter.com/home?status=I+scored+" + hiScore + "+in+%22Fall+Leaf+Tap%22%2C+a+%23freegame+for+%23Android+from+%40AwakeLandGames at https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.AwakeLand.FallLeafTap");
			}
			// End of GUI
			GUI.EndGroup();
		}
		
	}

}