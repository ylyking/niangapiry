using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailerState : GameState		
{

    public MovieTexture videoClip;
    //public Material VideoMat;

    public AudioClip videoSound;

	
    public override void Init()
	{
        Screen.showCursor = false;


        videoClip.Stop();
        videoClip.Play();

        videoSound = videoClip.audioClip;
        Managers.Audio.PlayMusic(videoSound, 1, 1);

        Debug.Log("Entering Trailer State");
	}
	
    public override void DeInit()
	{
        videoClip.Stop();
        Managers.Audio.StopMusic();
        Screen.showCursor = true;

        Debug.Log("Exit the current State and returning main menu");

	}
	
    public override void OnUpdate()
	{
        if (Input.anyKeyDown || !videoClip.isPlaying)
            Managers.Game.ChangeState(typeof(MainMenuState));

  
	}

    public override void OnRender()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), videoClip, ScaleMode.StretchToFill);
    }

	
	public override void Pause()
	{

	}
	
	public override void Resume()
	{
   
	}

	
}
