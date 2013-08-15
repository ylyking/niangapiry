using UnityEngine;
using System.Collections;

public class GameOverState : GameState 
{
    float timeLapse = 7;
    GUISkin gSkinB = null;

    public override void Init()
    {
        gSkinB = Resources.Load("GUI/GUISkin B") as GUISkin;
        gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);

        timeLapse = 5;
        Managers.Game.IsPlaying = false;

        if (Managers.Register.Score > Managers.Register.TopScore)
            Managers.Register.TopScore = Managers.Register.Score;

        //ShowFlash(2.0f);

        //Application.LoadLevel("Intro");
        //Managers.Game.PopState();
        //Managers.Register.SoftReset();
    }
	
    public override void DeInit()
    {
        Managers.Register.SoftReset();
        //Managers.Register.HardReset();
        Managers.Game.IsPlaying = true;
    }

    public override void OnUpdate()
    {
        timeLapse -= Time.deltaTime;

        if ( Input.anyKey)
            timeLapse -= Time.deltaTime;


        if (timeLapse < 0)
        {
            timeLapse = 7;
            Managers.Display.ShowFlash(2);
            Managers.Game.PopState();
            Managers.Game.PopState();
        }
    }

    public override void OnRender()
    {
        if (gSkinB) GUI.skin = gSkinB;

        //GUI.skin.label.fontSize = 64;
        //GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.color = Color.magenta;
        GUI.Label(new Rect((Screen.width * .5f) - Screen.width * .25f, (Screen.height * .5f), 100, 50), "- JUEGO TERMINADO -");
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}