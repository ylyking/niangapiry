using UnityEngine;
using System.Collections;

public class GameOverState : GameState 
{
//    private UITextInstance text1;
	
//    private UIButton RetryButton;
//    private UIButton QuitButton;
//    private UIButton HelpButton;
//    public AudioClip YouLose = null;
	
	
    public override void Init()
    {
//        Managers.Audio.PlayMusic(YouLose, 0.5f, 1.0f);

//        var text = Managers.Display.text;
//        text1 = text.addTextInstance( "GAME OVER", Screen.width * .5f, Screen.height * .45f, 1.5f);

//        RetryButton = UIButton.create( "RetryButtonUp.png", "RetryButtonDown.png", 0, 0);  				// Retry Menu
//        RetryButton.centerize();
//        RetryButton.positionFromTopLeft(0.925f, 0.05f) ;
//        RetryButton.onTouchUpInside += delegate(UIButton sender) 
//        {
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
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => sender.disabled = true;
			
//            print ("Que Dios te ayude");
//        };

    }
	
    private void RestartMission()
    {
//        Managers.Game.PopState();
//        Managers.Game.State.DeInit();
//        Managers.Game.State.Init();
    }

    private void QuitMission()
    {
        Managers.Game.PopState();
        Managers.Game.ChangeState(typeof(MainMenuState));
        //		Managers.Game.PopState();
    }


    public override void DeInit()
    {
        //text1.clear();
        //RetryButton.destroy();
        //QuitButton.destroy();
        //HelpButton.destroy();
    }

    public override void OnUpdate()
    {
        ;
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}