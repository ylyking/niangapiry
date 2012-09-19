#pragma strict


/*
Little script to fade an image (stretched to fit the screen, so use a 1x1 black pixel for a simple black fade in/out).
Simply apply it to your camera (or an empty GameObject), set the texture to use, set the fadeSpeed and call fadeIn() or fadeOut()
Easiest way is probably to apply this script to your main camera in the scene, then wherever you want to fade use something like:
Camera.main.SendMessage("fadeOut");
or
Camera.main.SendMessage("fadeIn");
Enjoy! 
*/

public var startAlpha 		:  float 	= 1;  										// Alpha start value   
public var fadeTexture 		: Texture2D;											// Texture used for fading
public var fadeDuration 	: float 	= 2;					 					// Default time a fade takes in seconds
public var displayDuration 	: float 	= 2;
public var guiDepth 		: int 		= -1000;									// Depth of the gui element
public var fadeIntoScene	: boolean 	= true;										// Fade into scene at start
public var nextLevel 		: String;
   
//  PRIVATE FIELDS
// ----------------------------------------
private var currentAlpha	: float 	= 1;										// Current alpha of the texture
private var currentDuration	: float;												// Current duration of the fade
private var fadeDirection	: int 		= -1;										// Direction of the fade
private var targetAlpha 	: float 	= 0;										// Fade alpha to
private var alphaDifference	: float		= 0;										// Alpha difference
private var backgroundStyle : GUIStyle 	= new GUIStyle();							// Style for background tiling
private var dummyTex		: Texture2D;
var alphaColor 				: Color 	= new Color();								// Color object for alpha setting
   
//  FADE METHODS
// ----------------------------------------
	
function Update()
{
	if (currentAlpha == 0)
    	            FadeOut();
}
   
function FadeIn( duration : float, to : float)
{
    currentDuration = duration;														// Set fade duration
    targetAlpha = to;																// Set target alpha
    alphaDifference = Mathf.Clamp01(currentAlpha - targetAlpha);					// Difference
    fadeDirection = -1;																// Set direction to Fade in
}
   
function FadeIn()
{
    FadeIn(fadeDuration, 0);
}

function FadeIn( duration : float)
{
    FadeIn(duration, 0);
}

function FadeOut( duration : float, to : float)
{
    currentDuration = duration;														// Set fade duration  
    targetAlpha = to;																// Set target alpha
    alphaDifference = Mathf.Clamp01(targetAlpha - currentAlpha);				    // Difference
    fadeDirection = 1;															    // Set direction to fade out
}

function FadeOut()
{
    FadeOut(fadeDuration, 1);
}   

function FadeOut( duration : float)
{
    FadeOut(duration, 1);
}

//  STATIC FADING FOR MAIN CAMERA
// ----------------------------------------

static function FadeInMain( duration : float, to : float)
{
    GetInstance().FadeIn(duration, to);
}

static function FadeInMain()
{
    GetInstance().FadeIn();
}

static function FadeInMain( duration : float)
{
    GetInstance().FadeIn(duration);
}

static function FadeOutMain( duration : float, to : float)
{
    GetInstance().FadeOut(duration, to);
}

static function  FadeOutMain()
{
    GetInstance().FadeOut();
}

static function FadeOutMain( duration : float)
{
    GetInstance().FadeOut(duration);
}

// Get script fom Camera
static function GetInstance() : CameraFade
{
    // Get Script
    var fader : CameraFade = Camera.main.GetComponent("CameraFade") as CameraFade;
    // Check if script exists
    if (fader == null)
    {
        Debug.LogError("No FadeInOut attached to the main camera.");
    }   
    return fader;
}

// ----------------------------------------
//  SCENE FADEIN
// ----------------------------------------

function Start()
{
//    Debug.Log("Starting FadeInOut");
   
    dummyTex = new Texture2D(1,1);
    dummyTex.SetPixel(0,0,Color.clear);
    backgroundStyle.normal.background = fadeTexture;
    currentAlpha = startAlpha;
    if (fadeIntoScene)
    {
        FadeIn();
    }
}

//  FADING METHOD
// ----------------------------------------

function OnGUI()
{   
    // Fade alpha if active
    if ((fadeDirection == -1 && currentAlpha > targetAlpha) ||
        (fadeDirection == 1 && currentAlpha < targetAlpha && displayDuration <= 0))
    {
        // Advance fade by fraction of full fade time
        currentAlpha += (fadeDirection * alphaDifference) * (Time.deltaTime / currentDuration);
        // Clamp to 0-1
        currentAlpha = Mathf.Clamp01(currentAlpha);
    } else displayDuration  -= Time.deltaTime;
   
    // Draw only if not transculent
    if (currentAlpha >= 0)
    {
        // Draw texture at depth
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        alphaColor.a = currentAlpha;
        GUI.color = alphaColor;
        GUI.depth = guiDepth;
        GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), dummyTex, backgroundStyle);
    }
	
	if(fadeDirection==1 && currentAlpha==1) Application.LoadLevel(nextLevel);	
}

