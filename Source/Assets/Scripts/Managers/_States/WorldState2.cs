using UnityEngine;
using System.Collections;

public class WorldState2 : InGameState
{
	public AudioClip MusicMap;
	
    public override void Init()
    {
        Managers.Game.IsPlaying = true;
		
//		Managers.Audio.PlayMusic( MusicMap, 1, 1);

        if (Managers.Register.YaguaDefeated && Managers.Register.MonaiDefeated && !Managers.Register.YasiYatereDefeated)
        {
            if (Managers.Register.UnlockedStages <= 3)
                Managers.Register.UnlockedStages = 4;
            Managers.Tiled.Load("/Levels/Home2.tmx");
        }
        else if (Managers.Register.YasiYatereDefeated)
        {
            if (Managers.Register.UnlockedStages <= 4)
                Managers.Register.UnlockedStages = 5;
            Managers.Tiled.Load("/Levels/Home3.tmx");
        }
        else
            Managers.Tiled.Load("/Levels/Home1.tmx");

        Managers.Display.camTransform.position = new Vector3(3.75f, 2.5f, 0);

        base.Init();

    }

    public override void DeInit()
    {
        //Debug.Log("Exit the current State and returning map");
        Managers.Register.HomeFile = Managers.Register.currentLevelFile;

        Managers.Tiled.Unload();
        base.DeInit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override void Pause()
    {

    }

    public override void Resume()
    {

    }
}
