#pragma strict
#pragma downcast

import System.Collections.Generic;
import System.IO;

class GameManager 	//	: MonoBehaviour		
{
	private static var Instance : GameManager = null;
    private function GameManager() {;} 												// private Constructor protected
    public static function Get(): GameManager 										// Singleton Instance Accessor 
    {
    	if(Instance == null)
    		Instance = new GameManager();
        return Instance;
    }


	var Score 		: uint 		= 0;
	var Fruits 		: uint 		= 0;
	var TopScore 	: uint 		= 100;
	
	var FireGauge	: int   	= 0;								
	var Key 		: int 		= 0;
	var Health		: uint 		= 3;
	var Lifes		: uint 		= 3;
	
	var ShowState	: boolean 	= true;
	var IsPlaying	: boolean 	= false;
	var IsPaused	: boolean 	= false;
	
	var ShowDelay	: float	 	= 6.0;
	var TimeLapse	: float	 	= 0.0;
	var FlashLapse	: float	 	= 0.0;
	
	var FlashTex	: Texture2D ;	
	var FlashPos	: Rect		= Rect( 0, 0, Screen.width, Screen.height);	
	var FlashBang	: boolean 	= false;
	
	var lastUpdate	: float	 	= 0.0f;	
	var FlashX		: int	 	= 0;	
	var FlashY		: int	 	= 3;
	
	var HealthTex	: Texture2D ;
	var HealthPos	: Rect;
	var HealthCoord	: Rect = Rect( 0, 0, .25, .25);
	
	var LifesTex	: Texture2D ;
	var LifesPos	: Rect;
	var LifesCoord	: Rect = Rect( 0, 0, 0.5, 0.5);
	
	private var gSkin 		: GUISkin;

    public function Init() 
    {
    	gSkin = Resources.Load("ñGUI/ÑGUISkin B") as GUISkin;
		gSkin.label.fontSize 	= Mathf.RoundToInt( Screen.width * 0.035f );
//		gSkin.label.fontStyle 	= FontStyle.Bold;
		
		HealthTex = Resources.Load("ñGUI/Items") as Texture2D;
 		HealthPos = Rect( (Screen.width * .025f), (Screen.height * .75f), HealthTex.width * .65, HealthTex.height * .65);
 		
 		FlashTex = Resources.Load("ñGUI/Rainbow") as Texture2D;
		
		LifesTex = Resources.Load("ñGUI/Lifes") as Texture2D;
		LifesPos = Rect( (Screen.width * .85f), (Screen.height * .85f), LifesTex.width , LifesTex.height );

   	}       
        //////////////////////////////////////////////////////////////
        
    public function ShowStatus() 
    {
    	ShowState = true;
		TimeLapse = ShowDelay;
   	}   
        //////////////////////////////////////////////////////////////
        
    public function ShowFlash( FlashDelay : float )
    {
    	if ( FlashBang ) return;
    	FlashBang = true;
    	FlashLapse = FlashDelay;	
    	
    }
    
    public function SetPause( PauseState : boolean )
    {
    	IsPaused = PauseState;
    	var player = GameObject.FindGameObjectWithTag("Player");
    	
    	if ( IsPaused )
    	{
    		Time.timeScale = 0.00000000000001;
    		(player.GetComponent(PlayerControls)as PlayerControls).enabled = false;
    	}
    	else
    	{
    		Time.timeScale = 1f;
    		(player.GetComponent(PlayerControls)as PlayerControls).enabled = true;
    	}	
			AudioListener.pause = IsPaused;
    	
    }
    
        //////////////////////////////////////////////////////////////
    
    public function GameOver() 
    {
    	IsPlaying = false;
    	ResetStats();
    	ShowFlash( 2.0 );
    	
    	Application.LoadLevel("Intro");	 	
   	}       
        //////////////////////////////////////////////////////////////
          
