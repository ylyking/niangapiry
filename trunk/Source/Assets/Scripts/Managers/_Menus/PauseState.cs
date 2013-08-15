using UnityEngine;
using System.Collections;

public class PauseState : GameState 
{
    GUISkin gSkinB = null;
    public AudioClip Pip;
	
    public override void Init()
    {
        Managers.Game.IsPaused = true;
        Managers.Audio.Play(Pip, Managers.Display.camTransform);

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
        Managers.Audio.Play(Pip, Managers.Display.camTransform);


        //        if ( Managers.Audio.SoundEnable )
        //            Managers.Audio.ResumeMusic();
    }
	
//    private void RestartMission()
//    {
//////		Managers.Missions.RestartMission();
////        Managers.Game.PopState();
////        Managers.Game.State.DeInit();
////        Managers.Game.State.Init();
//    }
	
    private void ReturnMap()
    {
        Managers.Game.PopState();
        Managers.Game.PopState();
//        Managers.Game.ChangeState(typeof(MainMenuState));
    }
	

    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape") || Input.GetButtonDown("Start") )
            Managers.Game.PopState();

        if (Managers.Game.IsPaused && Input.GetKeyDown("q") || Input.GetButtonDown("Select"))
        {
            Managers.Game.IsPaused = false;
            Managers.Dialog.StopConversation();
            ReturnMap();
        }
    }

    public override void OnRender()
    {
        if (Managers.Game.IsPaused)
        {
            if (gSkinB) GUI.skin = gSkinB;

            GUI.color = new Color(1, 0.36f, 0.22f, 1);
            GUI.Box(new Rect(((Screen.width * .5f) - 180), (Screen.height * .5f) - 128, 360, 256),
                    "\n\n - PAUSA - \n\n\n\n presiona 'Q' o 'Select' \n para Salir del Nivel \n y volver al Mapa principal");
                                    //"\n\n - PAUSE - \n\n press 'Q' to Quit Game \n and return Main Menu ");
            GUI.color = Color.clear;
            return;
        }
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}