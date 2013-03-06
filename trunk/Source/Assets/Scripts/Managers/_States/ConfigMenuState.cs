using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigMenuState : GameState 
{
//    private UIButton OkButton;
//    private UIButton ResetButton;
//    private UIToggleButton toggleMusic;
//    private UIToggleButton toggleButton;
	
//    private UITextInstance textSound;
//    private UITextInstance textReset;
//    private UITextInstance textInfo;
//    private UISprite Window;

        public override void Init()
        {
//        Window = UI.firstToolkit.addSprite("Shade.png", 0, 0);
//        Window.setSize( 400, 400 );
//        Window.centerize();
//        Window.positionFromTopLeft( 0.65f, 0.5f) ;
		
		
//        textInfo = Managers.Display.text.addTextInstance("Swap HUD Format System:",			// HUD System Setup
//            Screen.width * .5f, Screen.height * .4f, 1.0f);
	
//        if ( PlayerPrefs.GetInt("HUDCorned" ) == 1 )
//            Managers.Display.HUDCorned = true;
//        else 
//            Managers.Display.HUDCorned = false;
		
//                // Toggle Button
//        toggleButton = UIToggleButton.create( "OptionButtonUp.png", "OptionButtonDown.png", "OptionButtonHighlight.png", 0, 0 );
//        toggleButton.positionFromTopLeft( .4f, .25f );
//        toggleButton.selected = !Managers.Display.HUDCorned;
		
//        toggleButton.onToggle += ( sender, newValue ) => ToggleHUD( sender, newValue) ;
//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
//        textSound = Managers.Display.text.addTextInstance("Sound & Music Setup:",			// Sound Enable Setup
//            Screen.width * .5f, Screen.height * .6f, 1.0f);
	
//        if ( PlayerPrefs.GetInt("SoundAble" ) == 1 )
//            Managers.Audio.SoundEnable = true;
//        else 
//            Managers.Audio.SoundEnable = false;	
//                // Toggle Button
//        toggleMusic = UIToggleButton.create( "OptionButtonUp.png", "OptionButtonDown.png", "OptionButtonHighlight.png", 0, 0 );
////		toggleMusic.centerize();
//        toggleMusic.positionFromTopLeft( .6f, .25f );
//        toggleMusic.selected = Managers.Audio.SoundEnable;
		
//        toggleMusic.onToggle += ( sender, newValue ) => ToggleSound( sender, newValue) ;
//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
		

//        textReset = Managers.Display.text.addTextInstance("Reset Game Missions:\n WARNING: will delete\n All Score & Data",			// Game Reset Setup
//            Screen.width * .5f, Screen.height * .8f, 1.0f);
	
//        ResetButton = UIButton.create( "RetryButtonUp.png", "RetryButtonDown.png", 0, 0);		// ResetButton (or else)
////		ResetButton.centerize();
//        ResetButton.positionFromTopLeft( .8f, .25f );
//        ResetButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            Managers.Game.UnlockedStages = 1;
//            ani.onComplete = () => PlayerPrefs.SetInt("UnlockedStages", 1);
			
//            for (int i = 1; i < 8; i++) 	{
//                PlayerPrefs.SetInt("Top Score " + i , 500);
//            }
				
//            sender.disabled = true;
//        };
//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////		
		
//        OkButton = UIButton.create( "PlayButtonUp.png", "PlayButtonDown.png", 0, 0);		// ConfigButton (or else)
//        OkButton.centerize();
//        OkButton.positionFromTopLeft(0.925f, 0.05f) ;
//        OkButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//        OkButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => Managers.Game.PopState();
				
//            sender.disabled = true;
//        };

        }
	
//    public void ToggleHUD( UIToggleButton sender, bool Value)
//    {
//        if(sender.selected)
//            Managers.Display.HUDCorned = false;
//        else
//            Managers.Display.HUDCorned = true;
		
//        toggleButton.selected = Value;
		
//        if ( Managers.Display.HUDCorned )
//            PlayerPrefs.SetInt("HUDCorned", 1);
//        else
//            PlayerPrefs.SetInt("HUDCorned", 0);
//    }
	
//    public void ToggleSound( UIToggleButton sender, bool Value)
//    {
//        if(sender.selected)
//            Managers.Audio.SoundEnable = true;
//        else
//            Managers.Audio.SoundEnable = false;
		
//        toggleMusic.selected = Value;
		
//        if ( Managers.Audio.SoundEnable )
//            PlayerPrefs.SetInt("SoundAble", 1);
//        else
//            PlayerPrefs.SetInt("SoundAble", 0);
//    }

        public override void DeInit()
        {
//        textSound.clear();
//        textInfo.clear();
//        textReset.clear ();
//        OkButton.destroy();
//        ResetButton.destroy();
//        toggleButton.destroy();
//        toggleMusic.destroy();
//            Window.destroy();
		
    }
	
    public override void OnUpdate()
    {
//        if ( !Managers.Audio.SoundEnable )
//            textSound.text = "Sound & Music Setup:  \n Disabled";
//        else
//            textSound.text = "Sound & Music Setup:  \n Enabled ";
		
//        if ( !Managers.Display.HUDCorned )
//            textInfo.text = "Swap HUD Format System: \n all in 1 Corner      ";
//        else
//            textInfo.text = "Swap HUD Format System: \n arranged in 4 Corners";
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
}