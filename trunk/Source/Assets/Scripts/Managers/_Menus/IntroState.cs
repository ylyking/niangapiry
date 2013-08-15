using UnityEngine;
using System.Collections;

public class IntroState : GameState 
{
    public override void Init()
    {
        Managers.Display.ShowFadeOut(10);
    }
	
    public override void DeInit()
    {
        Managers.Display.ShowFadeIn(10);
        //print("Deactivated IntroState");
    }

    public override void OnUpdate()
    {
        if ((int)Time.time == 1 )
        {
            //Managers.Display.ShowImage("D:/Niangapiry/Source/Assets/Materials/Textures/sunhouse.png", 7);
            Managers.Display.ShowImage("GUI/sunhouse", 7);
        }

        if ((int)Time.time == 2 )
            Managers.Display.ShowFadeIn(.5f);

        if ((int)Time.time == 7 )
            Managers.Display.ShowFadeOut(2);

        if ((int)Time.time == 9 )
        {
            //			Application.LoadLevel("Empty");
            Managers.Game.ChangeState(typeof(MainMenuState));
        }
    }

    public override void Pause() { ;}

    public override void Resume() { ;}
}