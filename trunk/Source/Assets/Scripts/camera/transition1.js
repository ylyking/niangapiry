#pragma strict


public var gSkin 			: GUISkin;
public var isLoading		: boolean	= false;
private var PlayerCollide	: boolean	= false;
public var jumpLevel		: String 	="Level1";

public var SceneCamera 		: Camera;
public var PlayerCamera 	: Camera;

public var PlayerPrefab 	: GameObject;
private var PlayerController: PlayerControls;

private var ThisTransform 	: Transform;  
private var ObjAnim			: AnimSprite;

public var IntroA       : AudioClip = null;
public var Falling       : AudioClip = null;
public var IntroB       : AudioClip = null;




function Start()
{
	ThisTransform = transform;
    AudioManager.Get().Play(IntroA, ThisTransform, 0.75, 1.0);
	
	ObjAnim = GetComponent(AnimSprite) as AnimSprite ; 
	PlayerController = PlayerPrefab.GetComponent(PlayerControls) as PlayerControls;
	
	GameManager.Get().IsPlaying = false;
	
	PlayerController.enabled = false;	
//	isLoading = true;
	
	SceneCamera.enabled = true;
	PlayerCamera.enabled = false;
	
	var timertrigger = Time.time + .75f;	
	while( timertrigger > Time.time )// Animate Tunning #1
	{
		ObjAnim.PlayFrames( 1, 2, 2, 1);	
		yield;
	}
	
	ObjAnim.PlayFrames( 2, 0, 1, 1);
//	Random.seed = Time.renderedFrameCount;
	
	if( SceneCamera )	
	{
		SceneCamera.orthographicSize = 0.01;
	
	 	while(SceneCamera.orthographicSize < 0.35)
		{
			SceneCamera.orthographicSize += Time.deltaTime * 0.1f;
			yield;
		}
	}
	isLoading = false;
	
	timertrigger = Time.time + 2.0f;
 	while( timertrigger > Time.time )// Animate Cabalgarcha #1
	{
		ObjAnim.PlayFrames( 0, 0, 6, -1);	
		yield;
	}
	
	timertrigger = Time.time + 0.25f;
	while( timertrigger > Time.time )// Animate Tunning #2
	{
		ObjAnim.PlayFrames( 1, 2, 2, 1);	
		yield;
	}
	
	PlayerCamera.enabled 	= true;
	SceneCamera.enabled 	= false;
	PlayerPrefab.transform.position.y = 80.0f;
	PlayerController.enabled = true;
	

    var FallOut : AudioSource = AudioManager.Get().Play(Falling, ThisTransform, 1.0, 1.0);
    
 	while( !PlayerCollide )
	{
		yield;
	}
	
	FallOut.Stop();

	SceneCamera.enabled 		= true;
	PlayerCamera.enabled 		= false;
	
	timertrigger = Time.time + 0.25f;
	while( timertrigger > Time.time )// Animate Tunning #3
	{
		ObjAnim.PlayFrames( 1, 2, 2, 1);	
		yield;
	}
	
	GameManager.Get().IsPlaying = false;
	
    AudioManager.Get().Play(IntroB, ThisTransform, 0.65, 1.0);
	
	SceneCamera.orthographicSize = 0.15;
	timertrigger = Time.time + 3.0f;	// Animate Cabalgarcha #2
 	while( timertrigger > Time.time )
	{
		ObjAnim.PlayFrames( 0, 0, 6, 1);	
		yield;
	}
	isLoading = true;
	ObjAnim.PlayFrames( 2, 1, 1, 1);
	yield WaitForSeconds( 1.0f);	
	
	GameManager.Get().ShowFlash(2.0);
	GameManager.Get().ResetStats();
	
	Application.LoadLevel(jumpLevel);
	GameManager.Get().IsPlaying = true;
	
	
}
function Update()
{
	if ( Input.GetButtonDown( "Fire1" ) || Input.GetKeyDown("return") )
	{
		GameManager.Get().ResetStats();
		GameManager.Get().IsPlaying = true;
		Application.LoadLevel(jumpLevel);
	}
}

function OnTriggerEnter (other : Collider) {
	
	if(other.tag=="Player")
		PlayerCollide = true;
}



//function OnGUI()
//{
//	if(gSkin)
//		GUI.skin = gSkin;
//	else
//		Debug.Log("StartMenuGUI : GUI Skin object missing!");
//		
//		if (isLoading)
//		{
//			GUI.Label (  Rect( (Screen.width * .5f)-100 , (Screen.height * .5f) , 400, 50), "Loading..." );
//			return;
//		}
//
//		var backgroundStyle : GUIStyle = new GUIStyle();		
//		
//		GUI.Label (  Rect( 0, 0, Screen.width, Screen.height), "", backgroundStyle);
//
//}