    public function ResetStats() 
    {
    	if ( Score > TopScore)
			TopScore 	= Score;
		PlayerPrefs.SetInt("TopScore", TopScore);
		
    	Score 		= 0;
		Fruits 		= 0;
		
		FireGauge	= 0;								
		Key 		= 0;
			
		ShowState	= false;
		Health		= 3;		
		Lifes		= 3;
   	}       
        //////////////////////////////////////////////////////////////
        
    public function DeInit() {;}
        //////////////////////////////////////////////////////////////
        
    public function Update( lfTimestep : float ) 
    {

    	
    	if( IsPlaying && Input.GetKeyDown("escape") )
    		SetPause(!IsPaused); 
    	if( IsPaused && Input.GetKeyDown("q") )
    	{ 
    		SetPause(false);
    		GameOver();
    	}
    	
    	HealthCoord = Rect( Health * .25, 0, .25, .25);
    	
    	if ( ( (Fruits % 100) == 0 ) && Fruits )
    		{ ShowState = true; Lifes++; Fruits++;}
    	
    	if ( FlashBang )
    	{
			if( (Time.time - lastUpdate) >= 0.021f ) // secs to update increment: 1/12 (12 frames) my speed: 48 frames / 1 sec
				{ FlashX++; lastUpdate = Time.time; }
			
			FlashY -= System.Convert.ToByte( FlashX > 3 );
			
			FlashX = FlashX % 4;
			FlashY += 3 * System.Convert.ToByte( !FlashY ); //FlashY % 4;
			
    		FlashLapse -= lfTimestep;
    		FlashBang = (FlashLapse > 0.0f);
    	}
    	
    	if ( ShowState  )
    	{
    		TimeLapse -= lfTimestep;	                					// Decrease the message time
            ShowState = (TimeLapse > 0.0f);
        }
    }
        //////////////////////////////////////////////////////////////     
        
 	public function Render() 
 	{	
			if ( FlashBang && FlashTex)
			{
				GUI.DrawTextureWithTexCoords( FlashPos, FlashTex,  Rect( FlashX * .25, FlashY * .25, .25, .25 ) ); 
				return;
			}
		 	
		 	if (gSkin ) GUI.skin = gSkin; 
		 	
		 						
			if ( IsPaused )
			{
				GUI.color = Color(1, 0.36, 0.22, 1);
				GUI.Box(Rect((Screen.width * .5f)-(Screen.width * .15f) ,
							 (Screen.height * .5f)-(Screen.height * .15f) ,
							  (Screen.width * .3f), (Screen.height * .3f)),
								"\n\n - PAUSE - \n\n\n press 'Q' to Quit Game \n and return Main Menu ");
				GUI.color = Color.clear;
				return;
			}		
			
			if ( IsPlaying ) 
			{		
		 		GUI.DrawTextureWithTexCoords( HealthPos, HealthTex,  HealthCoord ); 
		 		
		 		if (!ShowState ) return;
		 		
		 		GUI.DrawTextureWithTexCoords( LifesPos, LifesTex, LifesCoord);  	
		 		
				GUI.color = Color.magenta;		
		 		GUI.Label (  Rect( (Screen.width * .05f)  , (Screen.height * .02f) , 100, 50),
		 			 "Score: " + Score + "\n" + "Fruits: " + Fruits );
		    	GUI.Label (  Rect( (Screen.width * .92f)  , (Screen.height * .9f) , 200, 50), "x" + Lifes );
		    }
		    
		    if( Lifes <= 0 )
		    {
//		    	GUI.skin.label.fontSize = 64;
//		    	GUI.skin.label.fontStyle = FontStyle.Bold;
		    	GUI.color = Color.magenta;	
		    	GUI.Label (  Rect( (Screen.width * .35f)  , (Screen.height * .5f) , 100, 50), "- GAME OVER -" );
		    }
    	
 	}
        //////////////////////////////////////////////////////////////
};

