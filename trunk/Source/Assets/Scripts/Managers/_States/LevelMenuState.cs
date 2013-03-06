using UnityEngine;
using System.Collections;

public class LevelMenuState : GameState 
{
//    private UITextInstance textHelp;
//    private UIButton QuitButton;
//    public GameObject OpeningPrefab = null;
	
//    private UIButton[] slots;
//    private int Rows	= 2;
//    private int Columns	= 5;
	 
	
    public override void Init()
    {
		
//        var text = Managers.Display.text;
//        this.textHelp = text.addTextInstance( "Choose Mission:", Screen.width * .25f, Screen.height * .85f, 2.0f);
		
//        QuitButton = UIButton.create( "QuitButtonUp.png", "QuitButtonDown.png", 0, 0);
//        QuitButton.centerize();
//        QuitButton.positionFromTopLeft( 0.925f, 0.5f);
//        QuitButton.onTouchUpInside += delegate(UIButton sender) 
//        {
//            sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//            var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//            ani.autoreverse = true;
//            ani.onComplete = () => //Managers.Game.SetState( typeof( MainMenuState) );
//            sender.disabled = true;
			
//            Managers.Game.SetState( typeof(MainMenuState), 1);
//        };
		
//        int index 		= 0;
////		int TotalSlots 	= Managers.Missions.UnlockedStages;
//        int TotalSlots 	= Managers.Game.UnlockedStages;
//        Debug.Log( "Missions Able: " + TotalSlots );
		
//        slots = new UIButton[Columns*Rows] ;
		
//        for ( int y = 0; y < Rows; y++) 
//        {		
//            for (int x = 0; x < Columns; x++, index++) 
//            {
//                if ( index < TotalSlots )
//                {
//                    string ButtonName = "Mission0" + index;
//                    slots[index] = UIButton.create( ButtonName + "Up.png", ButtonName + "Down.png", 0, 0);
//                    slots[index].centerize();
//                    slots[index].positionFromTopLeft( 0.6f + y * 0.15f, 0.1f + ( x * 0.2f) );
					
//                    slots[index].onTouchUpInside += OnSlotClicked;
//                }
//                else
//                {
//                    slots[index] = UIButton.create( "LockedButtonUp.png", "LockedButtonUp.png", 0, 0 );
//                    slots[index].centerize();
//                    slots[index].positionFromTopLeft( 0.6f + y * 0.15f, 0.1f + ( x * 0.2f) );
					
//                    slots[index].onTouchUpInside += ( sender ) => Debug.Log("First you must unlock previous level");
//                }
//            }
//        }

    }
	
    public override void DeInit()
    {
//        textHelp.clear();
//        QuitButton.destroy();
		
//        foreach ( UIButton slut in slots )
//            slut.destroy();

    }

    public override void OnUpdate()
    {
        ;
    }

//    private void OnSlotClicked( UIButton sender )
//    {
//        if (Managers.Game.State != this )return;
		
//        sender.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
//        var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
//        ani.autoreverse = true;
		
//        if ( (sender.index - 17) < 0 || (sender.index - 17) > Managers.Game.UnlockedStages) 
//        {
//            Debug.LogWarning("Level not Scripted or unlocked yet: " + sender.index);
//            return;
//        }
//            Debug.Log("Mission 0" + (sender.index - 17));
////		string NextMission = "Mission_" + ( sender.index - 16);
////		Debug.Log ( NextMission);
//        ani.onComplete = () => SetNextMission(  sender.index - 17 );
		
////		ani.onComplete = () => Managers.Game.PushState( System.Type.GetType(NextMission) );
////		ani.onComplete = () => Managers.Game.ChangeState( System.Type.GetType(NextMission) );
//        sender.disabled = true;
//    }
	
//    public void SetNextMission(int MissionId)
//    {
//        if ( OpeningPrefab != null )
//            OpeningPrefab.SetActive(false);
		
//        Managers.Game.ChangeState( System.Type.GetType("Mission_" + MissionId ) );
//        if( MissionId > 0)
//            Managers.Game.PushState( System.Type.GetType( "BriefState_" + MissionId ) );
//    }
	
    public override void Pause()
    {
//        textHelp.hidden = true;
//        QuitButton.hidden = true;
		
//        foreach ( UIButton slut in slots )
//        {
//            slut.hidden = true;
//            slut.disabled = true;
//        }
    }
	
    public override void Resume()
    {
//        textHelp.hidden = false;
//        QuitButton.hidden = false;
		
//        foreach ( UIButton slut in slots )
//        {
//            slut.hidden = false;
//            slut.disabled = false;
//        }
    }
}