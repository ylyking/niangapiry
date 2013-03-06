#pragma strict
import System.Collections.Generic;


public var aTexture 	: Texture2D;
public var gSkin 		: GUISkin;
public var isLoading	: boolean	= false;
public var DisplayMenu	: boolean	= false;
public var DisplayInfo	: boolean	= false;
public var jumpLevel	: String 	="JumpGameSinglePlayer";
public var highScores	: String 	="HighScores";

public var CamBack		: Camera;
public var CamFront		: Camera;
public var CamMiddle	: Camera;

public var SkyTransform 	: Transform;
private var ThisTransform 	: Transform;  

private var ChooseOption	: uint = 0;
private var TotalOptions	: uint = 3;
private var OptionsList		: Dictionary.< int, String> = new Dictionary.< int, String>() ;
public var PreOpening       : AudioClip = null;
public var Opening          : AudioSource = null;



function Start()
{
//	audio.
    
	ThisTransform = transform;
    AudioManager.Get().Play(PreOpening, ThisTransform, 1.0, 1.0);
    Opening = gameObject.GetComponent(AudioSource);
    
    
	OptionsList.Add( 0,  "Start Game");
	OptionsList.Add( 1,  "Help/Info");
	OptionsList.Add( 2,  "Quit");
	
	if (GameManager.Get())
	{
		GameManager.Get().Init() ;
		GameManager.Get().TopScore = PlayerPrefs.GetInt("TopScore");
	}

	if( CamFront )
	{
		camera.orthographicSize = 0.01;
		CamBack.fov 	= 160;
		CamMiddle.fov 	= 160;
		CamFront.fov 	= 160;
 		while( true ) //camera.orthographicSize < 0.9 && CamFront.fov > 60) 
		{
			yield CoUpdateCAM();
			yield CoUpdateScene();
		}
	}
}

function Update()	
{
	GameManager.Get().Update(Time.deltaTime);

    if( Input.GetKeyDown( "up" ) && ( ChooseOption > 0 ) && !DisplayInfo)
             ChooseOption--;                    
    if( Input.GetKeyDown( "down" ) && ( ChooseOption < ( TotalOptions -1 ) ) && !DisplayInfo)
            ChooseOption++;
                                                        
    if (Input.GetButtonDown( "Fire1" ) || Input.GetKeyDown("return") )
	    switch( ChooseOption )
	    {
	    	case 0:
	    		GameManager.Get().ShowFlash( 2.0 );
	    		Application.LoadLevel(jumpLevel);	
	    		break;
	    	case 1:
	    		GameManager.Get().ShowFlash( 0.25 );
	    		DisplayInfo = !DisplayInfo;
	    		break;
	     	case 2:
	     		GameManager.Get().ShowFlash( 1.0 );
	     		Application.Quit();
	    		break;
	    }
	    
}

function CoUpdateCAM()
{
	ThisTransform.position.x = System.Math.Round(( Mathf.Sin( Time.time  ) *0.05f), 4);

	if(camera.orthographicSize < 0.9)
	{
		camera.orthographicSize += 1 * Time.deltaTime;
		yield;
	}
	else	return;
}

function CoUpdateScene() 
{
	if (SkyTransform)
		 SkyTransform.Rotate( Vector3( 0, 0, 0.1f) );
		 
	
	DisplayMenu = (CamFront.fov <= 58);
	
	if ( !DisplayMenu )
	{
		CamBack.fov 	-=  100 * Time.deltaTime;
		CamMiddle.fov 	-=  100 * Time.deltaTime;
		CamFront.fov 	-=  100 * Time.deltaTime;
		yield;
	}
	else
		return;
}


function OnGUI()
{
	GameManager.Get().Render();
	
	if (!DisplayMenu)
			return;
			
    if ( Opening && !Opening.isPlaying )
        Opening.Play();
			
	if( Event.current.type == EventType.Repaint) 
	{
		
		if(gSkin)
		{
			GUI.skin = gSkin;
			GUI.skin.label.fontSize = 32;
			GUI.color = Color(1, 0.36, 0.22, 1);
		}	   		
		else
			Debug.Log("StartMenuGUI : GUI Skin object missing!");
			
		if (isLoading)
		{
			GUI.Label (  Rect( (Screen.width * .5f)-100 , (Screen.height * .5f) , 400, 50), "Loading..." );
			return;
		}
		if (DisplayInfo)
		{
			GUI.color = new Color(1, 0.36, 0.22, 1);
			GUI.Box(Rect((Screen.width * .5f)-(Screen.width * .3f) ,
						 (Screen.height * .5f)-(Screen.height * .35f) ,
						  (Screen.width * .6f), (Screen.height * .7f)),
				"\n CONTROLES: \n Desplazamiento: <- Cursores direccionales -> \n Saltar/Planear: Barra Espacio \n Correr/Agarrar/Tirar: Tecla Control \n Usar Item/Poder Especial: Tecla ALT"
				+ "\n\n\n CONTROLS: \n Move: <- Direction Keys -> \n Jump: SpaceBar \n Run & Grab: Control \n PowerUp/UseItem: Alt"
				+ "\n\n\n\n Programed By CarliXYZ \n Art & Design By FAC! \n Music By Menta-Beat \n\n ÑANGAPIRY® - 2012 - SUNHOUSE - ");
			GUI.color = Color.clear;
			return;
		}

		GUI.Label (  Rect( (Screen.width * .05f)-40 , (Screen.height * .05f) -10 , 400, 100), "Top Score\n" + GameManager.Get().TopScore );

		for ( var Option : int in OptionsList.Keys )
		{
		
			if ( Option == ChooseOption )
				GUI.color = Color.white;
  			else
				GUI.color = Color(1, 0.36f, 0.22f, 1);
				
            GUI.Label( Rect( (Screen.width * .65f), (Screen.height *.8f) + Option * 32, 400, 50), OptionsList[Option]);
		
		}
	}
}


function OnApplicationQuit()
{
	if (GameManager.Get())
		PlayerPrefs.SetInt("TopScore", GameManager.Get().TopScore);

}