using UnityEngine;
using System.Collections;

public class DemoAdvise : GameState 
{
    GUISkin gSkinB = null;
	
    public override void Init()
    {
        Managers.Game.IsPaused = true;

            Time.timeScale = 0.00000000000001f;
            //(player.GetComponent<PlayerControls>() as PlayerControls).enabled = false;
            //( (PlayerControls) player.GetComponent(typeof(PlayerControls)) ).enabled = false;

            //(player.GetComponent<PlayerControls>()as PlayerControls).enabled = true;
        AudioListener.pause = Managers.Game.IsPaused;

        gSkinB = Resources.Load("GUI/GUISkin B") as GUISkin;
        gSkinB.label.fontSize = Mathf.RoundToInt(Screen.width * 0.035f);

    }

    public override void DeInit()
    {
        Managers.Game.IsPaused = false;
        Time.timeScale = 1;

        //(player.GetComponent<PlayerControls>()as PlayerControls).enabled = true;
        AudioListener.pause = Managers.Game.IsPaused;


        //        if ( Managers.Audio.SoundEnable )
        //            Managers.Audio.ResumeMusic();
    }
	
    private void RestartMission()
    {
////		Managers.Missions.RestartMission();
//        Managers.Game.PopState();
//        Managers.Game.State.DeInit();
//        Managers.Game.State.Init();
    }
	


    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape") || Input.GetButtonDown("Start") || Input.GetButtonDown("Fire1"))
            Managers.Game.PopState();

    }

    public override void OnRender()
    {
        if (Managers.Game.IsPaused)
        {
            if (gSkinB) GUI.skin = gSkinB;

            GUI.color = new Color(1, 0.36f, 0.22f, 1);
            GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .25f),
                                 (Screen.height * .5f) - (Screen.height * .15f),
                                 (Screen.width * .5f), (Screen.height * .35f)),
                                    "\n\n NIVEL NO DISPONIBLE \n\n Aguarda el día \n de Estreno para disfrutar \n la version Completa ");
            GUI.color = Color.clear;
            return;
        }
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}