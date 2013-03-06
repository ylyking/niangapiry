using UnityEngine;
using System.Collections;

public class PauseState : GameState 
{
//    private UITextInstance text2;
//    private UIButton PlayButton;
	
//    private UIButton RetryButton;
//    private UIButton QuitButton;
//    private UIButton HelpButton;
	
	
    public override void Init()
    {
//        var text = Managers.Display.text;
//        text2 = text.addTextInstance( "PAUSE", Screen.width * .5f, Screen.height * .45f, 1.0f);
		
//        Managers.Audio.PauseMusic();

//        PlayButton = UIButton.create( "PlayButtonUp.png", "PlayButtonDown.png",  0, 0); 				// Continue
//        PlayButton.centerize();
//        PlayButton.positionFromTopLeft( 0.075f, 0.05f) ; 	//0.05f, 0.075f
//        PlayButton.onTouchUpInside += delegate(UIButton sender) 
//        {
////			Time.timeScale = 1;
				
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => Managers.Game.PopState();
//            sender.disabled = true;
			
////			Managers.Game.SetState( typeof(GamePlayState), 1 );
////			Managers.Game.PopState( typeof(GamePlayState));
//        };
		
		
//        RetryButton = UIButton.create( "RetryButtonUp.png", "RetryButtonDown.png", 0, 0);  				// Retry Menu
//        RetryButton.centerize();
//        RetryButton.positionFromTopLeft( 0.925f, 0.05f) ;
//        RetryButton.onTouchUpInside += delegate(UIButton sender) 
//        {
////			Time.timeScale = 1;
			
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            sender.disabled = true;
			
//            ani.onComplete = () => 	RestartMission();
//        };
		
//        QuitButton = UIButton.create( "QuitButtonUp.png", "QuitButtonDown.png", 0, 0); 				// Quit main Menu
//        QuitButton.centerize();
//        QuitButton.positionFromTopLeft( 0.925f, 0.5f) ;
//        QuitButton.onTouchUpInside += delegate(UIButton sender) 
//        {
////			Time.timeScale = 1;
			
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            sender.disabled = true;
			
//            ani.onComplete = () =>	QuitMission();
//        };
		
				
//        HelpButton = UIButton.create( "HelpButtonUp.png", "HelpButtonDown.png", 0, 0);  				// Quit main Menu
//        HelpButton.centerize();
//        HelpButton.positionFromTopLeft( 0.925f, 0.95f) ;
//        HelpButton.onTouchUpInside += delegate(UIButton sender) 
//        {
////			Time.timeScale = 1;
			
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => HelpMission();
//                sender.disabled = true;
//        };
		
////		Time.timeScale = 0.0000001f;
		
////		print ("Activated PauseState");
    }
	
    private void RestartMission()
    {
////		Managers.Missions.RestartMission();
//        Managers.Game.PopState();
//        Managers.Game.State.DeInit();
//        Managers.Game.State.Init();
    }
	
    private void QuitMission()
    {
//        Managers.Game.PopState();
//        Managers.Game.ChangeState(typeof(MainMenuState));
////		Managers.Game.PopState();
    }
	
    private void HelpMission()
    {
//        Managers.Game.PopState();
//        Managers.Game.ShowHelp();
    }
	
	
    public override void DeInit()
    {
//        Time.timeScale = 1;
		
//        if ( Managers.Audio.SoundEnable )
//            Managers.Audio.ResumeMusic();
		
//        text2.clear();
//        PlayButton.destroy();
//        RetryButton.destroy();
//        QuitButton.destroy();
//        HelpButton.destroy();
		
////		print ("Deactivated");
    }
    public override void OnUpdate()
    {
        ;
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}