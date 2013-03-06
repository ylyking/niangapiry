using UnityEngine;
using System.Collections;

public class IntroState : GameState 
{
//    private UITextInstance text1;
//    private UISprite Sunhouse;
	
    public override void Init()
    {
//        Sunhouse = UI.firstToolkit.addSprite("Sunhouse.png", 0, 0);
////		Sunhouse.setSize( 320, 320 );
//        Sunhouse.centerize();
//        Sunhouse.positionFromTopLeft( 0.5f, 0.5f) ;

//        var text = Managers.Display.text;
//        text1 = text.addTextInstance("Developed By", Screen.width * .5f, Screen.height * .25f, 0.75f);
////		text2 = text.addTextInstance(" Yesterday tomorrow, today!?", Screen.width * .5f, Screen.height * .5f, 1.0f);
////		text3 = text.addTextInstance("Going To MainMenu/ l", Screen.width * .5f, Screen.height * .65f, 1.0f);
    }
	
    public override void DeInit()
    {
		
//        Sunhouse.destroy();
//        text1.clear();
////		text2.clear();
////		text3.clear();
		
////		print ("Deactivated IntroState");
    }

    public override void OnUpdate()
    {
//        if (Time.time > 2.0 )
//        {
////			Application.LoadLevel("Empty");
//            Managers.Game.ChangeState(typeof(MainMenuState));
//        }
    }

    public override void Pause() { ;}

    public override void Resume() { ;}
}