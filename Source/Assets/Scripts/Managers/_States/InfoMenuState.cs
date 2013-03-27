using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfoMenuState : GameState 
{
    //private UIButton OkButton;
    //private UITextInstance textCredits;
    //private UISprite Sunhouse;
    //private UISprite Window;
	
	
    public override void Init()
    {
    //    Window = UI.firstToolkit.addSprite("Shade.png", 0, 0);
    //    Window.setSize( 400, 400 );
    //    Window.centerize();
    //    Window.positionFromTopLeft( 0.5f, 0.5f) ;
		
    //    Sunhouse = UI.firstToolkit.addSprite("Sunhouse.png", 0, 0);
    //    Sunhouse.setSize( 125, 125 );
    //    Sunhouse.centerize();
    //    Sunhouse.positionFromTopLeft( 0.75f, 0.5f) ;
		

		
    //    textCredits = Managers.Display.text.addTextInstance("Programing Carlixyz \n\n Art Design FAC! \n\n Music MentaBeat",
    //        Screen.width * .5f, Screen.height * .35f, 1.0f);

    //    OkButton = UIButton.create( "PlayButtonUp.png", "PlayButtonDown.png", 0, 0);   		// About
    //    OkButton.centerize();
    //    OkButton.positionFromTopLeft( 0.925f, 0.95f) ;
    //    OkButton.highlightedTouchOffsets = new UIEdgeOffsets( 30 );
    //    OkButton.onTouchUpInside += delegate(UIButton sender) 
    //    {
    //        var ani = sender.scaleFromTo( 0.3f, Vector3.one, new Vector3(1.3f,1.3f,1), Easing.Sinusoidal.easeOut );
    //        ani.autoreverse = true;
    //        ani.onComplete = () => //Managers.Game.SetState( typeof( MainMenuState) );
    //        Managers.Game.PopState();
				
    //        sender.disabled = true;
    //    };
    }
	
    public override void DeInit()
    {
    //    textCredits.clear();
    //    OkButton.destroy();
    //    Sunhouse.destroy();
    //        Window.destroy();
    }

    public override void OnUpdate()
    {
        ;
    }

    public override void Pause() { ;}

    public override void Resume() { ;}
}