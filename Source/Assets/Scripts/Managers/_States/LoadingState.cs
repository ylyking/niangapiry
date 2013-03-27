using UnityEngine;
using System.Collections;

public class LoadingState : GameState 
{
    //private UITextInstance text1;
    //private UITextInstance text2;
	
    public override void Init()
    {
		
    //    var text = Managers.Display.text;
    //    text1 = text.addTextInstance( "Activated LoadingState", 10, 10, 2.0f);
    //    text2 = text.addTextInstance( "Now Loading Something", Screen.width * .5f, Screen.height * .5f, 1.0f);

    //    print ("Activated IntroState");
    }
	
    public override void DeInit()
    {
    //    text1.clear();
    //    text2.clear();
		
    //    print ("Deactivated");
    }
    public override void OnUpdate()
    {
        ;
    }
    public override void Pause() { ;}

    public override void Resume() { ;}
}