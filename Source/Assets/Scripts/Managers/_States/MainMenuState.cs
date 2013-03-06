using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuState : GameState 
{
////	public GUISkin gSkin;
////	public bool isLoading		= false;
////	public bool DisplayMenu		= false;
////	public bool DisplayInfo		= false;
////	public string StartLevel	="JumpGameSinglePlayer";
////	public string highScores	="HighScores";

//    public AudioClip ChaChan = null;
//    public AudioClip MusicLevel = null;
	
//    private UIButton PlayButton;
//    private UIButton AboutButton;
//    private UIButton ConfigButton;
	
//    public GameObject OpeningPrefab = null;
	
////	private UITextInstance textTitle;
	
////	private uint ChooseOption	= 0;
////	private uint TotalOptions	= 3;
////	private Dictionary< int, string>  OptionsList = new Dictionary< int, string>() ;
	
    public override void Init()
    {
////        OptionsList.Add( 0,  "Start Game");
////        OptionsList.Add( 1,  "Help/Info");
////        OptionsList.Add( 2,  "Quit");	
		
//        if ( OpeningPrefab != null )
//            OpeningPrefab.SetActive(true);
////		textTitle = Managers.Display.text.addTextInstance("eUFOria!", Screen.width * .5f, Screen.height * .25f, 2.0f);

//        ConfigButton = UIButton.create( "ConfigButtonUp.png", "ConfigButtonDown.png", 0, 0);		// ConfigButton (or else)
//        ConfigButton.centerize();
////		ConfigButton.positionFromTopLeft( 0.9f, 0.15f) ;
//        ConfigButton.positionFromTopLeft(0.925f, 0.05f) ;
//        ConfigButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//        ConfigButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => //Managers.Game.SetState(typeof(ConfigMenuState));
//            sender.disabled = true;
			
//            Managers.Game.PushState( typeof(ConfigMenuState) );
//        };
		
//        PlayButton = UIButton.create( "PlayButtonUp.png", "PlayButtonDown.png",  0, 0);  			// Start
//        PlayButton.centerize();
//        PlayButton.positionFromTopLeft( 0.925f, 0.5f) ;
//        PlayButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//        PlayButton.onTouchUpInside += delegate(UIButton sender) 
//        {
////			if ( Managers.Game.State != this ) sender.disabled = true;
			
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => //Managers.Game.SetState( typeof(LevelMenuState) );
//                Managers.Game.ChangeState( typeof(LevelMenuState) );
				
//            sender.disabled = true;
//        };
			
//        AboutButton = UIButton.create( "AboutButtonUp.png", "AboutButtonDown.png",   0, 0);   		// About
//        AboutButton.centerize();
////		AboutButton.positionFromTopLeft( 0.9f, 0.85f) ;
//        AboutButton.positionFromTopLeft( 0.925f, 0.95f) ;
//        AboutButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//        AboutButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => //Managers.Game.SetState(typeof(InfoMenuState));
//            sender.disabled = true;
			
//            Managers.Game.PushState( typeof(InfoMenuState) );
//        };

//        Managers.Audio.Play(ChaChan, Camera.mainCamera.transform.position);
//        //Managers.Audio.StopMusic();
//        Managers.Audio.PlayMusic( MusicLevel, 0.5f, 1.0f);
		
    }
	
    public override void DeInit()
    {
////		textTitle.clear();
//        PlayButton.destroy();
//        AboutButton.destroy();
//        ConfigButton.destroy();

//        //Managers.Audio.st

    }
	
    public override void OnUpdate()
    {

		
////		if (!DisplayMenu)
////        	return;		
////		if( Input.GetKeyDown( "up" ) && ( ChooseOption > 0 ) && !DisplayInfo)
////            ChooseOption--;                    
////   		if( Input.GetKeyDown( "down" ) && ( ChooseOption < ( TotalOptions -1 ) ) && !DisplayInfo)
////            ChooseOption++;
////		
////	    if (Input.GetButtonDown( "Fire1" ) || Input.GetKeyDown("return") )
////	        switch( ChooseOption )
////	        {
////	            case 0:
////	                    Application.LoadLevel(StartLevel);       
////	                    break;
////	            case 1:
////	                    DisplayInfo = !DisplayInfo;
////	                    break;
////	            case 2:
////	                    Application.Quit();
////	                    break;
////	        }
		
////		foreach ( int Option in OptionsList.Keys )
////		{
////			if ( Option == ChooseOption )
////				GUI.color = Color.white;
////			else
////				GUI.color = Color(1, 0.36, 0.22, 1);
////			        
////			GUI.Label( Rect( (Screen.width *.65), (Screen.height *.8) + Option * 32, 400, 50), OptionsList[Option]);
////		}
    }
    public override void Pause()
    {
////		textTitle.hidden = true;
//        PlayButton.hidden = true;
//        AboutButton.hidden = true;
//        ConfigButton.hidden = true;


    }

    public override void Resume()
    {
////		textTitle.hidden = false;
		
//        PlayButton.hidden = false;
//        PlayButton.disabled = false;
		
//        AboutButton.hidden = false;
//        AboutButton.disabled = false;
		
//        ConfigButton.hidden = false;	
//        ConfigButton.disabled = false;

//        Managers.Audio.StopMusic();

    }
}