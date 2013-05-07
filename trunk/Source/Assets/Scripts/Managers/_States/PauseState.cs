using UnityEngine;
using System.Collections;

public class PauseState : GameState 
{
	
    public override void Init()
    {
        Managers.Game.IsPaused = true;

            Time.timeScale = 0.00000000000001f;
            //(player.GetComponent<PlayerControls>() as PlayerControls).enabled = false;
            //( (PlayerControls) player.GetComponent(typeof(PlayerControls)) ).enabled = false;

            //(player.GetComponent<PlayerControls>()as PlayerControls).enabled = true;
        AudioListener.pause = Managers.Game.IsPaused;

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
	
    private void ReturnMap()
    {
        Managers.Game.PopState();
        Managers.Game.PopState();
//        Managers.Game.ChangeState(typeof(MainMenuState));
    }
	



    public override void OnUpdate()
    {
        if (Input.GetKeyDown("escape"))
            Managers.Game.PopState();

        if (Managers.Game.IsPaused && Input.GetKeyDown("q"))
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
            GUI.color = new Color(1, 0.36f, 0.22f, 1);
            GUI.Box(new Rect((Screen.width * .5f) - (Screen.width * .15f),
                                 (Screen.height * .5f) - (Screen.height * .15f),
                                 (Screen.width * .35f), (Screen.height * .35f)),
                                    "\n\n - PAUSE - \n\n press 'Q' to Quit Game \n and return Main Menu ");
            GUI.color = Color.clear;
            return;
        }
    }

    public override void Pause() { ;}
    public override void Resume() { ;}
	

}