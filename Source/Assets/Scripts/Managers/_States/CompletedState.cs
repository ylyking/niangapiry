using UnityEngine;
using System.Collections;

public class CompletedState : GameState 
{
    public AudioClip Congratulations;
	
    public override void Init()
	{
        Managers.Audio.PlayMusic(Congratulations, 0.5f, 1.0f);


		
        //RetryButton = UIButton.create( "RetryButtonUp.png", "RetryButtonDown.png", 0, 0);  				// Retry Menu
        //RetryButton.centerize();
        //RetryButton.positionFromTopLeft( 0.925f, 0.05f) ;
        //RetryButton.onTouchUpInside += delegate(UIButton sender) 
        //{
        //    sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
        //    var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
        //    ani.autoreverse = true;
        //    sender.disabled = true;
			
        //    ani.onComplete = () => 	RestartMission();
        //};
		

		
				
//        ContinueButton = UIButton.create( "PlayButtonUp.png", "PlayButtonDown.png", 0, 0);  				// Continue Next mission
//        ContinueButton.centerize();
//        ContinueButton.positionFromTopLeft( 0.925f, 0.95f) ;
//        ContinueButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => NextMission();
			
////			print ("Que Dios te ayude");
//        };
		
	}
	
	private void RestartMission()
	{
        //Managers.Game.PopState();
        //Managers.Game.State.DeInit();
        //Managers.Game.State.Init();
	}
	
	private void QuitMission()
	{
        //Managers.Game.PopState();
        //Managers.Game.ChangeState(typeof(MainMenuState));
	}
	
	private void NextMission()
	{
        //Managers.Game.PopState();
		
        //string NextMission = (Managers.Game.State.GetType()).ToString();						// Get current Mission name
        //int MissionId = int.Parse( NextMission.Substring( NextMission.LastIndexOf("_")+ 1 ) );	// Get current Mission number
        //NextMission = "Mission_" + (MissionId + 1) ; 											// Set NextMission name
		
        //if ( (MissionId + 1) >= Managers.Game.UnlockedStages )									// Unlock New Mission Selection
        //    Managers.Game.UnlockedStages++;
		
        //Managers.Game.ChangeState( System.Type.GetType(NextMission) );							// NextMission == currentMission + 1
        //Managers.Game.PushState( System.Type.GetType( "BriefState_" + (MissionId +1) ) );
		
			
        //Debug.Log ( "Loading: " + NextMission );

	}
	
	
    public override void DeInit()
	{

	}
	
    public override void OnUpdate()
	{
		;
	}
	
	public override void Pause(){;}
	public override void Resume(){;}
	

}